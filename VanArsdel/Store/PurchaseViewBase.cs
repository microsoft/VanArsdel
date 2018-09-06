// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VanArsdel
{
    public abstract class PurchaseViewBase : UserControl, IExitTransition
    {
        public abstract Task PlayExitTransition();
    }
}
