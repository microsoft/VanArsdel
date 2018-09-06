// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace VanArsdel.Lights
{
    public class DistantLight : XamlLight
    {
        // These properties are set up to make it easier to use from XAML. EG: using double instead of float and splitting up Vector3 into separate properties.

        private static string GetIdStatic(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return typeof(DistantLight).FullName;
            }
            else
            {
                return string.Format("{0}{1}", typeof(DistantLight).FullName, id);
            }
        }

        protected override string GetId()
        {
            return GetIdStatic(LightId);
        }

        #region LightId

        public static readonly DependencyProperty LightIdProperty = DependencyProperty.RegisterAttached("LightId", typeof(string), typeof(DistantLight), new PropertyMetadata(null));

        public string LightId
        {
            get { return GetValue(LightIdProperty) as string; }
            set { SetValue(LightIdProperty, value); }
        }

        #endregion

        #region TargetIdProperty

        // Register an attached property that enables apps to set a UIElement
        // or Brush as a target for this light type in markup.
        public static readonly DependencyProperty TargetIdProperty = DependencyProperty.RegisterAttached("TargetId", typeof(string), typeof(DistantLight), new PropertyMetadata(null, OnTargetIdPropertyChanged));

        public static void SetTargetId(DependencyObject target, string value)
        {
            target.SetValue(TargetIdProperty, value);
        }

        public static string GetTargetId(DependencyObject target)
        {
            return target.GetValue(TargetIdProperty) as string;
        }

        // Handle attached property changed to automatically target and untarget UIElements and Brushes.
        private static void OnTargetIdPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            string oldId = e.OldValue as string;
            string newId = e.NewValue as string;

            string[] oldIds = null;
            if (!string.IsNullOrEmpty(oldId))
            {
                oldIds = oldId.Split(',');
            }
            else
            {
                oldIds = new string[0];
            }
            string[] newIds = null;
            if (!string.IsNullOrEmpty(newId))
            {
                newIds = newId.Split(',');
            }
            else
            {
                newIds = new string[0];
            }

            List<string> added = new List<string>();
            List<string> removed = new List<string>();

            foreach (var id in newIds)
            {
                if (!oldIds.Contains(id))
                {
                    added.Add(id);
                }
            }
            foreach (var id in oldIds)
            {
                if (!newIds.Contains(id))
                {
                    removed.Add(id);
                }
            }

            foreach (var id in added)
            {
                if (obj is UIElement)
                {
                    XamlLight.AddTargetElement(GetIdStatic(id), obj as UIElement);
                }
                else if (obj is Brush)
                {
                    XamlLight.AddTargetBrush(GetIdStatic(id), obj as Brush);
                }
            }
            foreach (var id in removed)
            {
                if (obj is UIElement)
                {
                    XamlLight.RemoveTargetElement(GetIdStatic(id), obj as UIElement);
                }
                else if (obj is Brush)
                {
                    XamlLight.RemoveTargetBrush(GetIdStatic(id), obj as Brush);
                }
            }
        }

        #endregion

        #region ColorProperty

        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(DistantLight), new PropertyMetadata(Colors.White, OnColorPropertyChanged));
        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DistantLight target)
            {
                target.OnColorChanged((Color)e.NewValue);
            }
        }

        private void OnColorChanged(Color newValue)
        {
            if (CompositionLight is Windows.UI.Composition.DistantLight distantLight)
            {
                distantLight.Color = newValue;
            }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        #region IntensityProperty

        public static DependencyProperty IntensityProperty = DependencyProperty.Register("Intensity", typeof(double), typeof(DistantLight), new PropertyMetadata((double)1, OnIntensityPropertyChanged));

        private static void OnIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DistantLight target)
            {
                target.OnIntensityChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnIntensityChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.DistantLight distantLight)
            {
                distantLight.Intensity = (float)newValue;
            }
        }

        public double Intensity
        {
            get { return (double)GetValue(IntensityProperty); }
            set { SetValue(IntensityProperty, value); }
        }

        #endregion

        #region CoordinateSpaceUIElementProperty

        public static readonly DependencyProperty CoordinateSpaceUIElementProperty = DependencyProperty.Register("CoordinateSpaceUIElement", typeof(UIElement), typeof(DistantLight), new PropertyMetadata(null, OnCoordinateSpaceUIElementPropertyChanged));

        private static void OnCoordinateSpaceUIElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DistantLight target)
            {
                target.CoordinateSpaceUIElementChanged(e.OldValue as UIElement, e.NewValue as UIElement);
            }
        }

        private void CoordinateSpaceUIElementChanged(UIElement oldValue, UIElement newValue)
        {
            if (CompositionLight is Windows.UI.Composition.DistantLight distantLight)
            {
                if (newValue == null)
                {
                    distantLight.CoordinateSpace = null;
                }
                else
                {
                    distantLight.CoordinateSpace = ElementCompositionPreview.GetElementVisual(newValue);
                }
            }
        }

        public UIElement CoordinateSpaceUIElement
        {
            get { return GetValue(CoordinateSpaceUIElementProperty) as UIElement; }
            set { SetValue(CoordinateSpaceUIElementProperty, value); }
        }

        #endregion

        #region DirectionXProperty

        public static readonly DependencyProperty DirectionXProperty = DependencyProperty.Register("DirectionX", typeof(double), typeof(DistantLight), new PropertyMetadata((double)0, OnDirectionXPropertyChanged));

        private static void OnDirectionXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DistantLight target)
            {
                target.OnDirectionXChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnDirectionXChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.DistantLight distantLight)
            {
                var oldDirection = distantLight.Direction;
                distantLight.Direction = new Vector3((float)newValue, oldDirection.Y, oldDirection.Z);
            }
        }

        public double DirectionX
        {
            get { return (double)GetValue(DirectionXProperty); }
            set { SetValue(DirectionXProperty, value); }
        }

        #endregion

        #region DirectionYProperty

        public static readonly DependencyProperty DirectionYProperty = DependencyProperty.Register("DirectionY", typeof(double), typeof(DistantLight), new PropertyMetadata((double)0, OnDirectionYPropertyChanged));

        private static void OnDirectionYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DistantLight target)
            {
                target.OnDirectionYChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnDirectionYChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.DistantLight distantLight)
            {
                var oldDirection = distantLight.Direction;
                distantLight.Direction = new Vector3(oldDirection.X, (float)newValue, oldDirection.Z);
            }
        }

        public double DirectionY
        {
            get { return (double)GetValue(DirectionYProperty); }
            set { SetValue(DirectionYProperty, value); }
        }

        #endregion

        #region DirectionZProperty

        public static readonly DependencyProperty DirectionZProperty = DependencyProperty.Register("DirectionZ", typeof(double), typeof(DistantLight), new PropertyMetadata((double)-1, OnDirectionZPropertyChanged));

        private static void OnDirectionZPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DistantLight target)
            {
                target.OnDirectionZChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnDirectionZChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.DistantLight distantLight)
            {
                var oldDirection = distantLight.Direction;
                distantLight.Direction = new Vector3(oldDirection.X, oldDirection.Y, (float)newValue);
            }
        }

        public double DirectionZ
        {
            get { return (double)GetValue(DirectionZProperty); }
            set { SetValue(DirectionZProperty, value); }
        }

        #endregion

        protected override void OnConnected(UIElement newElement)
        {
            if (CompositionLight == null)
            {
                // OnConnected is called when the first target UIElement is shown on the screen.
                // This enables delaying composition object creation until it's actually necessary.
                var distantLight = Window.Current.Compositor.CreateDistantLight();
                distantLight.Color = Color;
                distantLight.Intensity = (float)Intensity;
                var element = CoordinateSpaceUIElement;
                if (element != null)
                {
                    distantLight.CoordinateSpace = ElementCompositionPreview.GetElementVisual(element);
                }
                distantLight.Direction = new Vector3((float)DirectionX, (float)DirectionY, (float)DirectionZ);

                CompositionLight = distantLight;
            }
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            // OnDisconnected is called when there are no more target UIElements on the screen.
            // The CompositionLight should be disposed when no longer required.
            if (CompositionLight != null)
            {
                CompositionLight.Dispose();
                CompositionLight = null;
            }
        }
    }
}
