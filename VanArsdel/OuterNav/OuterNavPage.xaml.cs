// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VanArsdel.Devices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using DepControls = Microsoft.UI.Xaml.Controls;

namespace VanArsdel
{
    public sealed partial class OuterNavPage : Page
    {
        private bool _dontNavigate;

        public OuterNavPage(OuterNavViewModel viewModel)
        {
            ViewModel = viewModel;

            viewModel.OverlayProvider.ShowCategorySelectorRequested += OverlayProvider_ShowCategorySelectorRequested;
            viewModel.OverlayProvider.HideCategorySelectorRequested += OverlayProvider_HideCategorySelectorRequested;
            viewModel.OverlayProvider.ShowProductConfigRequested += OverlayProvider_ShowProductConfigRequested;
            viewModel.OverlayProvider.HideProductConfigRequested += OverlayProvider_HideProductConfigRequested;
            viewModel.OverlayProvider.ShowPurchaseFormRequested += OverlayProvider_ShowPurchaseFormRequested;
            viewModel.OverlayProvider.HidePurchaseFormRequested += OverlayProvider_HidePurchaseFormRequested;

            this.InitializeComponent();
        }

        #region ViewModelProperty

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(OuterNavViewModel), typeof(OuterNavPage), new PropertyMetadata(null));

        public OuterNavViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as OuterNavViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = ViewModel?.MainViewModel;
        }

        private void CustomizationDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = ((FrameworkElement)sender).DataContext;

            if (NavView.SelectedItem == viewModel)
            {
                NavView.SelectedItem = ViewModel?.StoreViewModel;
            }

            ViewModel?.NavMenuItems.Remove(viewModel);
        }

        private void NavFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (NavView.SelectedItem != e.Parameter)
            {
                _dontNavigate = true;
                // Workaround bug in NavigationView that throws an exception when setting SelectedItem to an item that was just added
                try
                {
                    NavView.SelectedItem = e.Parameter;
                }
                catch { }
                _dontNavigate = false;
            }
        }

        private async Task NavigateWithTransition(Type sourcePageType, object parameter)
        {
            if (_categorySelectorView != null)
            {
                _ = HideCategorySelector();
            }
            if (_purchaseView != null)
            {
                _ = HidePurchaseForm();
            }
            if (NavFrame.Content is IExitTransition page)
            {
                await page.PlayExitTransition();
            }

            NavFrame.Navigate(sourcePageType, parameter, new SuppressNavigationTransitionInfo());
        }

        private async Task GoBackWithTransition()
        {
            if (NavFrame.Content is IExitTransition page)
            {
                await page.PlayExitTransition();
            }

            NavFrame.GoBack(new SuppressNavigationTransitionInfo());
        }

        private void OnMenuItemSelectionChanged(DepControls.NavigationView sender, DepControls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem != null && !_dontNavigate)
            {
                Type pageType = null;

                switch (args.SelectedItem)
                {
                    case MainViewModel main:
                        pageType = typeof(MainPage);
                        break;
                    case StoreViewModel store:
                        pageType = typeof(StorePage);
                        break;
                    case ProductConfigurationViewModel config:
                        pageType = typeof(ProductConfigurationPage);
                        break;
                }

                if (pageType != null)
                {
                    _ = NavigateWithTransition(pageType, args.SelectedItem);
                }
            }
        }

        private void OnBackRequested(DepControls.NavigationView sender, DepControls.NavigationViewBackRequestedEventArgs args)
        {
            if (_categorySelectorView != null)
            {
                _ = HideCategorySelector();
            }
            else if (_purchaseView != null)
            {
                _ = HidePurchaseForm();
            }
            else
            {
                _ = GoBackWithTransition();
            }
        }

        private CategorySelectorView _categorySelectorView = null;

        private void OverlayProvider_HideCategorySelectorRequested(IOverlayProvider obj)
        {
            if (_categorySelectorView == null)
            {
                return;
            }
            _ = HideCategorySelector();
        }

        private async Task HideCategorySelector()
        {
            NavFrame.IsEnabled = true;
            await _categorySelectorView.PlayExitTransition();
            _categorySelectorView = null;
            OverlayContainer.Child = null;
        }

        private void OverlayProvider_ShowCategorySelectorRequested(IOverlayProvider obj)
        {
            if (_categorySelectorView != null)
            {
                return;
            }
            NavFrame.IsEnabled = false;
            _categorySelectorView = new CategorySelectorView(ViewModel.CategorySelectorViewModel);
            OverlayContainer.Child = _categorySelectorView;
        }

        private void OverlayProvider_ShowProductConfigRequested(IOverlayProvider source, DeviceCategory category, string connectedAnimationId)
        {
            _ = AddNewProductConfig(category, connectedAnimationId);
        }

        private int _productConfigCounter = 1;
        private async Task AddNewProductConfig(DeviceCategory category, string connectedAnimationId)
        {
            var vm = ViewModel;
            var device = category.DefaultDevice.Clone();
            device.Caption += " " + _productConfigCounter.ToString();
            _productConfigCounter++;
            var configVM = new ProductConfigurationViewModel(vm.OverlayProvider, device);
            await configVM.SetProductImageSourceAsync(category.ThumbnailPath);
            vm.AddNavMenuItem(configVM);
            _ = NavigateWithTransition(typeof(ProductConfigurationPage), configVM);
        }

        private void OverlayProvider_HideProductConfigRequested(IOverlayProvider source, ProductConfigurationViewModel configViewModel)
        {
            var vm = ViewModel;
            _ = NavigateWithTransition(typeof(StorePage), vm.StoreViewModel);
            vm.RemoveNavMenuItem(configViewModel);
        }

        private PurchaseViewBase _purchaseView = null;

        private void OverlayProvider_ShowPurchaseFormRequested(IOverlayProvider source, bool compactMode)
        {
            NavFrame.IsEnabled = false;
            if (compactMode)
            {
                _purchaseView = new CompactPurchaseView(new PurchaseViewModel(ViewModel.OverlayProvider));
            }
            else
            {
                _purchaseView = new PurchaseView(new PurchaseViewModel(ViewModel.OverlayProvider));
            }
            OverlayContainer.Child = _purchaseView;
        }

        private void OverlayProvider_HidePurchaseFormRequested(IOverlayProvider source)
        {
            if (_purchaseView == null)
            {
                return;
            }
            _ = HidePurchaseForm();
        }

        private async Task HidePurchaseForm()
        {
            NavFrame.IsEnabled = true;
            await _purchaseView.PlayExitTransition();
            _purchaseView = null;
            OverlayContainer.Child = null;
        }
    }
}