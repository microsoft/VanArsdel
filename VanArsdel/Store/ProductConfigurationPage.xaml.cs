// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

namespace VanArsdel
{
    public sealed partial class ProductConfigurationPage : Page, IExitTransition
    {
        public ProductConfigurationPage()
        {
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(ProductConfigurationViewModel), typeof(ProductConfigurationPage), new PropertyMetadata(null));

        public ProductConfigurationViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as ProductConfigurationViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as ProductConfigurationViewModel;

            PlayEntranceTransition();

            base.OnNavigatedTo(e);
        }

        private void PlayEntranceTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.2f, 1f));
            var duration = TimeSpan.FromMilliseconds(300);

            var propertiesVisual = ElementCompositionPreview.GetElementVisual(Properties);
            var deviceVisual = ElementCompositionPreview.GetElementVisual(DeviceLayout);
            var dayNightVisual = ElementCompositionPreview.GetElementVisual(DayNightSelectionPanel);

            // Initial conditions
            propertiesVisual.Opacity = 0f;
            propertiesVisual.Offset = new Vector3(344f, 0f, 0f);

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

            deviceVisual.StartAnimation(deviceVisualOpacityAnim.Target, deviceVisualOpacityAnim);
            dayNightVisual.StartAnimation(dayNightVisualOpacityAnim.Target, dayNightVisualOpacityAnim);
        }

        public Task PlayExitTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.7f, 0.0f), new Vector2(1.0f, 0.5f));
            var duration = TimeSpan.FromMilliseconds(300);

            var propertiesVisual = ElementCompositionPreview.GetElementVisual(Properties);
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