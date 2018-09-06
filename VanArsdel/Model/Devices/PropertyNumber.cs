// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public interface IPropertyNumber : IProperty
    {
        double Value { get; set; }
        double SmallChange { get; set; }
        double LargeChange { get; set; }
        double Min { get; set; }
        double Max { get; set; }
        double StepValue { get; set; }
    }


    public class PropertyNumber : Property, IPropertyNumber
    {
        public static PropertyNumber ParsePropertyNumber(JsonObject data)
        {
            double value = double.Parse(data["Value"].GetString());
            double smallChange = double.Parse(data["SmallChange"].GetString());
            double largeChange = double.Parse(data["LargeChange"].GetString());
            double min = double.Parse(data["Min"].GetString());
            double max = double.Parse(data["Max"].GetString());
            double stepValue = double.Parse(data["StepValue"].GetString());
            bool isVisibleInProductEditor = bool.Parse(data["IsVisibleInProductEditor"].GetString());
            bool isVisibleInMyLights = bool.Parse(data["IsVisibleInMyLights"].GetString());
            List<PropertyMetadataItem> metadata = null;
            if (data.ContainsKey("Metadata"))
            {
                metadata = PropertyMetadataItem.ParsePropertyMetadataItemList(data["Metadata"].GetArray());
            }
            return new PropertyNumber(data["Id"].GetString(), data["Caption"].GetString(), value, smallChange, largeChange, min, max, stepValue, isVisibleInProductEditor, isVisibleInMyLights, metadata);
        }

        public PropertyNumber(string id, string header, double value, double smallChange, double largeChange, double min, double max, double stepValue, bool isVisibleInProductEditor, bool isVisibleInMyLights, List<PropertyMetadataItem> metadata)
            : base(id, PropertyEditorType.Slider, header, isVisibleInProductEditor, isVisibleInMyLights, metadata)
        {
            _header = header;
            _value = value;
            _smallChange = smallChange;
            _largeChange = largeChange;
            _min = min;
            _max = max;
            _stepValue = stepValue;
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaiseValueChanged();
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private double _smallChange;
        public double SmallChange
        {
            get { return _smallChange; }
            set
            {
                if (_smallChange != value)
                {
                    _smallChange = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private double _largeChange;
        public double LargeChange
        {
            get { return _largeChange; }
            set
            {
                if (_largeChange != value)
                {
                    _largeChange = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private double _min;
        public double Min
        {
            get { return _min; }
            set
            {
                if (_min != value)
                {
                    _min = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private double _max;
        public double Max
        {
            get { return _max; }
            set
            {
                if (_max != value)
                {
                    _max = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        private double _stepValue;
        public double StepValue
        {
            get { return _stepValue; }
            set
            {
                if (_stepValue != value)
                {
                    _stepValue = value;
                    RaisePropertyChangedFromSource();
                }
            }
        }

        public override bool ComparePresetValueString(string value)
        {
            double v;
            if (double.TryParse(value, out v))
            {
                return _value == v;
            }
            return false;
        }

        public override void SetPresetValueString(string value)
        {
            double v;
            if (double.TryParse(value, out v))
            {
                Value = v;
            }
        }


        public override IProperty Clone()
        {
            return new PropertyNumber(_id, _header, _value, _smallChange, _largeChange, _min, _max, _stepValue, _isVisibleInProductEditor, _isVisibleInMyLights, _metadata.Clone());
        }
    }

    public class PropertyNumberForwarder : PropertyForwarder<IPropertyNumber>, IPropertyNumber
    {
        public PropertyNumberForwarder(string id, IEnumerable<IProperty> linkedProperties)
            : base(id, PropertyEditorType.Slider, linkedProperties)
        { }

        public double Value
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return 0;
                }
                return _linkedProperties[0].Value;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.Value = value;
                }
            }
        }

        public double SmallChange
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return 0;
                }
                return _linkedProperties[0].SmallChange;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.SmallChange = value;
                }
            }
        }

        public double LargeChange
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return 0;
                }
                return _linkedProperties[0].LargeChange;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.LargeChange = value;
                }
            }
        }

        public double Min
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return 0;
                }
                return _linkedProperties[0].Min;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.Min = value;
                }
            }
        }

        public double Max
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return 0;
                }
                return _linkedProperties[0].Max;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.Max = value;
                }
            }
        }

        public double StepValue
        {
            get
            {
                if (_linkedProperties == null || _linkedProperties.Count == 0)
                {
                    return 0;
                }
                return _linkedProperties[0].StepValue;
            }
            set
            {
                foreach (var prop in _linkedProperties)
                {
                    prop.StepValue = value;
                }
            }
        }
    }
}
