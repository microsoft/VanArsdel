// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VanArsdel.Devices;
using VanArsdel.Model;
using Windows.UI.Xaml;

namespace VanArsdel
{
    public class StoreViewModel : INotifyPropertyChanged
    {
        public StoreViewModel(IOverlayProvider overlayProvider, IStringProvider stringProvider, IDeviceProvider deviceProvider)
        {
            _overlayProvider = overlayProvider;
            _stringProvider = stringProvider;
            _deviceProvider = deviceProvider;
        }

        private IOverlayProvider _overlayProvider;
        private IStringProvider _stringProvider;
        private IDeviceProvider _deviceProvider;
        
        public IEnumerable<Product> Products
        {
            get => _deviceProvider?.Products;
        }

        public void OnCustomizeClick(object sender, RoutedEventArgs e)
        {
            _overlayProvider.ShowCategorySelector();
        }

        public override string ToString()
        {
            return _stringProvider?.GetString("StorePageNavMenuItem");
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