// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace VanArsdel.Lights
{
    public class Spotlight : XamlLight
    {
        // These properties are set up to make it easier to use from XAML. EG: using double instead of float and splitting up Vector3 into separate properties.

        private static string GetIdStatic(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return typeof(Spotlight).FullName;
            }
            else
            {
                return string.Format("{0}{1}", typeof(Spotlight).FullName, id);
            }
        }

        protected override string GetId()
        {
            return GetIdStatic(LightId);
        }

        #region LightId

        public static readonly DependencyProperty LightIdProperty = DependencyProperty.RegisterAttached("LightId", typeof(string), typeof(Spotlight), new PropertyMetadata(null));

        public string LightId
        {
            get { return GetValue(LightIdProperty) as string; }
            set { SetValue(LightIdProperty, value); }
        }

        #endregion

        #region TargetIdProperty

        // Register an attached property that enables apps to set a UIElement
        // or Brush as a target for this light type in markup.
        public static readonly DependencyProperty TargetIdProperty = DependencyProperty.RegisterAttached("TargetId", typeof(string), typeof(Spotlight), new PropertyMetadata(null, OnTargetIdPropertyChanged));

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

        #region ConstantAttenuationProperty

        public static DependencyProperty ConstantAttenuationProperty = DependencyProperty.Register("ConstantAttenuation", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnConstantAttenuationPropertyChanged));

        private static void OnConstantAttenuationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnConstantAttenuationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnConstantAttenuationChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.ConstantAttenuation = (float)newValue;
            }
        }

        public double ConstantAttenuation
        {
            get { return (double)GetValue(ConstantAttenuationProperty); }
            set { SetValue(ConstantAttenuationProperty, value); }
        }

        #endregion

        #region LinearAttenuationProperty

        public static DependencyProperty LinearAttenuationProperty = DependencyProperty.Register("LinearAttenuation", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnLinearAttenuationPropertyChanged));

        private static void OnLinearAttenuationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnLinearAttenuationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnLinearAttenuationChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.LinearAttenuation = (float)newValue;
            }
        }

        public double LinearAttenuation
        {
            get { return (double)GetValue(LinearAttenuationProperty); }
            set { SetValue(LinearAttenuationProperty, value); }
        }

        #endregion

        #region QuadraticAttenuationProperty

        public static DependencyProperty QuadraticAttenuationProperty = DependencyProperty.Register("QuadraticAttenuation", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnQuadraticAttenuationPropertyChanged));

        private static void OnQuadraticAttenuationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnQuadraticAttenuationChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnQuadraticAttenuationChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.QuadraticAttenuation = (float)newValue;
            }
        }

        public double QuadraticAttenuation
        {
            get { return (double)GetValue(QuadraticAttenuationProperty); }
            set { SetValue(QuadraticAttenuationProperty, value); }
        }

        #endregion

        #region InnerConeAngleInDegreesProperty

        public static DependencyProperty InnerConeAngleInDegreesProperty = DependencyProperty.Register("InnerConeAngleInDegrees", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnInnerConeAngleInDegreesPropertyChanged));

        private static void OnInnerConeAngleInDegreesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnInnerConeAngleInDegreesChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnInnerConeAngleInDegreesChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.InnerConeAngleInDegrees = (float)newValue;
            }
        }

        public double InnerConeAngleInDegrees
        {
            get { return (double)GetValue(InnerConeAngleInDegreesProperty); }
            set { SetValue(InnerConeAngleInDegreesProperty, value); }
        }

        #endregion

        #region InnerConeColorProperty

        public static DependencyProperty InnerConeColorProperty = DependencyProperty.Register("InnerConeColor", typeof(Color), typeof(Spotlight), new PropertyMetadata(Colors.White, OnInnerConeColorPropertyChanged));

        private static void OnInnerConeColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnInnerConeColorChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnInnerConeColorChanged(Color oldValue, Color newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.InnerConeColor = newValue;
            }
        }

        public Color InnerConeColor
        {
            get { return (Color)GetValue(InnerConeColorProperty); }
            set { SetValue(InnerConeColorProperty, value); }
        }

        #endregion

        #region InnerConeIntensityProperty

        public static DependencyProperty InnerConeIntensityProperty = DependencyProperty.Register("InnerConeIntensity", typeof(double), typeof(Spotlight), new PropertyMetadata((double)1, OnInnerConeIntensityPropertyChanged));

        private static void OnInnerConeIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnInnerConeIntensityChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnInnerConeIntensityChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.InnerConeIntensity = (float)newValue;
            }
        }

        public double InnerConeIntensity
        {
            get { return (double)GetValue(InnerConeIntensityProperty); }
            set { SetValue(InnerConeIntensityProperty, value); }
        }

        #endregion

        #region OuterConeAngleInDegreesProperty

        public static DependencyProperty OuterConeAngleInDegreesProperty = DependencyProperty.Register("OuterConeAngleInDegrees", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnOuterConeAngleInDegreesPropertyChanged));

        private static void OnOuterConeAngleInDegreesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OuterConeAngleInDegreesChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OuterConeAngleInDegreesChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.OuterConeAngleInDegrees = (float)newValue;
            }
        }

        public double OuterConeAngleInDegrees
        {
            get { return (double)GetValue(OuterConeAngleInDegreesProperty); }
            set { SetValue(OuterConeAngleInDegreesProperty, value); }
        }

        #endregion

        #region OuterConeColorProperty

        public static DependencyProperty OuterConeColorProperty = DependencyProperty.Register("OuterConeColor", typeof(Color), typeof(Spotlight), new PropertyMetadata(Colors.White, OnOuterConeColorPropertyChanged));

        private static void OnOuterConeColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnOuterConeColorChanged((Color)e.OldValue, (Color)e.NewValue);
            }
        }

        private void OnOuterConeColorChanged(Color oldValue, Color newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.OuterConeColor = newValue;
            }
        }

        public Color OuterConeColor
        {
            get { return (Color)GetValue(OuterConeColorProperty); }
            set { SetValue(OuterConeColorProperty, value); }
        }

        #endregion

        #region OuterConeIntensityProperty

        public static DependencyProperty OuterConeIntensityProperty = DependencyProperty.Register("OuterConeIntensity", typeof(double), typeof(Spotlight), new PropertyMetadata((double)1, OnOuterConeIntensityPropertyChanged));

        private static void OnOuterConeIntensityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnOuterConeIntensityChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOuterConeIntensityChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                spotlight.OuterConeIntensity = (float)newValue;
            }
        }

        public double OuterConeIntensity
        {
            get { return (double)GetValue(OuterConeIntensityProperty); }
            set { SetValue(OuterConeIntensityProperty, value); }
        }

        #endregion

        #region CoordinateSpaceUIElementProperty

        public static readonly DependencyProperty CoordinateSpaceUIElementProperty = DependencyProperty.Register("CoordinateSpaceUIElement", typeof(UIElement), typeof(Spotlight), new PropertyMetadata(null, OnCoordinateSpaceUIElementPropertyChanged));

        private static void OnCoordinateSpaceUIElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.CoordinateSpaceUIElementChanged(e.OldValue as UIElement, e.NewValue as UIElement);
            }
        }

        private void CoordinateSpaceUIElementChanged(UIElement oldValue, UIElement newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                if (newValue == null)
                {
                    spotlight.CoordinateSpace = null;
                }
                else
                {
                    spotlight.CoordinateSpace = ElementCompositionPreview.GetElementVisual(newValue);
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

        public static readonly DependencyProperty DirectionXProperty = DependencyProperty.Register("DirectionX", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnDirectionXPropertyChanged));

        private static void OnDirectionXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnDirectionXChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnDirectionXChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                var oldDirection = spotlight.Direction;
                spotlight.Direction = new Vector3((float)newValue, oldDirection.Y, oldDirection.Z);
            }
        }

        public double DirectionX
        {
            get { return (double)GetValue(DirectionXProperty); }
            set { SetValue(DirectionXProperty, value); }
        }

        #endregion

        #region DirectionYProperty

        public static readonly DependencyProperty DirectionYProperty = DependencyProperty.Register("DirectionY", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnDirectionYPropertyChanged));

        private static void OnDirectionYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnDirectionYChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnDirectionYChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                var oldDirection = spotlight.Direction;
                spotlight.Direction = new Vector3(oldDirection.X, (float)newValue, oldDirection.Z);
            }
        }

        public double DirectionY
        {
            get { return (double)GetValue(DirectionYProperty); }
            set { SetValue(DirectionYProperty, value); }
        }

        #endregion

        #region DirectionZProperty

        public static readonly DependencyProperty DirectionZProperty = DependencyProperty.Register("DirectionZ", typeof(double), typeof(Spotlight), new PropertyMetadata((double)-1, OnDirectionZPropertyChanged));

        private static void OnDirectionZPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnDirectionZChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnDirectionZChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                var oldDirection = spotlight.Direction;
                spotlight.Direction = new Vector3(oldDirection.X, oldDirection.Y, (float)newValue);
            }
        }

        public double DirectionZ
        {
            get { return (double)GetValue(DirectionZProperty); }
            set { SetValue(DirectionZProperty, value); }
        }

        #endregion

        #region OffsetXProperty

        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnOffsetXPropertyChanged));

        private static void OnOffsetXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnOffsetXChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOffsetXChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                var oldOffset = spotlight.Offset;
                spotlight.Offset = new Vector3((float)newValue, oldOffset.Y, oldOffset.Z);
            }
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        #endregion

        #region OffsetYProperty

        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(Spotlight), new PropertyMetadata((double)0, OnOffsetYPropertyChanged));

        private static void OnOffsetYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnOffsetYChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOffsetYChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                var oldOffset = spotlight.Offset;
                spotlight.Offset = new Vector3(oldOffset.X, (float)newValue, oldOffset.Z);
            }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        #endregion

        #region OffsetZProperty

        public static readonly DependencyProperty OffsetZProperty = DependencyProperty.Register("OffsetZ", typeof(double), typeof(Spotlight), new PropertyMetadata((double)100, OnOffsetZPropertyChanged));

        private static void OnOffsetZPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Spotlight target)
            {
                target.OnOffsetZChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private void OnOffsetZChanged(double oldValue, double newValue)
        {
            if (CompositionLight is Windows.UI.Composition.SpotLight spotlight)
            {
                var oldOffset = spotlight.Offset;
                spotlight.Offset = new Vector3(oldOffset.X, oldOffset.Y, (float)newValue);
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
                var spotlight = Window.Current.Compositor.CreateSpotLight();
                spotlight.ConstantAttenuation = (float)ConstantAttenuation;
                spotlight.LinearAttenuation = (float)LinearAttenuation;
                spotlight.QuadraticAttenuation = (float)QuadraticAttenuation;
                spotlight.InnerConeAngleInDegrees = (float)InnerConeAngleInDegrees;
                spotlight.InnerConeColor = InnerConeColor;
                spotlight.InnerConeIntensity = (float)InnerConeIntensity;
                spotlight.OuterConeAngleInDegrees = (float)OuterConeAngleInDegrees;
                spotlight.OuterConeColor = OuterConeColor;
                spotlight.OuterConeIntensity = (float)OuterConeIntensity;
                var element = CoordinateSpaceUIElement;
                if(element != null)
                {
                    spotlight.CoordinateSpace = ElementCompositionPreview.GetElementVisual(element);
                }
                spotlight.Direction = new Vector3((float)DirectionX, (float)DirectionY, (float)DirectionZ);
                spotlight.Offset = new Vector3((float)OffsetX, (float)OffsetY, (float)OffsetZ);

                CompositionLight = spotlight;
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
