// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using VanArsdel.Devices;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace VanArsdel.Model
{
    public class Product
    {
        public static List<Product> ParseProductList(JsonArray data, List<DeviceCategory> categories)
        {
            var retVal = new List<Product>(data.Count);

            foreach (var node in data)
            {
                retVal.Add(ParseProduct(node.GetObject(), categories));
            }

            return retVal;
        }

        public static Product ParseProduct(JsonObject data, List<DeviceCategory> categories)
        {
            string categoryId = data["Category"].GetString();
            DeviceCategory category = categories.FirstOrDefault((a) => a.Id == categoryId);
            if (category == null)
            {
                throw new Exception(string.Format("Unable to locate category with id {0}", categoryId));
            }

            Product retVal = new Product
            {
                Id = data["Id"].GetString(),
                Category = category,
                Caption = data["Caption"].GetString(),
                Thumbnail = data["Thumbnail"].GetString(),
                ImageHeight = data["ImageHeight"].GetNumber(),
                ItemMinWidth = data["ItemMinWidth"].GetNumber(),
                ImageMargin = new Thickness(0, data["ImageTopMargin"].GetNumber(), 0, 0),
                PriceCaption = $"${data["Price"].GetNumber()}"
            };

            return retVal;
        }

        public string Id { get; set; }
        public double ItemMinWidth { get; set; }
        public string Caption { get; set; }
        public DeviceCategory Category { get; set; }
        public string PriceCaption { get; set; }
        public string Thumbnail { get; set; }

        public double ImageHeight { get; set; }
        public Thickness ImageMargin { get; set; }
    }
}