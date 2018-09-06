// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace VanArsdel
{
    public interface IExitTransition
    {
        Task PlayExitTransition();
    }
}