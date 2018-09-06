// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VanArsdel.Devices;

namespace VanArsdel
{
    public class OuterNavViewModel : INotifyPropertyChanged
    {
        public OuterNavViewModel(IOverlayProvider overlayProvider, MainViewModel mainViewModel, StoreViewModel storeViewModel, CategorySelectorViewModel categorySelectorViewModel)
        {
            _overlayProvider = overlayProvider;
            _mainViewModel = mainViewModel;
            _storeViewModel = storeViewModel;
            _categorySelectorViewModel = categorySelectorViewModel;

            _navMenuItems = new ObservableList<object>()
            {
                _mainViewModel,
                _storeViewModel,
                new NavigationViewItemSeparator()
            };
        }

        private CategorySelectorViewModel _categorySelectorViewModel;
        public CategorySelectorViewModel CategorySelectorViewModel
        {
            get { return _categorySelectorViewModel; }
        }

        private IOverlayProvider _overlayProvider;
        public IOverlayProvider OverlayProvider
        {
            get { return _overlayProvider; }
        }

        private MainViewModel _mainViewModel;
        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
        }

        private StoreViewModel _storeViewModel;
        public StoreViewModel StoreViewModel
        {
            get { return _storeViewModel; }
        }

        private ObservableList<object> _navMenuItems;
        public ObservableList<object> NavMenuItems
        {
            get { return _navMenuItems; }
        }

        public void AddNavMenuItem(object item)
        {
            _navMenuItems.Add(item);
        }

        public void RemoveNavMenuItem(object item)
        {
            _navMenuItems.Remove(item);
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