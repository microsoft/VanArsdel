// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Windows.Data.Json;
using Windows.Foundation;

namespace VanArsdel.Devices
{
    public static class PropertyMetadataExtensions
    {
        public static List<PropertyMetadataItem> Clone(this IReadOnlyList<PropertyMetadataItem> list)
        {
            if (list == null)
            {
                return null;
            }
            List<PropertyMetadataItem> retVal = new List<PropertyMetadataItem>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                retVal.Add(list[i].Clone());
            }
            return retVal;
        }

        public static bool HasProperty(this IReadOnlyList<PropertyMetadataItem> list, string id)
        {
            if (list == null)
            {
                return false;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static double GetDouble(this IReadOnlyList<PropertyMetadataItem> list, string id)
        {
            if (list == null)
            {
                return default(double);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    double retVal;
                    if (double.TryParse(list[i].Value, out retVal))
                    {
                        return retVal;
                    }
                    else
                    {
                        return default(double);
                    }
                }
            }
            return default(double);
        }

        public static float GetFloat(this IReadOnlyList<PropertyMetadataItem> list, string id)
        {
            if (list == null)
            {
                return default(float);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    float retVal;
                    if (float.TryParse(list[i].Value, out retVal))
                    {
                        return retVal;
                    }
                    else
                    {
                        return default(float);
                    }
                }
            }
            return default(float);
        }

        public static bool GetBool(this IReadOnlyList<PropertyMetadataItem> list, string id)
        {
            if (list == null)
            {
                return default(bool);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    bool retVal;
                    if (bool.TryParse(list[i].Value, out retVal))
                    {
                        return retVal;
                    }
                    else
                    {
                        return default(bool);
                    }
                }
            }
            return default(bool);
        }

        public static string GetString(this IReadOnlyList<PropertyMetadataItem> list, string id)
        {
            if (list == null)
            {
                return null;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    return list[i].Value;
                }
            }
            return null;
        }

        public static Size GetSize(this IReadOnlyList<PropertyMetadataItem> list, string id)
        {
            if (list == null)
            {
                return default(Size);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == id)
                {
                    if (list[i].Value == null)
                    {
                        return default(Size);
                    }
                    string[] rawSize = list[i].Value.ToLower().Split('x');
                    if (rawSize == null || rawSize.Length != 2)
                    {
                        return default(Size);
                    }
                    double width = 0, height = 0;
                    if (!double.TryParse(rawSize[0], out width))
                    {
                        width = 0;
                    }
                    if (!double.TryParse(rawSize[1], out height))
                    {
                        height = 0;
                    }
                    return new Size(width, height);
                }
            }
            return default(Size);
        }
    }

    public class PropertyMetadataItem
    {
        public static List<PropertyMetadataItem> ParsePropertyMetadataItemList(JsonArray data)
        {
            var retVal = new List<PropertyMetadataItem>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParsePropertyMetadataItem(node.GetObject()));
            }

            return retVal;
        }

        public static PropertyMetadataItem ParsePropertyMetadataItem(JsonObject data)
        {
            return new PropertyMetadataItem(data["Id"].GetString(), data["Value"].GetString());
        }

        public PropertyMetadataItem(string id, string value)
        {
            _id = id;
            _value = value;
        }

        private readonly string _id;
        public string Id
        {
            get { return _id; }
        }

        private readonly string _value;
        public string Value
        {
            get { return _value; }
        }

        public PropertyMetadataItem Clone()
        {
            return new PropertyMetadataItem(_id, _value);
        }
    }
}
