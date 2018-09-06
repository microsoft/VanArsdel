// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VanArsdel.Devices;

namespace VanArsdel
{
    public sealed partial class PropertyInspector : UserControl
    {
        public PropertyInspector()
        {
            this.InitializeComponent();
        }

        #region ActivePropertiesProperty

        public static readonly DependencyProperty ActivePropertiesProperty = DependencyProperty.Register("ActiveProperties", typeof(IReadOnlyList<IProperty>), typeof(PropertyInspector), new PropertyMetadata(null, new PropertyChangedCallback(OnActivePropertiesPropertyChanged)));

        private static void OnActivePropertiesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyInspector target)
            {
                target.OnActivePropertiesChanged(e.OldValue as IReadOnlyList<IProperty>, e.NewValue as IReadOnlyList<IProperty>);
            }
        }

        private void OnActivePropertiesChanged(IReadOnlyList<IProperty> oldValue, IReadOnlyList<IProperty> newValue)
        {
            if (newValue == null || newValue.Count == 0)
            {
                OuterGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                OuterGrid.Visibility = Visibility.Visible;
            }
        }

        public IReadOnlyList<IProperty> ActiveProperties
        {
            get { return GetValue(ActivePropertiesProperty) as IReadOnlyList<IProperty>; }
            set { SetValue(ActivePropertiesProperty, value); }
        }

        #endregion

        #region FooterContentProperty

        public static readonly DependencyProperty FooterContentProperty = DependencyProperty.Register("FooterContent", typeof(object), typeof(PropertyInspector), new PropertyMetadata(null));

        public object FooterContent
        {
            get { return GetValue(FooterContentProperty); }
            set { SetValue(FooterContentProperty, value); }
        }

        #endregion

        #region PresetsProperty

        public static readonly DependencyProperty PresetsProperty = DependencyProperty.Register("Presets", typeof(IReadOnlyList<Preset>), typeof(PropertyInspector), new PropertyMetadata(null));

        public IReadOnlyList<Preset> Presets
        {
            get { return GetValue(PresetsProperty) as IReadOnlyList<Preset>; }
            set { SetValue(PresetsProperty, value); }
        }

        #endregion

        #region VisibilityFilterProperty

        public static readonly DependencyProperty VisibilityFilterProperty = DependencyProperty.Register("VisibilityFilter", typeof(PropertyVisibilityFilter), typeof(PropertyInspector), new PropertyMetadata(PropertyVisibilityFilter.ShowAll, OnVisibilityFilterPropertyChanged));

        private static void OnVisibilityFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PropertyInspector target)
            {
                target.OnVisibilityFilterChanged((PropertyVisibilityFilter)e.OldValue, (PropertyVisibilityFilter)e.NewValue);
            }
        }

        private void OnVisibilityFilterChanged(PropertyVisibilityFilter oldValue, PropertyVisibilityFilter newValue)
        {
            switch (newValue)
            {
                case PropertyVisibilityFilter.ShowAll:
                    VisualStateManager.GoToState(this, "FilterShowAll", true);
                    break;
                case PropertyVisibilityFilter.ProductEditor:
                    VisualStateManager.GoToState(this, "FilterProductEditor", true);
                    break;
                case PropertyVisibilityFilter.MyLights:
                    VisualStateManager.GoToState(this, "FilterMyLights", true);
                    break;
            }
        }

        public PropertyVisibilityFilter VisibilityFilter
        {
            get { return (PropertyVisibilityFilter)GetValue(VisibilityFilterProperty); }
            set { SetValue(VisibilityFilterProperty, value); }
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var a = ActiveProperties;
            if (a == null || a.Count == 0)
            {
                OuterGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                OuterGrid.Visibility = Visibility.Visible;
            }
        }
    }
}
