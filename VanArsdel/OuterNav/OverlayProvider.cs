// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanArsdel.Devices;

namespace VanArsdel
{
    public interface IOverlayProvider
    {
        void ShowCategorySelector();
        void CloseCategorySelector();
        void ShowProductConfig(DeviceCategory category, string connectedAnimationId);
        void HideProductConfig(ProductConfigurationViewModel source);
        void ShowPurchaseForm(bool compactMode);
        void HidePurchaseForm();

        event Action<IOverlayProvider> ShowCategorySelectorRequested;
        event Action<IOverlayProvider> HideCategorySelectorRequested;
        event Action<IOverlayProvider, DeviceCategory, string> ShowProductConfigRequested;
        event Action<IOverlayProvider, ProductConfigurationViewModel> HideProductConfigRequested;
        event Action<IOverlayProvider, bool> ShowPurchaseFormRequested;
        event Action<IOverlayProvider> HidePurchaseFormRequested;
    }

    // Super minimal impelmentaiton of this pattern. It could be expanded to include a stack of overlays etc... 
    public class OverlayProvider : IOverlayProvider
    {
        public void ShowCategorySelector()
        {
            ShowCategorySelectorRequested?.Invoke(this);
        }

        public void CloseCategorySelector()
        {
            HideCategorySelectorRequested?.Invoke(this);
        }

        public void ShowProductConfig(DeviceCategory category, string connectedAnimationId)
        {
            ShowProductConfigRequested?.Invoke(this, category, connectedAnimationId);
        }

        public void HideProductConfig(ProductConfigurationViewModel source)
        {
            HideProductConfigRequested?.Invoke(this, source);
        }

        public void ShowPurchaseForm(bool compactMode)
        {
            ShowPurchaseFormRequested?.Invoke(this, compactMode);
        }

        public void HidePurchaseForm()
        {
            HidePurchaseFormRequested?.Invoke(this);
        }

        public event Action<IOverlayProvider> ShowCategorySelectorRequested;
        public event Action<IOverlayProvider> HideCategorySelectorRequested;
        public event Action<IOverlayProvider, DeviceCategory, string> ShowProductConfigRequested;
        public event Action<IOverlayProvider, ProductConfigurationViewModel> HideProductConfigRequested;
        public event Action<IOverlayProvider, bool> ShowPurchaseFormRequested;
        public event Action<IOverlayProvider> HidePurchaseFormRequested;
    }
}
