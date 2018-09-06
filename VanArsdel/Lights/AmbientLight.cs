// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace VanArsdel.Lights
{
    public class AmbientLight : XamlLight
    {
        private static string GetIdStatic(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return typeof(AmbientLight).FullName;
            }
            else
            {
                return string.Format("{0}{1}", typeof(AmbientLight).FullName, id);
            }
        }

        protected override string GetId()
        {
            return GetIdStatic(LightId);
        }

        #region LightId

        public static readonly DependencyProperty LightIdProperty = DependencyProperty.RegisterAttached("LightId", typeof(string), typeof(AmbientLight), new PropertyMetadata(null));

        public string LightId
        {
            get { return GetValue(LightIdProperty) as string; }
            set { SetValue(LightIdProperty, value); }
        }

        #endregion

        #region TargetIdProperty

        // Register an attached property that enables apps to set a UIElement
        // or Brush as a target for this light type in markup.
        public static readonly DependencyProperty TargetIdProperty = DependencyProperty.RegisterAttached("TargetId", typeof(string), typeof(AmbientLight), new PropertyMetadata(null, OnTargetIdPropertyChanged));

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

            foreach(var id in added)
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
            foreach(var id in removed)
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

        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(AmbientLight), new PropertyMetadata(Colors.Transparent, OnColorPropertyChanged));
        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AmbientLight target)
            {
                target.OnColorChanged((Color)e.NewValue);
            }
        }

        private void OnColorChanged(Color newValue)
        {
            if (CompositionLight is Windows.UI.Composition.AmbientLight ambientLight)
            {
                ambientLight.Color = newValue;
            }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        protected override void OnConnected(UIElement newElement)
        {
            if (CompositionLight == null)
            {
                // OnConnected is called when the first target UIElement is shown on the screen.
                // This enables delaying composition object creation until it's actually necessary.
                var ambientLight = Window.Current.Compositor.CreateAmbientLight();
                ambientLight.Color = Color;

                CompositionLight = ambientLight;
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
