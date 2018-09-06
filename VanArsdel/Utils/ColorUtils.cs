// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Markup;

namespace VanArsdel.Utils
{
    public static class ColorUtils
    {
        public const int DefaultRoundingPrecision = 5;

        // This ignores the Alpha channel of the input color
        public static double RGBToLuminance(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return RGBToLuminance(new NormalizedRGB(rgb, false), round, roundingPrecision);
        }

        public static double RGBToLuminance(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double LuminanceHelper(double i)
            {
                if (i <= 0.03928)
                {
                    return i / 12.92;
                }
                else
                {
                    return Math.Pow((i + 0.055) / 1.055, 2.4);
                }
            }
            double r = LuminanceHelper(rgb.R);
            double g = LuminanceHelper(rgb.G);
            double b = LuminanceHelper(rgb.B);

            // More accurate constants would be helpful here
            double l = r * 0.2126 + g * 0.7152 + b * 0.0722;

            if (round)
            {
                return Math.Round(l, roundingPrecision);
            }
            else
            {
                return l;
            }
        }

        // This ignores the Alpha channel of both input colors
        public static double ContrastRatio(Color a, Color b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            return ContrastRatio(new NormalizedRGB(a, false), new NormalizedRGB(b, false), round, roundingPrecision);
        }

        public static double ContrastRatio(in NormalizedRGB a, in NormalizedRGB b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
        {
            double la = RGBToLuminance(a, false);
            double lb = RGBToLuminance(b, false);
            double retVal = 0;
            if (la > lb)
            {
                retVal = (la + 0.05) / (lb + 0.05);
            }
            else
            {
                retVal = (lb + 0.05) / (la + 0.05);
            }

            if (round)
            {
                return Math.Round(retVal, roundingPrecision);
            }
            else
            {
                return retVal;
            }
        }

        // Returns the Color from colorOptions which has the best contrast ratio with background
        // in the case of ties the first color to appear in the list will be chosen
        // alpha channels are ignored
        public static Color ChooseColorForContrast(IEnumerable<Color> colorOptions, Color background)
        {
            if (colorOptions == null)
            {
                throw new ArgumentNullException("colorOptions");
            }
            Color bestColor = default(Color);
            double bestRatio = 0;
            foreach (var c in colorOptions)
            {
                double ratio = ContrastRatio(c, background, false);
                if (ratio > bestRatio)
                {
                    bestColor = c;
                    bestRatio = ratio;
                }
            }

            return bestColor;
        }

        public static Color ParseColorString(string input)
        {
            Color retVal;
            if(TryParseColorString(input, out retVal))
            {
                return retVal;
            }
            else
            {
                return default(Color);
            }
        }

        public static bool TryParseColorString(string input, out Color retVal)
        {
            if (input.StartsWith("#"))
            {
                UInt32 raw;
                if (UInt32.TryParse(input.Substring(1), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out raw))
                {
                    if (input.Length == 7)
                    {
                        raw |= 0xFF000000;
                    }

                    retVal = Color.FromArgb(
                        (byte)((raw & 0xFF000000) >> 24),
                        (byte)((raw & 0x00FF0000) >> 16),
                        (byte)((raw & 0x0000FF00) >> 8),
                        (byte)(raw & 0x000000FF));
                    return true;
                }
                else
                {
                    retVal = default(Color);
                    return false;
                }
            }
            else
            {
                string inputLower = input.ToLowerInvariant();
                switch (inputLower)
                {
                    case "aliceblue":
                        retVal = Colors.AliceBlue;
                        return true;
                    case "antiquewhite":
                        retVal = Colors.AntiqueWhite;
                        return true;
                    case "aqua":
                        retVal = Colors.Aqua;
                        return true;
                    case "aquamarine":
                        retVal = Colors.Aquamarine;
                        return true;
                    case "azure":
                        retVal = Colors.Azure;
                        return true;
                    case "beige":
                        retVal = Colors.Beige;
                        return true;
                    case "bisque":
                        retVal = Colors.Bisque;
                        return true;
                    case "black":
                        retVal = Colors.Black;
                        return true;
                    case "blanchedalmond":
                        retVal = Colors.BlanchedAlmond;
                        return true;
                    case "blue":
                        retVal = Colors.Blue;
                        return true;
                    case "blueviolet":
                        retVal = Colors.BlueViolet;
                        return true;
                    case "brown":
                        retVal = Colors.Brown;
                        return true;
                    case "burlywood":
                        retVal = Colors.BurlyWood;
                        return true;
                    case "cadetblue":
                        retVal = Colors.CadetBlue;
                        return true;
                    case "chartreuse":
                        retVal = Colors.Chartreuse;
                        return true;
                    case "chocolate":
                        retVal = Colors.Chocolate;
                        return true;
                    case "coral":
                        retVal = Colors.Coral;
                        return true;
                    case "cornflowerblue":
                        retVal = Colors.CornflowerBlue;
                        return true;
                    case "cornsilk":
                        retVal = Colors.Cornsilk;
                        return true;
                    case "crimson":
                        retVal = Colors.Crimson;
                        return true;
                    case "cyan":
                        retVal = Colors.Cyan;
                        return true;
                    case "darkblue":
                        retVal = Colors.DarkBlue;
                        return true;
                    case "darkcyan":
                        retVal = Colors.DarkCyan;
                        return true;
                    case "darkgoldenrod":
                        retVal = Colors.DarkGoldenrod;
                        return true;
                    case "darkgray":
                        retVal = Colors.DarkGray;
                        return true;
                    case "darkgreen":
                        retVal = Colors.DarkGreen;
                        return true;
                    case "darkkhaki":
                        retVal = Colors.DarkKhaki;
                        return true;
                    case "darkmagenta":
                        retVal = Colors.DarkMagenta;
                        return true;
                    case "darkolivegreen":
                        retVal = Colors.DarkOliveGreen;
                        return true;
                    case "darkorange":
                        retVal = Colors.DarkOrange;
                        return true;
                    case "darkorchid":
                        retVal = Colors.DarkOrchid;
                        return true;
                    case "darkred":
                        retVal = Colors.DarkRed;
                        return true;
                    case "darksalmon":
                        retVal = Colors.DarkSalmon;
                        return true;
                    case "darkseagreen":
                        retVal = Colors.DarkSeaGreen;
                        return true;
                    case "darkslateblue":
                        retVal = Colors.DarkSlateBlue;
                        return true;
                    case "darkslategray":
                        retVal = Colors.DarkSlateGray;
                        return true;
                    case "darkturquoise":
                        retVal = Colors.DarkTurquoise;
                        return true;
                    case "darkviolet":
                        retVal = Colors.DarkViolet;
                        return true;
                    case "deeppink":
                        retVal = Colors.DeepPink;
                        return true;
                    case "deepskyblue":
                        retVal = Colors.DeepSkyBlue;
                        return true;
                    case "dimgray":
                        retVal = Colors.DimGray;
                        return true;
                    case "dodgerblue":
                        retVal = Colors.DodgerBlue;
                        return true;
                    case "firebrick":
                        retVal = Colors.Firebrick;
                        return true;
                    case "floralwhite":
                        retVal = Colors.FloralWhite;
                        return true;
                    case "forestgreen":
                        retVal = Colors.ForestGreen;
                        return true;
                    case "fuchsia":
                        retVal = Colors.Fuchsia;
                        return true;
                    case "gainsboro":
                        retVal = Colors.Gainsboro;
                        return true;
                    case "ghostwhite":
                        retVal = Colors.GhostWhite;
                        return true;
                    case "gold":
                        retVal = Colors.Gold;
                        return true;
                    case "goldenrod":
                        retVal = Colors.Goldenrod;
                        return true;
                    case "gray":
                        retVal = Colors.Gray;
                        return true;
                    case "green":
                        retVal = Colors.Green;
                        return true;
                    case "greenyellow":
                        retVal = Colors.GreenYellow;
                        return true;
                    case "honeydew":
                        retVal = Colors.Honeydew;
                        return true;
                    case "hotpink":
                        retVal = Colors.HotPink;
                        return true;
                    case "indianred":
                        retVal = Colors.IndianRed;
                        return true;
                    case "indigo":
                        retVal = Colors.Indigo;
                        return true;
                    case "ivory":
                        retVal = Colors.Ivory;
                        return true;
                    case "khaki":
                        retVal = Colors.Khaki;
                        return true;
                    case "lavender":
                        retVal = Colors.Lavender;
                        return true;
                    case "lavenderblush":
                        retVal = Colors.LavenderBlush;
                        return true;
                    case "lawngreen":
                        retVal = Colors.LawnGreen;
                        return true;
                    case "lemonchiffon":
                        retVal = Colors.LemonChiffon;
                        return true;
                    case "lightblue":
                        retVal = Colors.LightBlue;
                        return true;
                    case "lightcoral":
                        retVal = Colors.LightCoral;
                        return true;
                    case "lightcyan":
                        retVal = Colors.LightCyan;
                        return true;
                    case "lightgoldenrodyellow":
                        retVal = Colors.LightGoldenrodYellow;
                        return true;
                    case "lightgray":
                        retVal = Colors.LightGray;
                        return true;
                    case "lightgreen":
                        retVal = Colors.LightGreen;
                        return true;
                    case "lightpink":
                        retVal = Colors.LightPink;
                        return true;
                    case "lightsalmon":
                        retVal = Colors.LightSalmon;
                        return true;
                    case "lightseagreen":
                        retVal = Colors.LightSeaGreen;
                        return true;
                    case "lightskyblue":
                        retVal = Colors.LightSkyBlue;
                        return true;
                    case "lightslategray":
                        retVal = Colors.LightSlateGray;
                        return true;
                    case "lightsteelblue":
                        retVal = Colors.LightSteelBlue;
                        return true;
                    case "lightyellow":
                        retVal = Colors.LightYellow;
                        return true;
                    case "lime":
                        retVal = Colors.Lime;
                        return true;
                    case "limegreen":
                        retVal = Colors.LimeGreen;
                        return true;
                    case "linen":
                        retVal = Colors.Linen;
                        return true;
                    case "magenta":
                        retVal = Colors.Magenta;
                        return true;
                    case "maroon":
                        retVal = Colors.Maroon;
                        return true;
                    case "mediumaquamarine":
                        retVal = Colors.MediumAquamarine;
                        return true;
                    case "mediumblue":
                        retVal = Colors.MediumBlue;
                        return true;
                    case "mediumorchid":
                        retVal = Colors.MediumOrchid;
                        return true;
                    case "mediumpurple":
                        retVal = Colors.MediumPurple;
                        return true;
                    case "mediumseagreen":
                        retVal = Colors.MediumSeaGreen;
                        return true;
                    case "mediumslateblue":
                        retVal = Colors.MediumSlateBlue;
                        return true;
                    case "mediumspringgreen":
                        retVal = Colors.MediumSpringGreen;
                        return true;
                    case "mediumturquoise":
                        retVal = Colors.MediumTurquoise;
                        return true;
                    case "mediumvioletred":
                        retVal = Colors.MediumVioletRed;
                        return true;
                    case "midnightblue":
                        retVal = Colors.MidnightBlue;
                        return true;
                    case "mintcream":
                        retVal = Colors.MintCream;
                        return true;
                    case "mistyrose":
                        retVal = Colors.MistyRose;
                        return true;
                    case "moccasin":
                        retVal = Colors.Moccasin;
                        return true;
                    case "navajowhite":
                        retVal = Colors.NavajoWhite;
                        return true;
                    case "navy":
                        retVal = Colors.Navy;
                        return true;
                    case "oldlace":
                        retVal = Colors.OldLace;
                        return true;
                    case "olive":
                        retVal = Colors.Olive;
                        return true;
                    case "olivedrab":
                        retVal = Colors.OliveDrab;
                        return true;
                    case "orange":
                        retVal = Colors.Orange;
                        return true;
                    case "orangered":
                        retVal = Colors.OrangeRed;
                        return true;
                    case "orchid":
                        retVal = Colors.Orchid;
                        return true;
                    case "palegoldenrod":
                        retVal = Colors.PaleGoldenrod;
                        return true;
                    case "palegreen":
                        retVal = Colors.PaleGreen;
                        return true;
                    case "paleturquoise":
                        retVal = Colors.PaleTurquoise;
                        return true;
                    case "palevioletred":
                        retVal = Colors.PaleVioletRed;
                        return true;
                    case "papayawhip":
                        retVal = Colors.PapayaWhip;
                        return true;
                    case "peachpuff":
                        retVal = Colors.PeachPuff;
                        return true;
                    case "peru":
                        retVal = Colors.Peru;
                        return true;
                    case "pink":
                        retVal = Colors.Pink;
                        return true;
                    case "plum":
                        retVal = Colors.Plum;
                        return true;
                    case "powderblue":
                        retVal = Colors.PowderBlue;
                        return true;
                    case "purple":
                        retVal = Colors.Purple;
                        return true;
                    case "red":
                        retVal = Colors.Red;
                        return true;
                    case "rosybrown":
                        retVal = Colors.RosyBrown;
                        return true;
                    case "royalblue":
                        retVal = Colors.RoyalBlue;
                        return true;
                    case "saddlebrown":
                        retVal = Colors.SaddleBrown;
                        return true;
                    case "salmon":
                        retVal = Colors.Salmon;
                        return true;
                    case "sandybrown":
                        retVal = Colors.SandyBrown;
                        return true;
                    case "seagreen":
                        retVal = Colors.SeaGreen;
                        return true;
                    case "seashell":
                        retVal = Colors.SeaShell;
                        return true;
                    case "sienna":
                        retVal = Colors.Sienna;
                        return true;
                    case "silver":
                        retVal = Colors.Silver;
                        return true;
                    case "skyblue":
                        retVal = Colors.SkyBlue;
                        return true;
                    case "slateblue":
                        retVal = Colors.SlateBlue;
                        return true;
                    case "slategray":
                        retVal = Colors.SlateGray;
                        return true;
                    case "snow":
                        retVal = Colors.Snow;
                        return true;
                    case "springgreen":
                        retVal = Colors.SpringGreen;
                        return true;
                    case "steelblue":
                        retVal = Colors.SteelBlue;
                        return true;
                    case "tan":
                        retVal = Colors.Tan;
                        return true;
                    case "teal":
                        retVal = Colors.Teal;
                        return true;
                    case "thistle":
                        retVal = Colors.Thistle;
                        return true;
                    case "tomato":
                        retVal = Colors.Tomato;
                        return true;
                    case "transparent":
                        retVal = Colors.Transparent;
                        return true;
                    case "turquoise":
                        retVal = Colors.Turquoise;
                        return true;
                    case "violet":
                        retVal = Colors.Violet;
                        return true;
                    case "wheat":
                        retVal = Colors.Wheat;
                        return true;
                    case "white":
                        retVal = Colors.White;
                        return true;
                    case "whitesmoke":
                        retVal = Colors.WhiteSmoke;
                        return true;
                    case "yellow":
                        retVal = Colors.Yellow;
                        return true;
                    case "yellowgreen":
                        retVal = Colors.YellowGreen;
                        return true;
                }
                retVal = default(Color);
                return false;
            }
        }
    }
}
