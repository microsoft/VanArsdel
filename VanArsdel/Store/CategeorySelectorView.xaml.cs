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

namespace VanArsdel
{
    public sealed partial class CategorySelectorView : UserControl, IExitTransition
    {
        public CategorySelectorView(CategorySelectorViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(CategorySelectorViewModel), typeof(CategorySelectorView), new PropertyMetadata(null));

        public CategorySelectorViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as CategorySelectorViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PlayEntranceTransition();
        }

        private void PlayEntranceTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.2f, 1f));
            var duration = TimeSpan.FromMilliseconds(300);

            var backdropVisual = ElementCompositionPreview.GetElementVisual(Backdrop);
            var leftVisual = ElementCompositionPreview.GetElementVisual(LeftCategoryContent);
            var centerVisual = ElementCompositionPreview.GetElementVisual(CenterCategoryContent);
            var rightVisual = ElementCompositionPreview.GetElementVisual(RightCategoryContent);
            var captionVisual = ElementCompositionPreview.GetElementVisual(CaptionBlock);

            // Initial conditions
            backdropVisual.Opacity = 0f;

            leftVisual.Opacity = 0;
            leftVisual.Offset = new Vector3(800f, 0f, 0f);

            centerVisual.Opacity = 0;
            centerVisual.Offset = new Vector3(800f, 0f, 0f);

            rightVisual.Opacity = 0;
            rightVisual.Offset = new Vector3(800f, 0f, 0f);

            captionVisual.Opacity = 0;

            // Set up animations
            var backdropVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            backdropVisualOpacityAnim.Target = "Opacity";
            backdropVisualOpacityAnim.Duration = duration;
            backdropVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var leftVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            leftVisualOpacityAnim.Target = "Opacity";
            leftVisualOpacityAnim.Duration = duration;
            leftVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var leftVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            leftVisualOffsetAnim.Target = "Offset";
            leftVisualOffsetAnim.Duration = duration;
            leftVisualOffsetAnim.InsertKeyFrame(1f, Vector3.Zero, easingFunction);

            var centerVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            centerVisualOpacityAnim.Target = "Opacity";
            centerVisualOpacityAnim.Duration = duration;
            centerVisualOpacityAnim.DelayTime = TimeSpan.FromMilliseconds(20);
            centerVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var centerVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            centerVisualOffsetAnim.Target = "Offset";
            centerVisualOffsetAnim.Duration = duration;
            centerVisualOffsetAnim.DelayTime = TimeSpan.FromMilliseconds(20);
            centerVisualOffsetAnim.InsertKeyFrame(1f, Vector3.Zero, easingFunction);

            var rightVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            rightVisualOpacityAnim.Target = "Opacity";
            rightVisualOpacityAnim.Duration = duration;
            rightVisualOpacityAnim.DelayTime = TimeSpan.FromMilliseconds(40);
            rightVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var rightVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            rightVisualOffsetAnim.Target = "Offset";
            rightVisualOffsetAnim.Duration = duration;
            rightVisualOffsetAnim.DelayTime = TimeSpan.FromMilliseconds(40);
            rightVisualOffsetAnim.InsertKeyFrame(1f, Vector3.Zero, easingFunction);

            var captionVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            captionVisualOpacityAnim.Target = "Opacity";
            captionVisualOpacityAnim.Duration = duration;
            captionVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            // Start animations
            backdropVisual.StartAnimation(backdropVisualOpacityAnim.Target, backdropVisualOpacityAnim);

            leftVisual.StartAnimation(leftVisualOpacityAnim.Target, leftVisualOpacityAnim);
            leftVisual.StartAnimation(leftVisualOffsetAnim.Target, leftVisualOffsetAnim);

            centerVisual.StartAnimation(centerVisualOpacityAnim.Target, centerVisualOpacityAnim);
            centerVisual.StartAnimation(centerVisualOffsetAnim.Target, centerVisualOffsetAnim);

            rightVisual.StartAnimation(rightVisualOpacityAnim.Target, rightVisualOpacityAnim);
            rightVisual.StartAnimation(rightVisualOffsetAnim.Target, rightVisualOffsetAnim);

            captionVisual.StartAnimation(captionVisualOpacityAnim.Target, captionVisualOpacityAnim);
        }

        public Task PlayExitTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.7f, 0.0f), new Vector2(1.0f, 0.5f));
            var duration = TimeSpan.FromMilliseconds(500);

            var backdropVisual = ElementCompositionPreview.GetElementVisual(Backdrop);
            var leftVisual = ElementCompositionPreview.GetElementVisual(LeftCategoryContent);
            var centerVisual = ElementCompositionPreview.GetElementVisual(CenterCategoryContent);
            var rightVisual = ElementCompositionPreview.GetElementVisual(RightCategoryContent);
            var captionVisual = ElementCompositionPreview.GetElementVisual(CaptionBlock);

            // Set up animations
            var backdropVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            backdropVisualOpacityAnim.Target = "Opacity";
            backdropVisualOpacityAnim.Duration = duration;
            backdropVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var leftVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            leftVisualOpacityAnim.Target = "Opacity";
            leftVisualOpacityAnim.Duration = duration;
            leftVisualOpacityAnim.DelayTime = TimeSpan.FromMilliseconds(40);
            leftVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var leftVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            leftVisualOffsetAnim.Target = "Offset";
            leftVisualOffsetAnim.Duration = duration;
            leftVisualOffsetAnim.DelayTime = TimeSpan.FromMilliseconds(40);
            leftVisualOffsetAnim.InsertKeyFrame(1f, new Vector3(800f, 0f, 0f), easingFunction);

            var centerVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            centerVisualOpacityAnim.Target = "Opacity";
            centerVisualOpacityAnim.Duration = duration;
            centerVisualOpacityAnim.DelayTime = TimeSpan.FromMilliseconds(20);
            centerVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var centerVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            centerVisualOffsetAnim.Target = "Offset";
            centerVisualOffsetAnim.Duration = duration;
            centerVisualOffsetAnim.DelayTime = TimeSpan.FromMilliseconds(20);
            centerVisualOffsetAnim.InsertKeyFrame(1f, new Vector3(800f, 0f, 0f), easingFunction);

            var rightVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            rightVisualOpacityAnim.Target = "Opacity";
            rightVisualOpacityAnim.Duration = duration;
            rightVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var rightVisualOffsetAnim = compositor.CreateVector3KeyFrameAnimation();
            rightVisualOffsetAnim.Target = "Offset";
            rightVisualOffsetAnim.Duration = duration;
            rightVisualOffsetAnim.InsertKeyFrame(1f, new Vector3(800f, 0f, 0f), easingFunction);

            var captionVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            captionVisualOpacityAnim.Target = "Opacity";
            captionVisualOpacityAnim.Duration = duration;
            captionVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            // Start animations in scoped batch

            var scopedBatch = compositor.CreateScopedBatch(Windows.UI.Composition.CompositionBatchTypes.Animation);

            backdropVisual.StartAnimation(backdropVisualOpacityAnim.Target, backdropVisualOpacityAnim);

            leftVisual.StartAnimation(leftVisualOpacityAnim.Target, leftVisualOpacityAnim);
            leftVisual.StartAnimation(leftVisualOffsetAnim.Target, leftVisualOffsetAnim);

            centerVisual.StartAnimation(centerVisualOpacityAnim.Target, centerVisualOpacityAnim);
            centerVisual.StartAnimation(centerVisualOffsetAnim.Target, centerVisualOffsetAnim);

            rightVisual.StartAnimation(rightVisualOpacityAnim.Target, rightVisualOpacityAnim);
            rightVisual.StartAnimation(rightVisualOffsetAnim.Target, rightVisualOffsetAnim);

            captionVisual.StartAnimation(captionVisualOpacityAnim.Target, captionVisualOpacityAnim);

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
    }
}
