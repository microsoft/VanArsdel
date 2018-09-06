// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace VanArsdel
{
    public class PurchaseViewModel
    {
        public PurchaseViewModel(IOverlayProvider overlayProvider)
        {
            _overlayProvider = overlayProvider;
        }

        private IOverlayProvider _overlayProvider;

        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string CardExpirationMonth { get; set; }
        public string CardExpirationYear { get; set; }
        public string CCV { get; set; }

        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingZip { get; set; }

        public void OnLightDismiss(object sender, TappedRoutedEventArgs e)
        {
            _overlayProvider.HidePurchaseForm();
        }

        public void OnCancel(object sender, RoutedEventArgs e)
        {
            _overlayProvider.HidePurchaseForm();
        }
    }
}