// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VanArsdel.Devices;
using Windows.UI.Xaml.Input;
using System.Linq;
using Windows.UI.Xaml;

namespace VanArsdel
{
    public class CategorySelectorViewModel : INotifyPropertyChanged
    {
        public CategorySelectorViewModel(IDeviceProvider deviceProvider, IOverlayProvider overlayProvider)
        {
            _deviceProvider = deviceProvider;

            _leftCategory = _deviceProvider.DeviceCategories.FirstOrDefault((a) => a.Id == "FloorLamp");
            _centerCategory = _deviceProvider.DeviceCategories.FirstOrDefault((a) => a.Id == "DeskLamp");
            _rightCategory = _deviceProvider.DeviceCategories.FirstOrDefault((a) => a.Id == "HangingLamp");

            _overlayProvider = overlayProvider;
        }

        private IDeviceProvider _deviceProvider;
        private IOverlayProvider _overlayProvider;

        private DeviceCategory _leftCategory;
        public DeviceCategory LeftCategory
        {
            get { return _leftCategory; }
        }

        private DeviceCategory _centerCategory;
        public DeviceCategory CenterCategory
        {
            get { return _centerCategory; }
        }

        private DeviceCategory _rightCategory;
        public DeviceCategory RightCategory
        {
            get { return _rightCategory; }
        }

        public void OnLightDismiss(object sender, TappedRoutedEventArgs e)
        {
            _overlayProvider.CloseCategorySelector();
        }

        public void OnLeftCategoryClicked(object sender, RoutedEventArgs e)
        {
            _overlayProvider.ShowProductConfig(_leftCategory, null);
        }

        public void OnCenterCategoryClicked(object sender, RoutedEventArgs e)
        {
            _overlayProvider.ShowProductConfig(_centerCategory, null);
        }

        public void OnRightCategoryClicked(object sender, RoutedEventArgs e)
        {
            _overlayProvider.ShowProductConfig(_rightCategory, null);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}