// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace VanArsdel.Utils
{
    public static class MathUtils
    {
        public static byte ClampToByte(double c)
        {
            if (double.IsNaN(c))
            {
                return 0;
            }
            else if (double.IsPositiveInfinity(c))
            {
                return 255;
            }
            else if (double.IsNegativeInfinity(c))
            {
                return 0;
            }
            c = Math.Round(c);
            if (c <= 0)
            {
                return 0;
            }
            else if (c >= 255)
            {
                return 255;
            }
            else
            {
                return (byte)c;
            }
        }
    }
}
