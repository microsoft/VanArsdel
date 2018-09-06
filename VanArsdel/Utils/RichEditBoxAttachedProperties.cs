// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VanArsdel.Utils
{
    public static class RichEditBoxAttachedProperties
    {
        public static DependencyProperty BindableTextProperty = DependencyProperty.RegisterAttached("BindableText", typeof(string), typeof(RichEditBoxAttachedProperties), new PropertyMetadata(null, OnBindablePropertyChanged));

        private static void OnBindablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichEditBox target)
            {
                target.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, e.NewValue as string);
            }
        }

        public static string GetBindableText(DependencyObject target)
        {
            return target.GetValue(BindableTextProperty) as string;
        }

        public static void SetBindableText(DependencyObject target, string value)
        {
            target.SetValue(BindableTextProperty, value);
        }
    }
}
