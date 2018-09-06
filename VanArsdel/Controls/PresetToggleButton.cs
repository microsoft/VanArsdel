// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;

namespace VanArsdel
{
    public class PresetToggleButton : ToggleButton
    {
        public PresetToggleButton()
        {
            this.DefaultStyleKey = typeof(PresetToggleButton);
        }

        #region IconPathProperty

        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(Uri), typeof(PresetToggleButton), new PropertyMetadata(null, new PropertyChangedCallback(OnIconPathPropertyChanged)));

        private static void OnIconPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is PresetToggleButton target)
            {
                target.SetIconSource();
            }
        }

        public Uri IconPath
        {
            get { return GetValue(IconPathProperty) as Uri; }
            set { SetValue(IconPathProperty, value); }
        }

        #endregion

        #region CaptionProperty

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(PresetToggleButton), new PropertyMetadata(null));

        public string Caption
        {
            get { return GetValue(CaptionProperty) as string; }
            set { SetValue(CaptionProperty, value); }
        }

        #endregion

        protected override void OnApplyTemplate()
        {
            SetIconSource();

            base.OnApplyTemplate();
        }

        private void SetIconSource()
        {
            var iconImage = GetTemplateChild("IconImage") as Image;
            if (iconImage == null)
            {
                return;
            }
            var path = IconPath;
            if (path == null)
            {
                iconImage.Source = null;
            }
            else
            {
                BitmapImage imageSource = new BitmapImage();
                imageSource.UriSource = IconPath;
                iconImage.Source = imageSource;
            }
        }
    }
}
