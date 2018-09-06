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
    public class PointLight : XamlLight
    {
        // These properties are set up to make it easier to use from XAML. EG: using double instead of float and splitting up Vector3 into separate properties.

        private static string GetIdStatic(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return typeof(PointLight).FullName;
            }
            else
            {
                return string.Format("{0}{1}", typeof(PointLight).FullName, id);
            }
        }

        protected override string GetId()
        {
            return GetIdStatic(LightId);
        }

        #region LightId

        public static readonly DependencyProperty LightIdProperty = DependencyProperty.RegisterAttached("LightId", typeof(string), typeof(PointLight), new PropertyMetadata(null));

        public string LightId
        {
            get { return GetValue(LightIdProperty) as string; }
            set { SetValue(LightIdProperty, value); }
        }

        #endregion

        #region TargetIdProperty

        // Register an attached property that enables apps to set a UIElement
        // or Brush as a target for this light type in markup.
        public static readonly DependencyProperty TargetIdProperty = DependencyProperty.RegisterAttached("TargetId", typeof(string), typeof(PointLight), new PropertyMetadata(null, OnTargetIdPropertyChanged));

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

        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(PointLight), new PropertyMetadata(Colors.White, OnColorPropertyChanged));
        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnColorChanged((Color)e.NewValue);
            }
        }

        private void OnColorChanged(Color newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                pointlight.Color = newValue;
            }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        #region ConstantAttenuationProperty

        public static DependencyProperty ConstantAttenuationProperty = DependencyProperty.Register("ConstantAttenuation", typeof(double), typeof(PointLight), new PropertyMetadata((double)0, OnConstantAttenuationPropertyChanged));

        private static void OnConstantAttenuationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnConstantAttenuationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnConstantAttenuationChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                pointlight.ConstantAttenuation = (float)newValue;
            }
        }

        public double ConstantAttenuation
        {
            get { return (double)GetValue(ConstantAttenuationProperty); }
            set { SetValue(ConstantAttenuationProperty, value); }
        }

        #endregion

        #region LinearAttenuationProperty

        public static DependencyProperty LinearAttenuationProperty = DependencyProperty.Register("LinearAttenuation", typeof(double), typeof(PointLight), new PropertyMetadata((double)0, OnLinearAttenuationPropertyChanged));

        private static void OnLinearAttenuationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnLinearAttenuationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnLinearAttenuationChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                pointlight.LinearAttenuation = (float)newValue;
            }
        }

        public double LinearAttenuation
        {
            get { return (double)GetValue(LinearAttenuationProperty); }
            set { SetValue(LinearAttenuationProperty, value); }
        }

        #endregion

        #region QuadraticAttenuationProperty

        public static DependencyProperty QuadraticAttenuationProperty = DependencyProperty.Register("QuadraticAttenuation", typeof(double), typeof(PointLight), new PropertyMetadata((double)0, OnQuadraticAttenuationPropertyChanged));

        private static void OnQuadraticAttenuationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnQuadraticAttenuationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnQuadraticAttenuationChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                pointlight.QuadraticAttenuation = (float)newValue;
            }
        }

        public double QuadraticAttenuation
        {
            get { return (double)GetValue(QuadraticAttenuationProperty); }
            set { SetValue(QuadraticAttenuationProperty, value); }
        }

        #endregion

        #region IntensityProperty

        public static DependencyProperty IntensityProperty = DependencyProperty.Register("Intensity", typeof(double), typeof(PointLight), new PropertyMetadata((double)1, OnIntensityPropertyChanged));

        private static void OnIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnIntensityChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnIntensityChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                pointlight.Intensity = (float)newValue;
            }
        }

        public double Intensity
        {
            get { return (double)GetValue(IntensityProperty); }
            set { SetValue(IntensityProperty, value); }
        }

        #endregion

        #region CoordinateSpaceUIElementProperty

        public static readonly DependencyProperty CoordinateSpaceUIElementProperty = DependencyProperty.Register("CoordinateSpaceUIElement", typeof(UIElement), typeof(PointLight), new PropertyMetadata(null, OnCoordinateSpaceUIElementPropertyChanged));

        private static void OnCoordinateSpaceUIElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.CoordinateSpaceUIElementChanged(e.OldValue as UIElement, e.NewValue as UIElement);
            }
        }

        private void CoordinateSpaceUIElementChanged(UIElement oldValue, UIElement newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                if (newValue == null)
                {
                    pointlight.CoordinateSpace = null;
                }
                else
                {
                    pointlight.CoordinateSpace = ElementCompositionPreview.GetElementVisual(newValue);
                }
            }
        }

        public UIElement CoordinateSpaceUIElement
        {
            get { return GetValue(CoordinateSpaceUIElementProperty) as UIElement; }
            set { SetValue(CoordinateSpaceUIElementProperty, value); }
        }

        #endregion

        #region OffsetXProperty

        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(PointLight), new PropertyMetadata((double)0, OnOffsetXPropertyChanged));

        private static void OnOffsetXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnOffsetXChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOffsetXChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                var oldOffset = pointlight.Offset;
                pointlight.Offset = new Vector3((float)newValue, oldOffset.Y, oldOffset.Z);
            }
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        #endregion

        #region OffsetYProperty

        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(PointLight), new PropertyMetadata((double)0, OnOffsetYPropertyChanged));

        private static void OnOffsetYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnOffsetYChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOffsetYChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                var oldOffset = pointlight.Offset;
                pointlight.Offset = new Vector3(oldOffset.X, (float)newValue, oldOffset.Z);
            }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        #endregion

        #region OffsetZProperty

        public static readonly DependencyProperty OffsetZProperty = DependencyProperty.Register("OffsetZ", typeof(double), typeof(PointLight), new PropertyMetadata((double)100, OnOffsetZPropertyChanged));

        private static void OnOffsetZPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PointLight target)
            {
                target.OnOffsetZChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOffsetZChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.PointLight pointlight)
            {
                var oldOffset = pointlight.Offset;
                pointlight.Offset = new Vector3(oldOffset.X, oldOffset.Y, (float)newValue);
            }
        }

        public double OffsetZ
        {
            get { return (double)GetValue(OffsetZProperty); }
            set { SetValue(OffsetZProperty, value); }
        }

        #endregion

        protected override void OnConnected(UIElement newElement)
        {
            if (CompositionLight == null)
            {
                // OnConnected is called when the first target UIElement is shown on the screen.
                // This enables delaying composition object creation until it's actually necessary.
                var pointlight = Window.Current.Compositor.CreatePointLight();
                pointlight.Color = Color;
                pointlight.ConstantAttenuation = (float)ConstantAttenuation;
                pointlight.LinearAttenuation = (float)LinearAttenuation;
                pointlight.QuadraticAttenuation = (float)QuadraticAttenuation;
                pointlight.Intensity = (float)Intensity;
                var element = CoordinateSpaceUIElement;
                if (element != null)
                {
                    pointlight.CoordinateSpace = ElementCompositionPreview.GetElementVisual(element);
                }
                pointlight.Offset = new Vector3((float)OffsetX, (float)OffsetY, (float)OffsetZ);
                
                CompositionLight = pointlight;
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
