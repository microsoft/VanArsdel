// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using VanArsdel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VanArsdel.Devices;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace VanArsdel
{
    public sealed partial class DeviceLayout : UserControl
    {
        private static readonly float _perspectiveDistance = 1000f;
        private static readonly float _offsetAnimSnap = 1f;
        private static readonly float _deviceWidth = 600;
        private static readonly float _deviceSpacing = 24;
        private static readonly float _defaultDeviceHeight = 1080f;

        private static readonly TimeSpan _offsetAnimDuration = TimeSpan.FromMilliseconds(500);
        private static readonly CubicBezierEasingFunction _offsetEasing = Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));

        private static readonly TimeSpan _introAnimDuration = TimeSpan.FromMilliseconds(300);
        private static readonly CubicBezierEasingFunction _introAnimEasing = Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.09f), new Vector2(0.2f, 1.0f));
        private static readonly float _introZOrigin = -800f;

        private static readonly TimeSpan _outroAnimDuration = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan _outroAnimOffsetDuration = TimeSpan.FromMilliseconds(1000);
        private static readonly CubicBezierEasingFunction _outroAnimEasing = Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));
        private static readonly float _outroZDestination = -800f;

        private static readonly Color AmbientDayColor = Colors.AntiqueWhite;
        private static readonly Color AmbientNightColor = Colors.DimGray;

        public DeviceLayout()
        {
            this.InitializeComponent();
            _camera = new CompositionCamera(ElementCompositionPreview.GetElementVisual(LayoutContainer), true, false);
            _camera.PerspectiveDistance = _perspectiveDistance;
        }

        private enum ContainerState { Normal, Loading, Entering, Exiting, Exited };

        private class DeviceContainer
        {
            public DeviceContainer(Device device, LampControl deviceElement)
            {
                State = ContainerState.Entering;
                Device = device;
                DeviceElement = deviceElement;
                DeviceVisual = ElementCompositionPreview.GetElementVisual(DeviceElement);
                OffsetDestination = Vector3.Zero;
            }

            public ContainerState State { get; set; }
            public Device Device { get; }
            public Vector3 OffsetDestination;
            public LampControl DeviceElement;
            public Visual DeviceVisual;
            public CompositionScopedBatch AnimBatch;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DayMode)
            {
                AmbientA.Color = AmbientDayColor;
            }
            else
            {
                AmbientA.Color = AmbientNightColor;
            }
        }

        #region ActiveDevicesProperty

        public static readonly DependencyProperty ActiveDevicesProperty = DependencyProperty.Register("ActiveDevices", typeof(IReadOnlyList<Device>), typeof(DeviceLayout), new PropertyMetadata(null, new PropertyChangedCallback(OnActiveDevicesPropertyChanged)));

        private static void OnActiveDevicesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeviceLayout target)
            {
                target.OnActiveDevicesChanged(e.OldValue as IReadOnlyList<Device>, e.NewValue as IReadOnlyList<Device>);
            }
        }

        private void OnActiveDevicesChanged(IReadOnlyList<Device> oldValue, IReadOnlyList<Device> newValue)
        {
            List<Device> added = new List<Device>(newValue.Count);
            List<DeviceContainer> removed = new List<DeviceContainer>(_containers.Count);
            foreach (var container in _containers)
            {
                if (!newValue.Contains(container.Device))
                {
                    removed.Add(container);
                }
            }
            foreach (var device in newValue)
            {
                if (GetContainer(device) == null)
                {
                    added.Add(device);
                }
            }
            foreach (var remove in removed)
            {
                ExitContainer(remove);
            }
            foreach (var add in added)
            {
                AddNewContainer(add);
            }
        }

        public IReadOnlyList<Device> ActiveDevices
        {
            get { return GetValue(ActiveDevicesProperty) as List<Device>; }
            set { SetValue(ActiveDevicesProperty, value); }
        }

        #endregion

        #region DayModeProperty

        public static readonly DependencyProperty DayModeProperty = DependencyProperty.Register("DayMode", typeof(bool), typeof(DeviceLayout), new PropertyMetadata(true, OnDayModePropertyChanged));

        private static void OnDayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DeviceLayout target)
            {
                target.OnDayModeChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        private void OnDayModeChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                AmbientA.Color = AmbientDayColor;
            }
            else
            {
                AmbientA.Color = AmbientNightColor;
            }
        }

        public bool DayMode
        {
            get { return (bool)GetValue(DayModeProperty); }
            set { SetValue(DayModeProperty, value); }
        }

        #endregion

        private CompositionCamera _camera;

        private bool _disableAnimations = false;
        public bool DisableAnimations
        {
            get { return _disableAnimations; }
            set { _disableAnimations = value; }
        }

        public Visibility DeviceVisibility
        {
            get { return LayoutContainer.Opacity > 0 ? Visibility.Visible : Visibility.Collapsed; }
            set { LayoutContainer.Opacity = value == Visibility.Visible ? 1 : 0; }
        }
        
        private List<DeviceContainer> _containers = new List<DeviceContainer>();

        private DeviceContainer GetContainer(Device device)
        {
            return _containers.FirstOrDefault((a) => a.Device == device);
        }

        private DeviceContainer GetContainer(LampControl control)
        {
            return _containers.FirstOrDefault((a) => a.DeviceElement == control);
        }

        private void AddNewContainer(Device device)
        {
            LampControl element = new LampControl();
            element.Width = _deviceWidth;
            element.SetValue(Canvas.ZIndexProperty, 2);
            element.LayoutSizeUpdated += Element_LayoutSizeUpdated;
            element.AssetLoadComplete += Element_AssetLoadComplete;
            element.Visibility = Visibility.Collapsed;
            var container = new DeviceContainer(device, element);
            container.State = ContainerState.Loading;

            _containers.Add(container);
            _containers.Sort((a, b) => a.Device.CompareTo(b.Device));

            element.Device = device;
            LayoutContainer.Children.Add(element);

            element.InitializeComp();
        }

        private void Element_AssetLoadComplete(LampControl element)
        {
            element.AssetLoadComplete -= Element_AssetLoadComplete;

            var container = GetContainer(element);
            container.State = ContainerState.Entering;

            UpdateLayoutBounds(UpdateDeviceContainerAnimMode.Enter);

            int index = _containers.IndexOf(container);

            UpdateDeviceContainers(UpdateDeviceContainerAnimMode.Enter);

            if (_disableAnimations)
            {
                container.DeviceVisual.Offset = new Vector3(container.OffsetDestination.X, container.OffsetDestination.Y, 0f);
                container.DeviceVisual.Opacity = 1f;
                container.DeviceElement.SetValue(Canvas.ZIndexProperty, 101);
                container.State = ContainerState.Normal;
            }
            else
            {
                float containerOffsetY = _layoutHeight - element.LayoutSize.Y + _layoutOrigin.Y;
                container.DeviceVisual.Offset = new Vector3(_layoutOrigin.X + index * (_deviceSpacing + _deviceWidth), containerOffsetY, _introZOrigin);
                container.DeviceVisual.Opacity = 0f;

                var compositor = Window.Current.Compositor;

                container.AnimBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

                var introOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
                introOffsetAnim.InsertKeyFrame(1f, new Vector3(container.OffsetDestination.X, container.OffsetDestination.Y, 0f), _introAnimEasing);
                introOffsetAnim.Duration = _introAnimDuration;
                var introOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
                introOpacityAnim.InsertKeyFrame(1f, 1f, _introAnimEasing);
                introOpacityAnim.Duration = _introAnimDuration;

                container.DeviceVisual.StartAnimation("Offset", introOffsetAnim);
                container.DeviceVisual.StartAnimation("Opacity", introOpacityAnim);

                container.AnimBatch.Completed += EnterAnimBatch_Completed;
                container.AnimBatch.End();
            }

            element.Visibility = Visibility.Visible;
        }

        private void EnterAnimBatch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            CompositionScopedBatch sourceBatch = sender as CompositionScopedBatch;
            DeviceContainer container = _containers.FirstOrDefault((a) => a.AnimBatch == sourceBatch);
            if (container == null)
            {
                throw new Exception("Enter scoped batch completed for unknown container");
            }

            container.DeviceElement.SetValue(Canvas.ZIndexProperty, 101);

            container.State = ContainerState.Normal;
        }

        private void ExitContainer(DeviceContainer container)
        {
            if (container.State == ContainerState.Exiting || container.State == ContainerState.Exited)
            {
                return;
            }
            container.State = ContainerState.Exiting;

            if (container.AnimBatch != null)
            {
                container.AnimBatch.Dispose();
                container.AnimBatch = null;
            }

            if (_disableAnimations)
            {
                UpdateLayoutBounds(UpdateDeviceContainerAnimMode.Exit);
                UpdateDeviceContainers(UpdateDeviceContainerAnimMode.Exit);

                RemoveContainer(container);
            }
            else
            {
                container.DeviceElement.SetValue(Canvas.ZIndexProperty, 1);

                var compositor = Window.Current.Compositor;

                container.AnimBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

                var outroOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
                outroOffsetAnim.InsertKeyFrame(1f, new Vector3(container.OffsetDestination.X, container.OffsetDestination.Y, _outroZDestination), _outroAnimEasing);
                outroOffsetAnim.Duration = _outroAnimDuration;
                var outroOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
                outroOpacityAnim.InsertKeyFrame(1f, 0f, _outroAnimEasing);
                outroOpacityAnim.Duration = _outroAnimDuration;

                container.DeviceVisual.StartAnimation("Offset", outroOffsetAnim);
                container.DeviceVisual.StartAnimation("Opacity", outroOpacityAnim);

                container.AnimBatch.Completed += ExitAnimBatch_Completed;
                container.AnimBatch.End();

                UpdateLayoutBounds(UpdateDeviceContainerAnimMode.Exit);
                UpdateDeviceContainers(UpdateDeviceContainerAnimMode.Exit);
            }
        }

        private void ExitAnimBatch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            CompositionScopedBatch sourceBatch = sender as CompositionScopedBatch;
            DeviceContainer container = _containers.FirstOrDefault((a) => a.AnimBatch == sourceBatch);
            if (container == null)
            {
                throw new Exception("Exit scoped batch completed for unknown container");
            }

            RemoveContainer(container);
        }

        private void RemoveContainer(DeviceContainer container)
        {
            container.DeviceElement.LayoutSizeUpdated -= Element_LayoutSizeUpdated;
            container.DeviceElement.AssetLoadComplete -= Element_AssetLoadComplete;
            container.State = ContainerState.Exited;
            LayoutContainer.Children.Remove(container.DeviceElement);
            container.DeviceElement = null;
            container.DeviceVisual = null;
            if (container.AnimBatch != null)
            {
                container.AnimBatch.Dispose();
                container.AnimBatch = null;
            }
            container.OffsetDestination = Vector3.Zero;

            _containers.Remove(container);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOuterSize();
        }

        private float _deadZoneLeft = 344f;
        public double DeadZoneLeft
        {
            get { return _deadZoneLeft; }
            set
            {
                if (_deadZoneLeft != (float)value)
                {
                    _deadZoneLeft = (float)value;
                    UpdateOuterSize();
                }
            }
        }

        private float _deadZoneRight = 368f;
        public double DeadZoneRight
        {
            get { return _deadZoneRight; }
            set
            {
                if (_deadZoneRight != (float)value)
                {
                    _deadZoneRight = (float)value;
                    UpdateOuterSize();
                }
            }
        }

        private void UpdateOuterSize()
        {
            float w = (float)LayoutContainer.ActualWidth;
            float h = (float)LayoutContainer.ActualHeight;
            if (float.IsNaN(w) || w <= 0f || float.IsNaN(h) || h <= 0f)
            {
                _viewWidth = 0f;
                _viewHeight = 0f;
            }
            else
            {
                _viewWidth = w - _deadZoneLeft - _deadZoneRight;
                _viewHeight = h;
            }

            LayoutTranslateTransform.X = _deadZoneLeft;

            UpdateCamera(UpdateDeviceContainerAnimMode.Normal);
            UpdateDeviceContainers(UpdateDeviceContainerAnimMode.Normal);
        }

        private float _viewWidth = 0f;
        private float _viewHeight = 0f;

        private void UpdateLayoutBounds(UpdateDeviceContainerAnimMode mode)
        {
            int containersInLayout = 0;
            _layoutHeight = 0;
            for (int i = 0; i < _containers.Count; i++)
            {
                if (_containers[i].State != ContainerState.Exited && _containers[i].State != ContainerState.Exiting)
                {
                    containersInLayout++;
                    _layoutHeight = Math.Max(_layoutHeight, _containers[i].DeviceElement.LayoutSize.Y);
                }
            }
            if (_layoutHeight == 0)
            {
                _layoutHeight = _defaultDeviceHeight;
            }

            _layoutWidth = containersInLayout * _deviceWidth;
            if (containersInLayout > 1)
            {
                _layoutWidth += (containersInLayout - 1) * _deviceSpacing;
            }

            _layoutOrigin = new Vector2(-_layoutWidth / 2f, -_layoutHeight / 2f);

            UpdateCamera(mode);
        }

        private float _layoutWidth = 0f;
        private float _layoutHeight = 0f;
        private Vector2 _layoutOrigin = Vector2.Zero;

        private enum UpdateDeviceContainerAnimMode { Normal, Enter, Exit, None };
        private void UpdateDeviceContainers(UpdateDeviceContainerAnimMode mode)
        {
            int index = 0;
            for (int i = 0; i < _containers.Count; i++)
            {
                if (_containers[i].State != ContainerState.Exited && _containers[i].State != ContainerState.Exiting)
                {
                    float containerOffsetY = _layoutHeight - _containers[i].DeviceElement.LayoutSize.Y + _layoutOrigin.Y;
                    _containers[i].OffsetDestination = new Vector3(_layoutOrigin.X + index * (_deviceSpacing + _deviceWidth), containerOffsetY, 0f);
                    index++;
                    if (_disableAnimations || mode == UpdateDeviceContainerAnimMode.None || SnapOffsets(_containers[i].OffsetDestination, _containers[i].DeviceVisual.Offset))
                    {
                        _containers[i].DeviceVisual.Offset = _containers[i].OffsetDestination;
                    }
                    else
                    {
                        var offsetAnim = Window.Current.Compositor.CreateVector3KeyFrameAnimation();
                        switch (mode)
                        {
                            case UpdateDeviceContainerAnimMode.Enter:
                                offsetAnim.InsertKeyFrame(1f, _containers[i].OffsetDestination, _introAnimEasing);
                                offsetAnim.Duration = _introAnimDuration;
                                break;
                            case UpdateDeviceContainerAnimMode.Exit:
                                offsetAnim.InsertKeyFrame(1f, _containers[i].OffsetDestination, _outroAnimEasing);
                                offsetAnim.Duration = _outroAnimOffsetDuration;
                                break;
                            default:
                                offsetAnim.InsertKeyFrame(1f, _containers[i].OffsetDestination, _offsetEasing);
                                offsetAnim.Duration = _offsetAnimDuration;
                                break;
                        }
                        _containers[i].DeviceVisual.StartAnimation("Offset", offsetAnim);
                    }
                }
            }
        }

        private void UpdateCamera(UpdateDeviceContainerAnimMode mode)
        {
            if (_disableAnimations)
            {
                _camera.AnimPosition = false;
            }
            else
            {
                switch (mode)
                {
                    case UpdateDeviceContainerAnimMode.None:
                        _camera.AnimPosition = false;
                        break;
                    case UpdateDeviceContainerAnimMode.Enter:
                        _camera.AnimPosition = true;
                        _camera.PositionAnimEasing = _introAnimEasing;
                        _camera.PositionAnimDuration = _introAnimDuration;
                        break;
                    case UpdateDeviceContainerAnimMode.Exit:
                        _camera.AnimPosition = true;
                        _camera.PositionAnimEasing = _outroAnimEasing;
                        _camera.PositionAnimDuration = _outroAnimOffsetDuration;
                        break;
                    default:
                        _camera.AnimPosition = true;
                        _camera.PositionAnimEasing = _offsetEasing;
                        _camera.PositionAnimDuration = _offsetAnimDuration;
                        break;
                }
            }

            var ratio = Math.Max(_layoutWidth / _viewWidth, _layoutHeight / _viewHeight);

            _camera.Position = new Vector3(0f, 0f, ratio * _perspectiveDistance);
            _camera.ViewportSize = new Vector2(_viewWidth, _viewHeight);
        }

        private bool SnapOffsets(Vector3 a, Vector3 b)
        {
            return (Math.Abs(a.X - b.X) <= _offsetAnimSnap &&
                    Math.Abs(a.Y - b.Y) <= _offsetAnimSnap &&
                    Math.Abs(a.Z - b.Z) <= _offsetAnimSnap);
        }

        private void Element_LayoutSizeUpdated(LampControl source, Vector2 layoutSize)
        {
            UpdateLayoutBounds(UpdateDeviceContainerAnimMode.Normal);
            UpdateDeviceContainers(UpdateDeviceContainerAnimMode.Normal);
        }
    }
}
