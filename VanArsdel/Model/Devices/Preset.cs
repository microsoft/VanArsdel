// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace VanArsdel.Devices
{
    public class Preset : INotifyPropertyChanged
    {
        public static List<Preset> ParsePresetList(JsonArray data)
        {
            var retVal = new List<Preset>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParsePreset(node.GetObject()));
            }

            return retVal;
        }

        public static Preset ParsePreset(JsonObject data)
        {
            var itemList = PresetItem.ParsePresetItemList(data["Items"].GetArray());

            return new Preset(data["Id"].GetString(), data["Caption"].GetString(), new Uri(data["IconPath"].GetString()), itemList);
        }

        public Preset(string id, string caption, Uri iconPath, List<PresetItem> presetItems)
        {
            if (presetItems == null || presetItems.Count == 0)
            {
                throw new ArgumentNullException("presetItems");
            }
            _presetItems = presetItems;
            _id = id;
            _caption = caption;
            _iconPath = iconPath;
            CheckForActive();
        }

        private List<PresetItem> _presetItems;

        protected readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private Uri _iconPath;
        public Uri IconPath
        {
            get { return _iconPath; }
            set
            {
                if (_iconPath != value)
                {
                    _iconPath = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isActive)
            {
                // Force ToggleButton back into selected state.
                RaisePropertyChanged("IsActive");
            }

            ActivatePreset();
        }

        private IReadOnlyList<IProperty> _activeProperties;

        public void SetActiveProperties(IReadOnlyList<IProperty> activeProperties)
        {
            if (_activeProperties != null)
            {
                foreach (var property in _activeProperties)
                {
                    property.ValueChanged -= Property_ValueChanged;
                }
            }

            _activeProperties = activeProperties;
            if (_activeProperties != null)
            {
                foreach (var property in _activeProperties)
                {
                    property.ValueChanged -= Property_ValueChanged;
                    property.ValueChanged += Property_ValueChanged;
                }
            }
            CheckForActive();
        }

        private void Property_ValueChanged(IProperty obj)
        {
            CheckForActive();
        }

        private void CheckForActive()
        {
            if (_activeProperties == null || _activeProperties.Count == 0)
            {
                IsActive = false;
                return;
            }
            foreach (var presetItem in _presetItems)
            {
                bool isPresetItemActive = true;

                foreach (var prop in _activeProperties)
                {
                    if (prop.Id == presetItem.PropertyId)
                    {
                        if (!prop.ComparePresetValueString(presetItem.Value))
                        {
                            isPresetItemActive = false;
                            break;
                        }
                    }
                }

                if (!isPresetItemActive)
                {
                    IsActive = false;
                    return;
                }
            }
            IsActive = true;
        }

        public void ActivatePreset()
        {
            if (_activeProperties != null)
            {
                foreach (var property in _activeProperties)
                {
                    foreach (var presetItem in _presetItems)
                    {
                        if (presetItem.PropertyId == property.Id)
                        {
                            property.SetPresetValueString(presetItem.Value);
                        }
                    }
                }
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void RaisePropertyChangedFromSource([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
