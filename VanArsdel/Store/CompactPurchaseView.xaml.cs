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
    public sealed partial class CompactPurchaseView : PurchaseViewBase
    {
        public CompactPurchaseView(PurchaseViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(PurchaseViewModel), typeof(PurchaseView), new PropertyMetadata(null));

        public PurchaseViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as PurchaseViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CardHolderNameField.Focus(FocusState.Programmatic);
            PlayEntranceTransition();
        }

        private void PlayEntranceTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.1f, 0.9f), new Vector2(0.2f, 1f));
            var duration = TimeSpan.FromMilliseconds(300);

            var backdropVisual = ElementCompositionPreview.GetElementVisual(Backdrop);
            var purchaseFormVisual = ElementCompositionPreview.GetElementVisual(PurchaseForm);

            // Initial conditions
            backdropVisual.Opacity = 0f;
            purchaseFormVisual.Opacity = 0f;

            // Set up animations
            var backdropVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            backdropVisualOpacityAnim.Target = "Opacity";
            backdropVisualOpacityAnim.Duration = duration;
            backdropVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            var purchaseFormVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            purchaseFormVisualOpacityAnim.Target = "Opacity";
            purchaseFormVisualOpacityAnim.Duration = duration;
            purchaseFormVisualOpacityAnim.InsertKeyFrame(1f, 1f, easingFunction);

            // Start animations
            backdropVisual.StartAnimation(backdropVisualOpacityAnim.Target, backdropVisualOpacityAnim);
            purchaseFormVisual.StartAnimation(purchaseFormVisualOpacityAnim.Target, purchaseFormVisualOpacityAnim);
        }

        public override Task PlayExitTransition()
        {
            var compositor = Window.Current.Compositor;

            var easingFunction = compositor.CreateCubicBezierEasingFunction(new Vector2(0.7f, 0.0f), new Vector2(1.0f, 0.5f));
            var duration = TimeSpan.FromMilliseconds(500);

            var backdropVisual = ElementCompositionPreview.GetElementVisual(Backdrop);
            var purchaseFormVisual = ElementCompositionPreview.GetElementVisual(PurchaseForm);

            // Set up animations
            var backdropVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            backdropVisualOpacityAnim.Target = "Opacity";
            backdropVisualOpacityAnim.Duration = duration;
            backdropVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            var purchaseFormVisualOpacityAnim = compositor.CreateScalarKeyFrameAnimation();
            purchaseFormVisualOpacityAnim.Target = "Opacity";
            purchaseFormVisualOpacityAnim.Duration = duration;
            purchaseFormVisualOpacityAnim.InsertKeyFrame(1f, 0f, easingFunction);

            // Start animations in scoped batch

            var scopedBatch = compositor.CreateScopedBatch(Windows.UI.Composition.CompositionBatchTypes.Animation);

            backdropVisual.StartAnimation(backdropVisualOpacityAnim.Target, backdropVisualOpacityAnim);
            purchaseFormVisual.StartAnimation(purchaseFormVisualOpacityAnim.Target, purchaseFormVisualOpacityAnim);

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
