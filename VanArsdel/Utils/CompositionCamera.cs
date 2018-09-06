// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace VanArsdel.Utils
{
    public class CompositionCamera
    {
        public CompositionCamera(Visual cameraVisual, bool activateAnimation = true, bool orthographic = true, float viewportWidth = 1f, float viewportHeight = 1f, float perspectiveDistance = 1f, float yaw = 0f, float pitch = 0f, float roll = 0f)
        {
            if (cameraVisual == null)
            {
                throw new ArgumentNullException("cameraVisual");
            }

            _cameraVisual = cameraVisual;
            _animationActive = activateAnimation;
            _orthographic = orthographic;

            _cameraVisual.Properties.InsertVector2("cameraAnimationViewportSize", new Vector2(viewportWidth, viewportHeight));
            _cameraVisual.Properties.InsertScalar("cameraAnimationPerspectiveDistance", perspectiveDistance);
            _cameraVisual.Properties.InsertVector3("cameraAnimationPosition", new Vector3(0f, 0f, -perspectiveDistance));
            _cameraVisual.Properties.InsertScalar("cameraAnimationYaw", yaw);
            _cameraVisual.Properties.InsertScalar("cameraAnimationPitch", pitch);
            _cameraVisual.Properties.InsertScalar("cameraAnimationRoll", roll);

            BuildExpressionAnimation();
        }

        private static readonly string _expressionPerspective = "Matrix4x4.CreateTranslation(-properties.cameraAnimationPosition) * Matrix4x4.CreateFromAxisAngle(Vector3(0,1,0), properties.cameraAnimationYaw) * Matrix4x4.CreateFromAxisAngle(Vector3(1,0,0), properties.cameraAnimationPitch) * Matrix4x4.CreateFromAxisAngle(Vector3(0,0,1), properties.cameraAnimationRoll) * Matrix4x4.CreateTranslation(Vector3(0, 0, properties.cameraAnimationPerspectiveDistance)) * Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, -1 / properties.cameraAnimationPerspectiveDistance, 0, 0, 0, 1) * Matrix4x4.CreateTranslation(Vector3(properties.cameraAnimationViewportSize.X / 2, properties.cameraAnimationViewportSize.Y / 2, 0))";
        private static readonly string _expressionOrthographic = "Matrix4x4.CreateTranslation(-properties.cameraAnimationPosition) * Matrix4x4.CreateFromAxisAngle(Vector3(0,1,0), properties.cameraAnimationYaw) * Matrix4x4.CreateFromAxisAngle(Vector3(1,0,0), properties.cameraAnimationPitch) * Matrix4x4.CreateFromAxisAngle(Vector3(0,0,1), properties.cameraAnimationRoll) * Matrix4x4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1)";

        private Visual _cameraVisual;

        private ExpressionAnimation _expressionAnimation;
        private void BuildExpressionAnimation()
        {
            if (_expressionAnimation != null)
            {
                _cameraVisual.StopAnimation("transformMatrix");
                _expressionAnimation.Dispose();
            }
            _expressionAnimation = _cameraVisual.Compositor.CreateExpressionAnimation();
            if (_orthographic)
            {
                _expressionAnimation.Expression = _expressionOrthographic;
            }
            else
            {
                _expressionAnimation.Expression = _expressionPerspective;
            }
            _expressionAnimation.SetReferenceParameter("properties", _cameraVisual.Properties);

            if (_animationActive)
            {
                StartAnimation();
            }
        }

        private bool _animationActive;
        public bool AnimationActive
        {
            get { return _animationActive; }
            set
            {
                if (_animationActive != value)
                {
                    _animationActive = value;
                    if (_animationActive)
                    {
                        StartAnimation();
                    }
                    else
                    {
                        StopAnimation();
                    }
                }
            }
        }

        public void StartAnimation()
        {
            _animationActive = true;
            _cameraVisual.StartAnimation("transformMatrix", _expressionAnimation);
        }

        public void StopAnimation()
        {
            _animationActive = false;
            _cameraVisual.StopAnimation("transformMatrix");
        }

        private bool _orthographic;
        public bool Orthographic
        {
            get { return _orthographic; }
            set
            {
                if (_orthographic != value)
                {
                    _orthographic = value;
                    BuildExpressionAnimation();
                }
            }
        }

        public Vector2 ViewportSize
        {
            get
            {
                Vector2 value;
                var status = _cameraVisual.Properties.TryGetVector2("cameraAnimationViewportSize", out value);
                if (status == CompositionGetValueStatus.Succeeded)
                {
                    return value;
                }
                return Vector2.Zero;
            }
            set
            {
                _cameraVisual.Properties.InsertVector2("cameraAnimationViewportSize", value);
            }
        }

        public float PerspectiveDistance
        {
            get
            {
                float value;
                var status = _cameraVisual.Properties.TryGetScalar("cameraAnimationPerspectiveDistance", out value);
                if (status == CompositionGetValueStatus.Succeeded)
                {
                    return value;
                }
                return 1f;
            }
            set
            {
                _cameraVisual.Properties.InsertScalar("cameraAnimationPerspectiveDistance", value);
            }
        }

        public Vector3 Position
        {
            get
            {
                Vector3 value;
                var status = _cameraVisual.Properties.TryGetVector3("cameraAnimationPosition", out value);
                if (status == CompositionGetValueStatus.Succeeded)
                {
                    return value;
                }
                return Vector3.Zero;
            }
            set
            {
                if (_animPosition)
                {
                    var offsetAnim = Window.Current.Compositor.CreateVector3KeyFrameAnimation();
                    offsetAnim.InsertKeyFrame(1f, value, _positionAnimEasing);
                    offsetAnim.Duration = _positionAnimDuration;
                    _cameraVisual.Properties.StartAnimation("cameraAnimationPosition", offsetAnim);
                }
                else
                {
                    _cameraVisual.Properties.InsertVector3("cameraAnimationPosition", value);
                }
            }
        }

        private bool _animPosition = true;
        public bool AnimPosition
        {
            get { return _animPosition; }
            set { _animPosition = value; }
        }

        private TimeSpan _positionAnimDuration = TimeSpan.FromMilliseconds(500);
        public TimeSpan PositionAnimDuration
        {
            get { return _positionAnimDuration; }
            set { _positionAnimDuration = value; }
        }

        private CompositionEasingFunction _positionAnimEasing = Window.Current.Compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));
        public CompositionEasingFunction PositionAnimEasing
        {
            get { return _positionAnimEasing; }
            set { _positionAnimEasing = value; }
        }

        public float Yaw
        {
            get
            {
                float value;
                var status = _cameraVisual.Properties.TryGetScalar("cameraAnimationYaw", out value);
                if (status == CompositionGetValueStatus.Succeeded)
                {
                    return value;
                }
                return 0f;
            }
            set
            {
                _cameraVisual.Properties.InsertScalar("cameraAnimationYaw", value);
            }
        }

        public float Pitch
        {
            get
            {
                float value;
                var status = _cameraVisual.Properties.TryGetScalar("cameraAnimationPitch", out value);
                if (status == CompositionGetValueStatus.Succeeded)
                {
                    return value;
                }
                return 0f;
            }
            set
            {
                _cameraVisual.Properties.InsertScalar("cameraAnimationPitch", value);
            }
        }

        public float Roll
        {
            get
            {
                float value;
                var status = _cameraVisual.Properties.TryGetScalar("cameraAnimationRoll", out value);
                if (status == CompositionGetValueStatus.Succeeded)
                {
                    return value;
                }
                return 0f;
            }
            set
            {
                _cameraVisual.Properties.InsertScalar("cameraAnimationRoll", value);
            }
        }
    }
}
