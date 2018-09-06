// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VanArsdel.Devices
{
    public enum PropertyVisibilityFilter { ShowAll, ProductEditor, MyLights };

    public class PropertyTemplateSelector : DataTemplateSelector
    {
        public PropertyVisibilityFilter VisibilityFilter { set; get; } = PropertyVisibilityFilter.ShowAll;
        public DataTemplate EmptyTemplate { get; set; }
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate ToggleSwitchTemplate { get; set; }
        public DataTemplate SliderTemplate { get; set; }
        public DataTemplate ColorPickerTemplate { get; set; }
        public DataTemplate ColorPaletteTemplate { get; set; }
        public DataTemplate BitmapPickerTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return SelectTemplateCore(item, null);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is IProperty property)
            {
                if (VisibilityFilter == PropertyVisibilityFilter.ProductEditor && !property.IsVisibleInProductEditor)
                {
                    return EmptyTemplate;
                }
                if (VisibilityFilter == PropertyVisibilityFilter.MyLights && !property.IsVisibleInMyLights)
                {
                    return EmptyTemplate;
                }

                switch (property.Editor)
                {
                    case PropertyEditorType.ComboBox:
                        return ComboBoxTemplate;
                    case PropertyEditorType.ToggleSwitch:
                        return ToggleSwitchTemplate;
                    case PropertyEditorType.Slider:
                        return SliderTemplate;
                    case PropertyEditorType.ColorPicker:
                        return ColorPickerTemplate;
                    case PropertyEditorType.ColorPalette:
                        return ColorPaletteTemplate;
                    case PropertyEditorType.BitmapPicker:
                        return BitmapPickerTemplate;
                    case PropertyEditorType.String:
                        return StringTemplate;
                    default:
                        return EmptyTemplate;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
