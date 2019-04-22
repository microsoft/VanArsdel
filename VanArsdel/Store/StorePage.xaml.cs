// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Numerics;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VanArsdel
{
    public sealed partial class StorePage : Page
    {
        public StorePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as StoreViewModel;

            HeroVideo.AutoPlay = true;
            HeroVideo.MediaPlayer.IsLoopingEnabled = true;
            HeroVideo.Source = MediaSource.CreateFromUri(new System.Uri("ms-appx:///Assets/Videos/HeroVideo.mp4"));
            HeroVideo.MediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            base.OnNavigatedTo(e);
        }

        private void PlaybackSession_PlaybackStateChanged(Windows.Media.Playback.MediaPlaybackSession sender, object args)
        {
            if (sender.PlaybackState == Windows.Media.Playback.MediaPlaybackState.Playing)
            {
                sender.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    HeroVideo.Opacity = 1;
                    ProductsViewer.Opacity = 1;
                    ProductsViewer.Translation = Vector3.Zero;
                });
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            HeroVideo.MediaPlayer.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;
            base.OnNavigatingFrom(e);
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(StoreViewModel), typeof(StorePage), new PropertyMetadata(null));

        public StoreViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as StoreViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        private void StoreCustomizeTip_Loaded(object sender, RoutedEventArgs e)
        {
            StoreCustomizeTip.IsOpen = true;
        }

        private void StoreCustomizeTip_Closing(Microsoft.UI.Xaml.Controls.TeachingTip sender, Microsoft.UI.Xaml.Controls.TeachingTipClosingEventArgs args)
        {
            EngraveTextTip.IsOpen = true;
        }
    }
}
