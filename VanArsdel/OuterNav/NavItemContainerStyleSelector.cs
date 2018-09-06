// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VanArsdel
{
    public class NavItemContainerStyleSelector : StyleSelector
    {
        public Style ProductConfigurationMenuItemContainerStyle { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item is ProductConfigurationViewModel)
            {
                return ProductConfigurationMenuItemContainerStyle;
            }

            return base.SelectStyleCore(item, container);
        }
    }
}