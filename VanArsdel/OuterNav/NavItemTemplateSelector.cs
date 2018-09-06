// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VanArsdel
{
    public class NavItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProductConfigurationItemTemplate { get; set; }
        
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var template = GetItemTemplateOverride(item);

            return template ?? base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var template = GetItemTemplateOverride(item);

            return template ?? base.SelectTemplateCore(item, container);
        }

        private DataTemplate GetItemTemplateOverride(object item)
        {
            if (item is ProductConfigurationViewModel)
            {
                return ProductConfigurationItemTemplate;
            }

            return null;
        }
    }
}