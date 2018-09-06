// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.ApplicationModel.Resources;

namespace VanArsdel
{
    public interface IStringProvider
    {
        string GetString(string id);
    }

    public class StringProvider : IStringProvider
    {
        public StringProvider(ResourceLoader resourceLoader)
        {
            if (resourceLoader == null)
            {
                throw new ArgumentNullException("resourceLoader");
            }
            _resourceLoader = resourceLoader;
        }

        private ResourceLoader _resourceLoader;

        public string GetString(string id)
        {
            return _resourceLoader.GetString(id);
        }
    }
}
