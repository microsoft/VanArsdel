// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VanArsdel.Devices;
using Windows.UI.Xaml;

namespace VanArsdel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel(IStringProvider stringProvider, IDeviceProvider deviceProvider, IContextActionProvider contextActionProvider)
        {
            _stringProvider = stringProvider;
            _deviceProvider = deviceProvider;
            _contextActionProvider = contextActionProvider;

            _deviceProvider.SelectedDevicesChanged += _deviceProvider_SelectedDevicesChanged;
            _deviceProvider.RoomStructureChanged += _deviceProvider_RoomStructureChanged;

            _groupedDevices = _deviceProvider.Devices.GroupBy(
                (device) =>
                {
                    return device.Room;
                },
                (room, devices) =>
                {
                    return new Devices.DevicesGroup(room, devices);
                }).FillEmptyRooms(_deviceProvider.Rooms);
        }

        private IStringProvider _stringProvider;
        private IDeviceProvider _deviceProvider;
        private IContextActionProvider _contextActionProvider;

        private void _deviceProvider_SelectedDevicesChanged(IDeviceProvider sender, IReadOnlyList<Device> selected)
        {
            RaisePropertyChanged("SelectedDevices");
            RaisePropertyChanged("ActiveProperties");
            SelectedDevicesChanged?.Invoke(this, selected);
        }

        private void _deviceProvider_RoomStructureChanged(IDeviceProvider obj)
        {
            _updatingGroupedDevices = true;
            RaisePropertyChanged("GroupedDevices");
            _updatingGroupedDevices = false;
            SelectedDevicesChanged(this, _deviceProvider.SelectedDevices);
        }

        private bool _updatingGroupedDevices = false;

        public IReadOnlyList<Preset> Presets
        {
            get { return _deviceProvider.Presets; }
        }

        public IReadOnlyList<Device> SelectedDevices
        {
            get { return _deviceProvider.SelectedDevices; }
        }

        public void SelectDevices(IEnumerable<object> selected)
        {
            if (_updatingGroupedDevices)
            {
                return;
            }
            _deviceProvider.SelectDevices(selected);
        }

        public event Action<MainViewModel, IReadOnlyList<Device>> SelectedDevicesChanged;

        private IEnumerable<DevicesGroup> _groupedDevices;
        public IEnumerable<DevicesGroup> GroupedDevices
        {
            get { return _groupedDevices; }
        }

        public IReadOnlyList<IProperty> ActiveProperties
        {
            get { return _deviceProvider.ActiveProperties; }
        }

        public void AddNewRoom(FrameworkElement attachTo)
        {
            _contextActionProvider.ShowNewRoomFlyout(attachTo, string.Empty, (name) =>
            {
                if (!string.IsNullOrEmpty(name))
                {
                    _deviceProvider.AddNewRoom(name);
                }
            });
        }

        public override string ToString()
        {
            return _stringProvider?.GetString("MainPageNavMenuItem");
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
