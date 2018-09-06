// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;

namespace VanArsdel.Devices
{
    public class PresetItem
    {
        public static List<PresetItem> ParsePresetItemList(JsonArray data)
        {
            var retVal = new List<PresetItem>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParsePresetItem(node.GetObject()));
            }

            return retVal;
        }

        public static PresetItem ParsePresetItem(JsonObject data)
        {
            return new PresetItem(data["PropertyId"].GetString(), data["Value"].GetString());
        }

        public PresetItem(string propertyId, string value)
        {
            _propertyId = propertyId;
            _value = value;
        }

        private readonly string _propertyId;
        public string PropertyId
        {

            get { return _propertyId; }
        }

        private readonly string _value;
        public string Value
        {
            get { return _value; }
        }
    }
}
