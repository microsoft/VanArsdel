// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace VanArsdel.Devices
{
    public static class PropertyForwarderFactory
    {
        // Find those properties which all of the devices have in common and create a list of forwarders linked to those properties
        public static IReadOnlyList<IProperty> GenerateForwardersFromDevices(IReadOnlyList<Device> devices)
        {
            if (devices == null)
            {
                return new List<IProperty>();
            }

            if (devices.Count == 1)
            {
                return devices[0].Properties;
            }

            List<string> commonPropertyIds = null;

            // Find list of property ids which are present in all devices
            foreach (var device in devices)
            {
                if (commonPropertyIds == null)
                {
                    commonPropertyIds = new List<string>();
                    foreach (var prop in device.Properties)
                    {
                        commonPropertyIds.Add(prop.Id);
                    }
                }
                else
                {
                    List<string> pruneList = new List<string>();
                    foreach (string id in commonPropertyIds)
                    {
                        bool found = false;
                        foreach (var prop in device.Properties)
                        {
                            if (prop.Id == id)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            pruneList.Add(id);
                        }
                    }
                    commonPropertyIds.RemoveAll((a) => pruneList.Contains(a));
                }
            }

            if (commonPropertyIds == null || commonPropertyIds.Count == 0)
            {
                return new List<IProperty>();
            }

            var retVal = new List<IProperty>(commonPropertyIds.Count);

            foreach (var id in commonPropertyIds)
            {
                IProperty[] propList = new IProperty[devices.Count];
                for (int i = 0; i < devices.Count; i++)
                {
                    propList[i] = devices[i].GetPropertyById(id);
                }

                switch (propList[0])
                {
                    case IPropertyBool b:
                        retVal.Add(new PropertyBoolForwarder(id, propList));
                        break;
                    case IPropertyColor c:
                        retVal.Add(new PropertyColorForwarder(id, propList));
                        break;
                    case IPropertyList l:
                        retVal.Add(new PropertyListForwarder(id, propList));
                        break;
                    case IPropertyNumber n:
                        retVal.Add(new PropertyNumberForwarder(id, propList));
                        break;
                    case IPropertyColorPalette p:
                        retVal.Add(new PropertyColorPaletteForwarder(id, propList));
                        break;
                    case IPropertyBitmapPicker bm:
                        retVal.Add(new PropertyBitmapPickerForwarder(id, propList));
                        break;
                    case IPropertyString s:
                        retVal.Add(new PropertyStringForwarder(id, propList));
                        break;
                }
            }

            return retVal; ;
        }
    }
}
