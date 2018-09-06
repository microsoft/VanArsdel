// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml.Hosting;
using System.Numerics;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;

namespace VanArsdel
{
    public sealed partial class MainPage : Page, IExitTransition
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region ViewModelProperty
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainPage), new PropertyMetadata(null, new PropertyChangedCallback(OnViewModelPropertyChanged)));

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainPage target)
            {
                target.OnViewModelChanged(e.OldValue as MainViewModel, e.NewValue as MainViewModel);
            }
        }

        private void OnViewModelChanged(MainViewModel oldValue, MainViewModel newValue)
        {
            UnhookViewModel(oldValue);
            HookViewModel(newValue);
        }

        public MainViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as MainViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        private void HookViewModel(MainViewModel vm)
        {
            if (vm != null)
            {
                vm.SelectedDevicesChanged -= ViewModel_SelectedDevicesChanged;
                vm.SelectedDevicesChanged += ViewModel_SelectedDevicesChanged;
            }
        }

        private void UnhookViewModel(MainViewModel vm)
        {
            if (vm != null)
            {
                vm.SelectedDevicesChanged -= ViewModel_SelectedDevicesChanged;
            }
        }

        #endregion

        private void PlayEntranceTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.2f, 1f));
            var duration = TimeSpan.FromMilliseconds(300);

            var propertiesVisual = ElementCompositionPreview.GetElementVisual(Properties);
            var navVisual = ElementCompositionPreview.GetElementVisual(NavPaneContainer);
            var deviceVisual = ElementCompositionPreview.GetElementVisual(DeviceLayout);
            var dayNightVisual = ElementCompositionPreview.GetElementVisual(DayNightSelectionPanel);

            // Initial conditions
            propertiesVisual.Opacity = 0f;
            propertiesVisual.Offset = new Vector3(344f, 0f, 0f);

            navVisual.Opacity = 0f;
            navVisual.Offset = new Vector3(-320f, 0f, 0f);

            deviceVisual.Opacity = 0f;

            dayNightVisual.Opacity = 0f;

            // Set up animations
            var propertiesVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            propertiesVisualOpacityAnim.Target = "Opacity";
            propertiesVisualOpacityAnim.Duration = duration;
            propertiesVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var propertiesVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            propertiesVisualOffsetAnim.Target = "Offset";
            propertiesVisualOffsetAnim.Duration = duration;
            propertiesVisualOffsetAnim.InsertKeyFrame(1f, Vector3.Zero, easingFunction);

            var navVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            navVisualOpacityAnim.Target = "Opacity";
            navVisualOpacityAnim.Duration = duration;
            navVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var navVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            navVisualOffsetAnim.Target = "Offset";
            navVisualOffsetAnim.Duration = duration;
            navVisualOffsetAnim.InsertKeyFrame(1f, Vector3.Zero, easingFunction);

            var deviceVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            deviceVisualOpacityAnim.Target = "Opacity";
            deviceVisualOpacityAnim.Duration = duration;
            deviceVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var dayNightVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            dayNightVisualOpacityAnim.Target = "Opacity";
            dayNightVisualOpacityAnim.Duration = duration;
            dayNightVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            // Start animations
            propertiesVisual.StartAnimation(propertiesVisualOpacityAnim.Target, propertiesVisualOpacityAnim);
            propertiesVisual.StartAnimation(propertiesVisualOffsetAnim.Target, propertiesVisualOffsetAnim);

            navVisual.StartAnimation(navVisualOpacityAnim.Target, navVisualOpacityAnim);
            navVisual.StartAnimation(navVisualOffsetAnim.Target, navVisualOffsetAnim);

            deviceVisual.StartAnimation(deviceVisualOpacityAnim.Target, deviceVisualOpacityAnim);
            dayNightVisual.StartAnimation(dayNightVisualOpacityAnim.Target, dayNightVisualOpacityAnim);
        }

        public Task PlayExitTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.7f, 0.0f), new Vector2(1.0f, 0.5f));
            var duration = TimeSpan.FromMilliseconds(300);

            var propertiesVisual = ElementCompositionPreview.GetElementVisual(Properties);
            var navVisual = ElementCompositionPreview.GetElementVisual(NavPaneContainer);
            var deviceVisual = ElementCompositionPreview.GetElementVisual(DeviceLayout);
            var dayNightVisual = ElementCompositionPreview.GetElementVisual(DayNightSelectionPanel);

            // Set up animations
            var propertiesVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            propertiesVisualOpacityAnim.Target = "Opacity";
            propertiesVisualOpacityAnim.Duration = duration;
            propertiesVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var propertiesVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            propertiesVisualOffsetAnim.Target = "Offset";
            propertiesVisualOffsetAnim.Duration = duration;
            propertiesVisualOffsetAnim.InsertKeyFrame(1f, new Vector3(344f, 0f, 0f), easingFunction);

            var navVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            navVisualOpacityAnim.Target = "Opacity";
            navVisualOpacityAnim.Duration = duration;
            navVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var navVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            navVisualOffsetAnim.Target = "Offset";
            navVisualOffsetAnim.Duration = duration;
            navVisualOffsetAnim.InsertKeyFrame(1f, new Vector3(-320f, 0f, 0f), easingFunction);

            var deviceVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            deviceVisualOpacityAnim.Target = "Opacity";
            deviceVisualOpacityAnim.Duration = duration;
            deviceVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var dayNightVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            dayNightVisualOpacityAnim.Target = "Opacity";
            dayNightVisualOpacityAnim.Duration = duration;
            dayNightVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            // Start animations in scoped batch

            var scopedBatch = compositor.CreateScopedBatch(Windows.UI.Composition.CompositionBatchTypes.Animation);

            propertiesVisual.StartAnimation(propertiesVisualOpacityAnim.Target, propertiesVisualOpacityAnim);
            propertiesVisual.StartAnimation(propertiesVisualOffsetAnim.Target, propertiesVisualOffsetAnim);

            navVisual.StartAnimation(navVisualOpacityAnim.Target, navVisualOpacityAnim);
            navVisual.StartAnimation(navVisualOffsetAnim.Target, navVisualOffsetAnim);

            deviceVisual.StartAnimation(deviceVisualOpacityAnim.Target, deviceVisualOpacityAnim);
            dayNightVisual.StartAnimation(dayNightVisualOpacityAnim.Target, dayNightVisualOpacityAnim);

            scopedBatch.End();

            // Set up task completion

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            TypedEventHandler<object, CompositionBatchCompletedEventArgs> completedHandler = null;
            completedHandler = new TypedEventHandler<object, CompositionBatchCompletedEventArgs>((sender, args) =>
            {
                scopedBatch.Completed -= completedHandler;
                tcs.SetResult(null);
            });
            scopedBatch.Completed += completedHandler;

            return tcs.Task;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = e.Parameter as MainViewModel;
            ViewModel = vm;

            // Select first device by default if there no selected devices
            if ((ViewModel?.SelectedDevices?.Count ?? 0) == 0)
            {
                var defaultSelection = ViewModel?.GroupedDevices.FirstOrDefault()?.FirstOrDefault();

                if (defaultSelection != null)
                {
                    ViewModel.SelectDevices(new[] { defaultSelection });
                }
            }

            UpdateSelectionFromVM();

            DeviceList.SelectionChanged -= DeviceList_SelectionChanged;
            DeviceList.SelectionChanged += DeviceList_SelectionChanged;

            PlayEntranceTransition();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UpdateSelectionFromVM();

            base.OnNavigatedFrom(e);
        }

        private void DeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_deviceListSelectionInProgress)
            {
                return;
            }

            var vm = ViewModel;
            if (vm == null)
            {
                return;
            }

            var newSelected = DeviceList.SelectedItems;
            if (newSelected == null || newSelected.Count == 0)
            {
                if (vm.SelectedDevices == null || vm.SelectedDevices.Count == 0)
                {
                    vm.SelectDevices(null);
                }
                else
                {
                    UpdateSelectionFromVM();
                }
            }
            else
            {
                _deviceListSelectionInProgress = true;
                vm.SelectDevices(DeviceList.SelectedItems);
                _deviceListSelectionInProgress = false;
            }
        }

        private bool _deviceListSelectionInProgress = false;
        private void ViewModel_SelectedDevicesChanged(MainViewModel vm, IReadOnlyList<Devices.Device> selected)
        {
            UpdateSelectionFromVM();
        }

        private void DeviceList_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSelectionFromVM();
        }

        private void UpdateSelectionFromVM()
        {
            var vm = ViewModel as MainViewModel;
            if (vm != null && vm.SelectedDevices != null)
            {
                _deviceListSelectionInProgress = true;
                DeviceList.SelectedItems.Clear();
                foreach (var device in vm.SelectedDevices)
                {
                    DeviceList.SelectedItems.Add(device);
                }
                _deviceListSelectionInProgress = false;
            }
            else
            {
                DeviceList.SelectedItems.Clear();
            }
        }

        private bool _isNavPaneOpen = true;
        private void PaneButton_Click(object sender, RoutedEventArgs e)
        {
            _isNavPaneOpen = !_isNavPaneOpen;
            NavPane.Visibility = _isNavPaneOpen ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewRoom(sender as FrameworkElement);
        }

        private void DayRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DeviceLayout.DayMode = true;
            DayNightSelectionPanel.RequestedTheme = ElementTheme.Light;
        }

        private void NightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DeviceLayout.DayMode = false;
            DayNightSelectionPanel.RequestedTheme = ElementTheme.Dark;
        }
    }
}