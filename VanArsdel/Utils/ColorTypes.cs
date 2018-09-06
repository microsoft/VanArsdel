// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.UI;

namespace VanArsdel.Utils
{
    // Valid values for each channel are ∈ [0.0,1.0]
    // But sometimes it is useful to allow values outside that range during calculations as long as they are clamped eventually
    public readonly struct NormalizedRGB : IEquatable<NormalizedRGB>
    {
        public const int DefaultRoundingPrecision = 5;

        public NormalizedRGB(double r, double g, double b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            if (round)
            {
                R = Math.Round(r, roundingPrecision);
                G = Math.Round(g, roundingPrecision);
                B = Math.Round(b, roundingPrecision);
            }
            else
            {
                R = r;
                G = g;
                B = b;
            }
        }

        public NormalizedRGB(in Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            if (round)
            {
                R = Math.Round((double)rgb.R / 255.0, roundingPrecision);
                G = Math.Round((double)rgb.G / 255.0, roundingPrecision);
                B = Math.Round((double)rgb.B / 255.0, roundingPrecision);
            }
            else
            {
                R = (double)rgb.R / 255.0;
                G = (double)rgb.G / 255.0;
                B = (double)rgb.B / 255.0;
            }
        }

        public Color Denormalize(byte a = 255)
        {
            return Color.FromArgb(a, MathUtils.ClampToByte(R * 255.0), MathUtils.ClampToByte(G * 255.0), MathUtils.ClampToByte(B * 255.0));
        }

        public readonly double R;
        public readonly double G;
        public readonly double B;

        #region IEquatable<NormalizedRGB>

        public bool Equals(NormalizedRGB other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        #endregion

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj is NormalizedRGB other)
            {
                return R == other.R && G == other.G && B == other.B;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", R, G, B);
        }

        #endregion
    }
}
