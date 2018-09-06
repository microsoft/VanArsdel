// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VanArsdel.Devices;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VanArsdel
{

    public class ProductConfigurationViewModel : INotifyPropertyChanged
    {
        public ProductConfigurationViewModel(IOverlayProvider overlayProvider, Device device)
        {
            _overlayProvider = overlayProvider;
            _device = device;
            _deviceWrapper = new[] { _device };
        }

        private IOverlayProvider _overlayProvider;
        private Device _device;
        private IReadOnlyList<Device> _deviceWrapper;
        private ImageSource _productImageSource;

        public ImageSource ProductImageSource
        {
            get => _productImageSource;
        }

        public Device Device
        {
            get => _device;
        }

        public IReadOnlyList<Device> DeviceWrapper
        {
            get => _deviceWrapper;
        }

        public void OnBeginPurchase(object sender, RoutedEventArgs e)
        {
            var ctrlState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            if ((ctrlState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
            {
                _overlayProvider.ShowPurchaseForm(true);
            }
            else
            {
                _overlayProvider.ShowPurchaseForm(false);
            }
        }

        public void OnCancelPurchase(object sender, RoutedEventArgs e)
        {
            _overlayProvider.HideProductConfig(this);
        }

        public async Task SetProductImageSourceAsync(string imagePath)
        {
            var imageUri = new Uri(imagePath);
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(imageUri);

            using (IRandomAccessStream imageFileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(imageFileStream);
                _productImageSource = bitmapImage;
            }
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