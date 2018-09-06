// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using VanArsdel.Utils;
using System;
using System.Numerics;
using VanArsdel.Devices;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition.Effects;

namespace VanArsdel
{
    public sealed partial class LampControl : UserControl
    {
        private static readonly Vector2 _anchorPoint = new Vector2(0f);
        private static readonly CompositionBorderMode _borderMode = CompositionBorderMode.Hard;

        public LampControl()
        {
            this.InitializeComponent();
        }

        #region DeviceProperty

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register("Device", typeof(Device), typeof(LampControl), new PropertyMetadata(null, OnDevicePropertyChanged));

        private static void OnDevicePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LampControl target)
            {
                target.OnDeviceChanged(e.OldValue as Device, e.NewValue as Device);
            }
        }

        private void OnDeviceChanged(Device oldValue, Device newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyValueChanged -= Device_PropertyChanged;
            }

            if (newValue != null)
            {
                newValue.PropertyValueChanged -= Device_PropertyChanged;
                newValue.PropertyValueChanged += Device_PropertyChanged;
            }

            LoadDeviceInfo(newValue);
        }

        public Device Device
        {
            get { return GetValue(DeviceProperty) as Device; }
            set { SetValue(DeviceProperty, value); }
        }

        #endregion

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var w = ActualWidth;
            var h = ActualHeight;
            if (double.IsNaN(w) || w <= 0 || double.IsNaN(h) || h <= 0)
            {
                return;
            }

            InitializeComp();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DisposeComp();
        }

        public void InitializeComp()
        {
            if (_isCompInitialized)
            {
                return;
            }

            _assetsLoaded = false;

            _engravingVisual = ElementCompositionPreview.GetElementVisual(EngravingContainer);

            var compositor = Window.Current.Compositor;

            _containerVisual = compositor.CreateContainerVisual();
            _containerVisual.Comment = "Lamp Container";

            _middleVisual = compositor.CreateSpriteVisual();
            _middleVisual.AnchorPoint = _anchorPoint;
            _middleVisual.Offset = Vector3.Zero;
            _middleVisual.Comment = "Lamp Middle";
            _middleVisual.BorderMode = _borderMode;
            _containerVisual.Children.InsertAtTop(_middleVisual);

            _topVisual = compositor.CreateSpriteVisual();
            _topVisual.AnchorPoint = _anchorPoint;
            _topVisual.Offset = Vector3.Zero;
            _topVisual.Comment = "Lamp Top";
            _topVisual.BorderMode = _borderMode;
            _containerVisual.Children.InsertAtTop(_topVisual);

            _bottomVisual = compositor.CreateSpriteVisual();
            _bottomVisual.AnchorPoint = _anchorPoint;
            _bottomVisual.Offset = Vector3.Zero;
            _bottomVisual.Comment = "Lamp Bottom";
            _bottomVisual.BorderMode = _borderMode;
            _containerVisual.Children.InsertAtTop(_bottomVisual);

            _haloVisual = compositor.CreateSpriteVisual();
            _haloVisual.AnchorPoint = _anchorPoint;
            _haloVisual.Offset = Vector3.Zero;
            _haloVisual.Comment = "Lamp Halo";
            _haloVisual.BorderMode = _borderMode;
            _haloVisual.Opacity = 0;
            _containerVisual.Children.InsertAtTop(_haloVisual);

            _ambientLight = compositor.CreateAmbientLight();
            _ambientLight.Color = Colors.Gainsboro;

            _spotLight = compositor.CreateSpotLight();
            _spotLight.CoordinateSpace = _containerVisual;
            _spotLight.InnerConeColor = Colors.WhiteSmoke;
            _spotLight.InnerConeAngleInDegrees = 0.0f;
            _spotLight.OuterConeColor = Colors.Yellow;
            _spotLight.OuterConeAngleInDegrees = 40.0f;
            _spotLight.Comment = "Spotlight Lamp";

            _spotLight1 = compositor.CreateSpotLight();

            _pointLight = compositor.CreatePointLight();
            _pointLight.CoordinateSpace = _containerVisual;
            _pointLight.Comment = "Pointlight";

            TimeSpan colorAnimDuration = TimeSpan.FromMilliseconds(300);

            var spotlightAnimationCollection = compositor.CreateImplicitAnimationCollection();
            var spotlightInnerColorAnim = compositor.CreateColorKeyFrameAnimation();
            spotlightInnerColorAnim.Target = "InnerConeColor";
            spotlightInnerColorAnim.InsertExpressionKeyFrame(1f, "this.FinalValue");
            spotlightInnerColorAnim.Duration = colorAnimDuration;
            spotlightAnimationCollection[spotlightInnerColorAnim.Target] = spotlightInnerColorAnim;
            var spotlightOuterColorAnim = compositor.CreateColorKeyFrameAnimation();
            spotlightOuterColorAnim.Target = "OuterConeColor";
            spotlightOuterColorAnim.InsertExpressionKeyFrame(1f, "this.FinalValue");
            spotlightOuterColorAnim.Duration = colorAnimDuration;
            spotlightAnimationCollection[spotlightOuterColorAnim.Target] = spotlightOuterColorAnim;
            var pointlightAnimationCollection = compositor.CreateImplicitAnimationCollection();
            var pointlightColorAnim = compositor.CreateColorKeyFrameAnimation();
            pointlightColorAnim.Target = "Color";
            pointlightColorAnim.InsertExpressionKeyFrame(1f, "this.FinalValue");
            pointlightColorAnim.Duration = colorAnimDuration;
            pointlightAnimationCollection[pointlightColorAnim.Target] = pointlightColorAnim;

            _spotLight1.ImplicitAnimations = spotlightAnimationCollection;
            _spotLight.ImplicitAnimations = spotlightAnimationCollection;
            _pointLight.ImplicitAnimations = pointlightAnimationCollection;

            ElementCompositionPreview.SetElementChildVisual(CompElement, _containerVisual);

            _isCompInitialized = true;

            LoadDeviceInfo(Device);
        }

        private void DisposeComp()
        {
            _isCompInitialized = false;

            _engravingVisual = null;

            if (_ambientLight != null)
            {
                _ambientLight.Dispose();
                _ambientLight = null;
            }
            if (_spotLight != null)
            {
                _spotLight.Dispose();
                _spotLight = null;
            }
            if (_spotLight1 != null)
            {
                _spotLight1.Dispose();
                _spotLight1 = null;
            }
            if (_pointLight != null)
            {
                _pointLight.Dispose();
                _pointLight = null;
            }
            if (_topVisual != null)
            {
                _topVisual.Dispose();
                _topVisual = null;
            }
            if (_middleVisual != null)
            {
                _middleVisual.Dispose();
                _middleVisual = null;
            }
            if (_bottomVisual != null)
            {
                _bottomVisual.Dispose();
                _bottomVisual = null;
            }
            if (_haloVisual != null)
            {
                _haloVisual.Dispose();
                _haloVisual = null;
            }
            if (_containerVisual != null)
            {
                _containerVisual.Dispose();
                _containerVisual = null;
            }
            if (_topColorImage != null)
            {
                _topColorImage.Dispose();
                _topColorImage = null;
            }
            if (_topNormalImage != null)
            {
                _topNormalImage.Dispose();
                _topNormalImage = null;
            }
            if (_middleColorImage != null)
            {
                _middleColorImage.Dispose();
                _middleColorImage = null;
            }
            if (_middleNormalImage != null)
            {
                _middleNormalImage.Dispose();
                _middleNormalImage = null;
            }
            if (_bottomColorImage != null)
            {
                _bottomColorImage.Dispose();
                _bottomColorImage = null;
            }
            if (_bottomNormalImage != null)
            {
                _bottomNormalImage.Dispose();
                _bottomNormalImage = null;
            }
            if (_haloColorImage != null)
            {
                _haloColorImage.Dispose();
                _haloColorImage = null;
            }
        }

        private bool _isCompInitialized = false;
        private ContainerVisual _containerVisual;
        private SpriteVisual _topVisual;
        private SpriteVisual _middleVisual;
        private SpriteVisual _bottomVisual;
        private SpriteVisual _haloVisual;

        private float _haloOffset = 0f;

        private LoadedImageSurface _topColorImage;
        private LoadedImageSurface _topNormalImage;
        private LoadedImageSurface _middleColorImage;
        private LoadedImageSurface _middleNormalImage;
        private LoadedImageSurface _bottomColorImage;
        private LoadedImageSurface _bottomNormalImage;
        private LoadedImageSurface _haloColorImage;

        private AmbientLight _ambientLight;
        private SpotLight _spotLight;
        private SpotLight _spotLight1;
        private PointLight _pointLight;

        private bool _showEngraving = false;
        private float _engravingOffset = 0f;
        private string _engravingText = null;
        private Visual _engravingVisual;

        private void LoadDeviceInfo(Device device)
        {
            if (_isCompInitialized && device != null)
            {
                var lightsOnProperty = device.GetPropertyById<IPropertyBool>("LightsOn");
                var brightnessProperty = device.GetPropertyById<IPropertyNumber>("Brightness");
                var bulbColorProperty = device.GetPropertyById<IPropertyColorPalette>("LightBulbColor");
                var topBitmapProperty = device.GetPropertyById<IPropertyBitmapPicker>("TopBitmap");
                var middleBitmapProperty = device.GetPropertyById<IPropertyBitmapPicker>("MiddleBitmap");
                var bottomBitmapProperty = device.GetPropertyById<IPropertyBitmapPicker>("BottomBitmap");
                var haloBitmapProperty = device.GetPropertyById<IPropertyBitmapPicker>("HaloBitmap");
                var engravingTextProperty = device.GetPropertyById<IPropertyString>("Engraving");

                if (middleBitmapProperty.SelectedItem != null && middleBitmapProperty.SelectedItem.Metadata.HasProperty("EngraveOffset"))
                {
                    _showEngraving = true;
                    _engravingOffset = middleBitmapProperty.SelectedItem.Metadata.GetFloat("EngraveOffset");
                }
                else
                {
                    _showEngraving = false;
                    _engravingOffset = 0f;
                }
                UpdateEngraving();

                SetLightsOn(lightsOnProperty.Value);
                SetBrightness(brightnessProperty.Value);
                SetBulbColor(bulbColorProperty.SelectedColor);
                SetEngravingText(engravingTextProperty.RawText);

                SetAllBitmaps(topBitmapProperty.SelectedItem?.AssetPath, topBitmapProperty.SelectedItem?.AssetSize, topBitmapProperty.SelectedItem?.Metadata.GetFloat("HaloOffset"), topBitmapProperty.SelectedItem?.Metadata.GetSize("HaloSize"), middleBitmapProperty.SelectedItem?.AssetPath, middleBitmapProperty.SelectedItem?.AssetSize, bottomBitmapProperty.SelectedItem?.AssetPath, bottomBitmapProperty.SelectedItem?.AssetSize);
            }
        }

        private void Device_PropertyChanged(Device device, IProperty property)
        {
            switch (property.Id)
            {
                case "LightsOn":
                    if (property is IPropertyBool b)
                    {
                        SetLightsOn(b.Value);
                    }
                    break;
                case "Brightness":
                    if (property is IPropertyNumber n)
                    {
                        SetBrightness(n.Value);
                    }
                    break;
                case "LightBulbColor":
                    if (property is IPropertyColorPalette c)
                    {
                        SetBulbColor(c.SelectedColor);
                    }
                    break;
                case "TopBitmap":
                    if (property is IPropertyBitmapPicker tbp)
                    {
                        SetAllBitmaps(tbp.SelectedItem?.AssetPath, tbp.SelectedItem?.AssetSize, tbp.SelectedItem?.Metadata.GetFloat("HaloOffset"), tbp.SelectedItem?.Metadata.GetSize("HaloSize"), null, null, null, null);
                    }
                    break;
                case "MiddleBitmap":
                    if (property is IPropertyBitmapPicker mbp)
                    {
                        if (mbp.SelectedItem.Metadata.HasProperty("EngraveOffset"))
                        {
                            _showEngraving = true;
                            _engravingOffset = mbp.SelectedItem.Metadata.GetFloat("EngraveOffset");
                        }
                        else
                        {
                            _showEngraving = false;
                            _engravingOffset = 0f;
                        }
                        UpdateEngraving();

                        SetAllBitmaps(null, null, null, null, mbp.SelectedItem?.AssetPath, mbp.SelectedItem?.AssetSize, null, null);
                    }
                    break;
                case "BottomBitmap":
                    if (property is IPropertyBitmapPicker bbp)
                    {
                        SetAllBitmaps(null, null, null, null, null, null, bbp.SelectedItem?.AssetPath, bbp.SelectedItem?.AssetSize);
                    }
                    break;
                case "Engraving":
                    if (property is IPropertyString s)
                    {
                        SetEngravingText(s.RawText);
                    }
                    break;
            }
        }

        private void SetLightsOn(bool value)
        {
            if (_isCompInitialized)
            {
                if (value)
                {
                    // TOP
                    _ambientLight.Targets.Add(_topVisual);
                    _spotLight.Targets.Add(_topVisual);

                    // MIDDLE
                    _ambientLight.Targets.Add(_middleVisual);
                    _spotLight.Targets.Add(_middleVisual);
                    _pointLight.Targets.Add(_middleVisual);

                    // BOTTOM
                    _ambientLight.Targets.Add(_bottomVisual);
                    _spotLight.Targets.Add(_bottomVisual);
                    _pointLight.Targets.Add(_bottomVisual);

                    // HALO
                    _ambientLight.Targets.Add(_haloVisual);
                    _spotLight.Targets.Add(_haloVisual);
                    _pointLight.Targets.Add(_haloVisual);
                    _haloVisual.IsVisible = true;
                }
                else
                {
                    _haloVisual.IsVisible = false;
                    _ambientLight.Targets.RemoveAll();
                    _spotLight.Targets.RemoveAll();
                    _spotLight1.Targets.RemoveAll();
                    _pointLight.Targets.RemoveAll();
                }
            }
        }

        private void SetBrightness(double value)
        {
            var brightness = (float)value;
            if (_isCompInitialized)
            {
                _pointLight.Offset = new Vector3(_pointLight.Offset.X, -brightness, 6000 - brightness * 4);
                _pointLight.Intensity = brightness / 200;
                _spotLight.Offset = new Vector3(_spotLight.Offset.X, _spotLight.Offset.Y, brightness * 2);
                _spotLight.InnerConeIntensity = brightness / 300;
                _haloVisual.Opacity = brightness / 8000;
            }
        }

        private void SetBulbColor(Color value)
        {
            if (_isCompInitialized)
            {
                _pointLight.Color = value;
                _spotLight.OuterConeColor = value;
                _spotLight.InnerConeColor = value;
                _spotLight1.InnerConeColor = value;
            }
        }

        private void SetAllBitmaps(string topPath, Size? topSize, float? haloOffset, Size? haloSize, string middlePath, Size? middleSize, string bottomPath, Size? bottomSize)
        {
            if (!_isCompInitialized)
            {
                return;
            }

            LoadedImageSurface topSurface = null;
            LoadedImageSurface topNormalSurface = null;
            LoadedImageSurface haloSurface = null;
            LoadedImageSurface middleSurface = null;
            LoadedImageSurface middleNormalSurface = null;
            LoadedImageSurface bottomSurface = null;
            LoadedImageSurface bottomNormalSurface = null;

            if (topPath != null)
            {
                string topNormalPath = topPath.Replace("_Color.png", "_Normal.png");
                string topHaloPath = topPath.Replace("_Color.png", "_Halo.png");

                if (topSize.HasValue)
                {
                    _topVisual.Size = topSize.Value.ToVector2();
                    topSurface = LoadedImageSurface.StartLoadFromUri(new Uri(topPath), topSize.Value);
                    topNormalSurface = LoadedImageSurface.StartLoadFromUri(new Uri(topNormalPath), topSize.Value);
                }
                else
                {
                    topSurface = LoadedImageSurface.StartLoadFromUri(new Uri(topPath));
                    topNormalSurface = LoadedImageSurface.StartLoadFromUri(new Uri(topNormalPath));
                }
                if (haloSize.HasValue)
                {
                    _haloVisual.Size = haloSize.Value.ToVector2();
                    haloSurface = LoadedImageSurface.StartLoadFromUri(new Uri(topHaloPath), haloSize.Value);
                }
                else
                {
                    haloSurface = LoadedImageSurface.StartLoadFromUri(new Uri(topHaloPath));
                }

                if (!_isCompInitialized)
                {
                    return;
                }
            }

            if (middlePath != null)
            {
                string middleNormalPath = middlePath.Replace("_Color.png", "_Normal.png");

                if (middleSize.HasValue)
                {
                    _middleVisual.Size = middleSize.Value.ToVector2();
                    middleSurface = LoadedImageSurface.StartLoadFromUri(new Uri(middlePath), middleSize.Value);
                    middleNormalSurface = LoadedImageSurface.StartLoadFromUri(new Uri(middleNormalPath), middleSize.Value);
                }
                else
                {
                    middleSurface = LoadedImageSurface.StartLoadFromUri(new Uri(middlePath));
                    middleNormalSurface = LoadedImageSurface.StartLoadFromUri(new Uri(middleNormalPath));
                }

                if (!_isCompInitialized)
                {
                    return;
                }
            }

            if (bottomPath != null)
            {
                string bottomNormalPath = bottomPath.Replace("_Color.png", "_Normal.png");

                if (bottomSize.HasValue)
                {
                    _bottomVisual.Size = bottomSize.Value.ToVector2();
                    bottomSurface = LoadedImageSurface.StartLoadFromUri(new Uri(bottomPath), bottomSize.Value);
                    bottomNormalSurface = LoadedImageSurface.StartLoadFromUri(new Uri(bottomNormalPath), bottomSize.Value);
                }
                else
                {
                    bottomSurface = LoadedImageSurface.StartLoadFromUri(new Uri(bottomPath));
                    bottomNormalSurface = LoadedImageSurface.StartLoadFromUri(new Uri(bottomNormalPath));
                }

                if (!_isCompInitialized)
                {
                    return;
                }
            }

            var compositor = Window.Current.Compositor;
            if (topSurface != null && topNormalSurface != null)
            {
                _topColorImage = topSurface;
                _topNormalImage = topNormalSurface;
                _topVisual.Brush = CreateNormalMapBrush(compositor, _topNormalImage, _topColorImage);
            }

            if (haloSurface != null)
            {
                _haloColorImage = haloSurface;
                // The halo color image being its own normal image is deliberate
                _haloVisual.Brush = CreateNormalMapBrush(compositor, _haloColorImage, _haloColorImage);
            }

            if (middleSurface != null && middleNormalSurface != null)
            {
                _middleColorImage = middleSurface;
                _middleNormalImage = middleNormalSurface;
                _middleVisual.Brush = CreateNormalMapBrush(compositor, _middleNormalImage, _middleColorImage);
            }

            if (bottomSurface != null && bottomNormalSurface != null)
            {
                _bottomColorImage = bottomSurface;
                _bottomNormalImage = bottomNormalSurface;
                _bottomVisual.Brush = CreateNormalMapBrush(compositor, _bottomNormalImage, _bottomColorImage);
            }

            if (haloOffset.HasValue)
            {
                _haloOffset = haloOffset.Value;
            }
            LayoutVisuals(_assetsLoaded);
        }

        private class ImageWrapper
        {
            public ImageWrapper() { }

            public ImageWrapper(CoreDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            private CoreDispatcher _dispatcher;
            private object _lock = new object();
            private bool _loaded = false;
            private LoadedImageSurface _surface;

            public void LoadImage(Uri uri, Size? size = null)
            {
                lock (_lock)
                {
                    if (_surface != null)
                    {
                        _surface.LoadCompleted -= _surface_LoadCompleted;
                        _surface.Dispose();
                        _surface = null;
                    }

                    _loaded = false;
                    if (size.HasValue)
                    {
                        _surface = LoadedImageSurface.StartLoadFromUri(uri, size.Value);
                    }
                    else
                    {
                        _surface = LoadedImageSurface.StartLoadFromUri(uri);
                    }
                    _surface.LoadCompleted += _surface_LoadCompleted;
                }
            }

            private void _surface_LoadCompleted(LoadedImageSurface sender, LoadedImageSourceLoadCompletedEventArgs args)
            {
                lock (_lock)
                {
                    if (sender != _surface)
                    {
                        return;
                    }
                    _surface.LoadCompleted -= _surface_LoadCompleted;
                    if (args.Status == LoadedImageSourceLoadStatus.Success)
                    {
                        _loaded = true;
                    }
                    else
                    {
                        _loaded = false;
                        _surface.Dispose();
                        _surface = null;
                    }
                }
            }

            private CompositionBrush CreateBrush(Compositor compositor)
            {
                lock (_lock)
                {
                    if (!_loaded || _surface == null)
                    {
                        return null;
                    }
                    return compositor.CreateSurfaceBrush(_surface);
                }
            }
        }

        private Task<LoadedImageSurface> LoadImage(Uri uri, Size? size, CoreDispatcher dispatcher = null)
        {
            TaskCompletionSource<LoadedImageSurface> completionSource = new TaskCompletionSource<LoadedImageSurface>();
            LoadedImageSurface result = null;

            if (size.HasValue)
            {
                result = LoadedImageSurface.StartLoadFromUri(uri, size.Value);
            }
            else
            {
                result = LoadedImageSurface.StartLoadFromUri(uri);
            }

            TypedEventHandler<LoadedImageSurface, LoadedImageSourceLoadCompletedEventArgs> loadCompleteHandler = null;
            loadCompleteHandler = new TypedEventHandler<LoadedImageSurface, LoadedImageSourceLoadCompletedEventArgs>((sender, args) =>
            {
                sender.LoadCompleted -= loadCompleteHandler;
                if (dispatcher != null)
                {
                    _ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                     {
                         if (args.Status == LoadedImageSourceLoadStatus.Success)
                         {
                             completionSource.SetResult(result);
                         }
                         else
                         {
                             completionSource.SetResult(null);
                         }
                     });
                }
                else
                {
                    if (args.Status == LoadedImageSourceLoadStatus.Success)
                    {
                        completionSource.SetResult(result);
                    }
                    else
                    {
                        completionSource.SetResult(null);
                    }
                }
            });

            result.LoadCompleted += loadCompleteHandler;

            return completionSource.Task;
        }

        private void SetEngravingText(string engravingText)
        {
            _engravingText = engravingText != null ? engravingText : string.Empty;
            EngraveText1.Text = _engravingText;
            EngraveText2.Text = _engravingText;
            UpdateEngraving();
        }

        private void LayoutVisuals(bool broadcastLayoutUpdated)
        {
            if (!CheckForAssetsLoaded())
            {
                return;
            }

            float totalWidth = 0f;
            float totalHeight = 0f;
            float middleOffset = 0f;
            float bottomOffset = 0f;

            if (_topColorImage != null)
            {
                totalWidth = Math.Max(totalWidth, _topVisual.Size.X);
                totalHeight += _topVisual.Size.Y;

                middleOffset += _topVisual.Size.Y;
                bottomOffset += _topVisual.Size.Y;
            }
            if (_middleColorImage != null)
            {
                totalWidth = Math.Max(totalWidth, _middleVisual.Size.X);
                totalHeight += _middleVisual.Size.Y;

                bottomOffset += _middleVisual.Size.Y;
            }
            if (_bottomColorImage != null)
            {
                totalWidth = Math.Max(totalWidth, _bottomVisual.Size.X);
                totalHeight += _bottomVisual.Size.Y;
            }

            _topVisual.Offset = new Vector3((totalWidth - _topVisual.Size.X) / 2f, 0f, 0f);

            _haloVisual.Offset = new Vector3((totalWidth - _haloVisual.Size.X) / 2f, _haloOffset, 0f);
            _middleVisual.Offset = new Vector3((totalWidth - _middleVisual.Size.X) / 2f, middleOffset, 0f);
            _bottomVisual.Offset = new Vector3((totalWidth - _bottomVisual.Size.X) / 2f, bottomOffset, 0f);

            ContainerElement.Width = totalWidth;
            ContainerElement.Height = totalHeight;

            _spotLight.Offset = new Vector3(totalWidth / 2f, totalHeight / 2f, _spotLight.Offset.Z);
            _spotLight1.Offset = new Vector3(totalWidth / 2f, totalHeight / 2f, _spotLight1.Offset.Z);
            _pointLight.Offset = new Vector3(totalWidth / 2f, totalHeight / 2f, _pointLight.Offset.Z);

            var device = Device;
            if (device != null)
            {
                _containerVisual.Offset = device.Category.DeviceOffset;
                _spotLight.Offset += device.Category.SpotlightOffset;
                _spotLight1.Offset += device.Category.Spotlight1Offset;
                _pointLight.Offset += device.Category.PointlightOffset;
            }
            else
            {
                _containerVisual.Offset = Vector3.Zero;
            }

            UpdateEngraving();

            if (_layoutSize.X != totalWidth || _layoutSize.Y != totalHeight)
            {
                _layoutSize = new Vector2(totalWidth, totalHeight);
                if (broadcastLayoutUpdated)
                {
                    LayoutSizeUpdated?.Invoke(this, _layoutSize);
                }
            }
        }

        private Vector2 _layoutSize = Vector2.Zero;
        public Vector2 LayoutSize
        {
            get { return _layoutSize; }
        }

        public event Action<LampControl, Vector2> LayoutSizeUpdated;

        private bool _assetsLoaded = false;
        public bool AssetsLoaded
        {
            get { return _assetsLoaded; }
        }

        public event Action<LampControl> AssetLoadComplete;

        private bool CheckForAssetsLoaded()
        {
            if (_assetsLoaded)
            {
                return true;
            }
            if (!_isCompInitialized)
            {
                return false;
            }
            if (_topColorImage != null &&
                _topNormalImage != null &&
                _middleColorImage != null &&
                _middleNormalImage != null &&
                _bottomColorImage != null &&
                _bottomNormalImage != null &&
                _haloColorImage != null)
            {
                _assetsLoaded = true;
                LayoutVisuals(false);
                AssetLoadComplete?.Invoke(this);
                return true;
            }
            return false;
        }

        private void EngravingContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateEngraving();
        }

        private void UpdateEngraving()
        {
            if (_isCompInitialized && _middleVisual != null && _showEngraving && !string.IsNullOrEmpty(_engravingText))
            {
                EngravingContainer.Visibility = Visibility.Visible;
                _engravingVisual.Offset = _containerVisual.Offset + new Vector3(((float)ContainerElement.Width - (float)EngravingContainer.ActualWidth) / 2f, _middleVisual.Offset.Y + _engravingOffset, 0f);
            }
            else
            {
                EngravingContainer.Visibility = Visibility.Collapsed;
            }
        }

        private CompositionBrush CreateNormalMapBrush(Compositor compositor, ICompositionSurface normalMapImage, ICompositionSurface colorMapImage)
        {
            var colorMapParameter = new CompositionEffectSourceParameter("ColorMap");
            var normalMapParameter = new CompositionEffectSourceParameter("NormalMap");

            var compositeEffect = new ArithmeticCompositeEffect()
            {
                Source1 = colorMapParameter,
                Source2 = new SceneLightingEffect()
                {
                    NormalMapSource = normalMapParameter
                }
            };

            var normalMapBrush = compositor.CreateSurfaceBrush(normalMapImage);
            var colorMapBrush = compositor.CreateSurfaceBrush(colorMapImage);
            normalMapBrush.Stretch = CompositionStretch.Fill;
            colorMapBrush.Stretch = CompositionStretch.Fill;

            var brush = compositor.CreateEffectFactory(compositeEffect).CreateBrush();
            brush.SetSourceParameter(colorMapParameter.Name, colorMapBrush);
            brush.SetSourceParameter(normalMapParameter.Name, normalMapBrush);

            return brush;
        }
    }
}
