// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace VanArsdel
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            ApplicationView.PreferredLaunchViewSize = new Size(1920, 1008);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (_initialized)
            {
                if (e.PrelaunchActivated == false)
                {
                    Window.Current.Activate();
                }
            }
            else
            {
                _ = FirstTimeInit(e);
            }
        }

        private async Task FirstTimeInit(LaunchActivatedEventArgs e)
        {
            await SetupDependencies();

            Window.Current.Content = new OuterNavPage(_outerNavViewModel);

            if (e.PrelaunchActivated == false)
            {
                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            if (_initialized)
            {
                var deferral = e.SuspendingOperation.GetDeferral();
                var suspendTask = _deviceProvider.HandleSuspend();
                suspendTask.ContinueWith((t) =>
                {
                    deferral.Complete();
                });
            }
        }

        private async Task SetupDependencies()
        {
            _stringProvider = new StringProvider(Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView());
            _deviceProvider = new DeviceProvider(_stringProvider);
            _contextActionProvider = new ContextActionProvider(_stringProvider, _deviceProvider);
            _overlayProvider = new OverlayProvider();

            await _deviceProvider.InitializeData(_contextActionProvider, _stringProvider.GetString("DataPath"));

            _storeViewModel = new StoreViewModel(_overlayProvider, _stringProvider, _deviceProvider);
            _mainViewModel = new MainViewModel(_stringProvider, _deviceProvider, _contextActionProvider);
            _categorySelectorViewModel = new CategorySelectorViewModel(_deviceProvider, _overlayProvider);
            _outerNavViewModel = new OuterNavViewModel(_overlayProvider, _mainViewModel, _storeViewModel, _categorySelectorViewModel);

            _initialized = true;
        }

        private bool _initialized = false;
        private IStringProvider _stringProvider;
        private IContextActionProvider _contextActionProvider;
        private IDeviceProvider _deviceProvider;
        private IOverlayProvider _overlayProvider;
        private OuterNavViewModel _outerNavViewModel;
        private MainViewModel _mainViewModel;
        private StoreViewModel _storeViewModel;
        private CategorySelectorViewModel _categorySelectorViewModel;
    }
}
