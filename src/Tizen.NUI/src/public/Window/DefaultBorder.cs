/*
 * Copyright(c) 2022 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.ComponentModel;
using Tizen.NUI.BaseComponents;

namespace Tizen.NUI
{
    /// <summary>
    /// This class creates a border UI.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DefaultBorder : IBorderInterface
    {
        #region Constant Fields
        private static readonly string ResourcePath = FrameworkInformation.ResourcePath;
        private static readonly string MinimalizeIcon = ResourcePath + "minimalize.png";
        private static readonly string MaximalizeIcon = ResourcePath + "maximalize.png";
        private static readonly string CloseIcon = ResourcePath + "close.png";
        private static readonly string LeftCornerIcon = ResourcePath + "leftCorner.png";
        private static readonly string RightCornerIcon = ResourcePath + "rightCorner.png";

        private static readonly string DarkMinimalizeIcon = ResourcePath + "dark_minimalize.png";
        private static readonly string DarkPreviousIcon = ResourcePath + "dark_smallwindow.png";
        private static readonly string DarkCloseIcon = ResourcePath + "dark_close.png";
        private static readonly string DarkLeftCornerIcon = ResourcePath + "dark_leftCorner.png";
        private static readonly string DarkRightCornerIcon = ResourcePath + "dark_rightCorner.png";


        private const uint DefaultHeight = 50;
        private const uint DefaultLineThickness = 5;
        private const uint DefaultTouchThickness = 20;
        private static readonly Color DefaultBackgroundColor = new Color(1, 1, 1, 0.3f);
        private static readonly Color DefaultClickedBackgroundColor = new Color(1, 1, 1, 0.4f);
        private static readonly Size2D DefaultMinSize = new Size2D(100, 0);
        #endregion //Constant Fields


        #region Fields
        private Color backgroundColor;
        private View rootView;
        private View borderView;

        private ImageView minimalizeIcon;
        private ImageView maximalizeIcon;
        private ImageView closeIcon;
        private ImageView leftCornerIcon;
        private ImageView rightCornerIcon;

        private Window.BorderDirection direction = Window.BorderDirection.None;
        private float preScale = 0;

        private View windowView = null;
        private bool isWinGestures = false;
        private Timer timer;

        private CurrentGesture currentGesture = CurrentGesture.None;
        private bool disposed = false;
        #endregion //Fields

        #region Events
        private PanGestureDetector borderPanGestureDetector;
        private PinchGestureDetector borderPinchGestureDetector;
        private PanGestureDetector winPanGestureDetector;
        private TapGestureDetector winTapGestureDetector;
        #endregion //Events

        #region Enums
        private enum CurrentGesture
        {
          None = 0,
          TapGesture = 1,
          PanGesture = 2,
          PinchGesture = 3,
        }
        #endregion //Enums

        #region Methods

        /// <summary>
        /// The thickness of the border.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public uint BorderLineThickness {get; set;}

        /// <summary>
        /// The thickness of the border's touch area.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public uint TouchThickness {get; set;}

        /// <summary>
        /// The height of the border.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public uint BorderHeight {get; set;}

        /// <summary>
        /// The minimum size by which the window will small.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size2D MinSize {get; set;}

        /// <summary>
        /// The maximum size by which the window will big.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size2D MaxSize {get; set;}

        /// <summary>
        /// The window with borders added.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Window BorderWindow {get; set;}

        /// <summary>
        /// Whether overlay mode.
        /// If overlay mode is true, the border area is hidden when the window is maximized.
        /// And if you touched at screen, the border area is shown on the screen.
        /// Default value is false;
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool OverlayMode {get; set;}

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DefaultBorder()
        {
            BorderLineThickness = DefaultLineThickness;
            TouchThickness = DefaultTouchThickness;
            BorderHeight = DefaultHeight;
            MinSize = DefaultMinSize;
            OverlayMode = false;
        }


        /// <summary>
        /// Create border UI. Users can override this method to draw border UI.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void CreateBorderView(View rootView)
        {
            if (rootView == null)
            {
                return;
            }
            this.rootView = rootView;
            rootView.BackgroundColor = DefaultBackgroundColor;
            rootView.CornerRadius = new Vector4(0.03f, 0.03f, 0.03f, 0.03f);
            rootView.CornerRadiusPolicy = VisualTransformPolicyType.Relative;

            borderView = new View()
            {
                Layout = new LinearLayout()
                {
                    LinearAlignment = LinearLayout.Alignment.End,
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            minimalizeIcon = new ImageView()
            {
                ResourceUrl = MinimalizeIcon,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomLeft,
                ParentOrigin = ParentOrigin.BottomLeft,
            };

            maximalizeIcon = new ImageView()
            {
                ResourceUrl = MaximalizeIcon,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomLeft,
                ParentOrigin = ParentOrigin.BottomLeft,
            };

            closeIcon = new ImageView()
            {
                ResourceUrl = CloseIcon,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomLeft,
                ParentOrigin = ParentOrigin.BottomLeft,
            };

            leftCornerIcon = new ImageView()
            {
                ResourceUrl = LeftCornerIcon,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomLeft,
                ParentOrigin = ParentOrigin.BottomLeft,
            };

            rightCornerIcon = new ImageView()
            {
              ResourceUrl = RightCornerIcon,
              PositionUsesPivotPoint = true,
              PivotPoint = PivotPoint.BottomLeft,
              ParentOrigin = ParentOrigin.BottomLeft,
            };

            rootView.Add(leftCornerIcon);
            borderView.Add(minimalizeIcon);
            borderView.Add(maximalizeIcon);
            borderView.Add(closeIcon);
            borderView.Add(rightCornerIcon);
            rootView.Add(borderView);

            minimalizeIcon.TouchEvent += OnMinimizeIconTouched;
            maximalizeIcon.TouchEvent += OnMaximizeIconTouched;
            closeIcon.TouchEvent += OnCloseIconTouched;
            leftCornerIcon.TouchEvent += OnLeftCornerIconTouched;
            rightCornerIcon.TouchEvent += OnRightCornerIconTouched;
        }

        /// Determines the behavior of pinch gesture.
        private void OnPinchGestureDetected(object source, PinchGestureDetector.DetectedEventArgs e)
        {
            if (e == null)
            {
                return;
            }
            if (e.PinchGesture.State == Gesture.StateType.Started)
            {
                preScale = e.PinchGesture.Scale;
            }
            else if (e.PinchGesture.State == Gesture.StateType.Finished || e.PinchGesture.State == Gesture.StateType.Cancelled)
            {
                if (preScale > e.PinchGesture.Scale)
                {
                    if (BorderWindow.IsMaximized())
                    {
                        BorderWindow.Maximize(false);
                    }
                    else
                    {
                        BorderWindow.Minimize(true);
                    }
                }
                else
                {
                    BorderWindow.Maximize(true);
                }
            }
        }

        /// Determines the behavior of borders.
        private void OnPanGestureDetected(object source, PanGestureDetector.DetectedEventArgs e)
        {
            if (e == null)
            {
                return;
            }
            PanGesture panGesture = e.PanGesture;

            if (panGesture.State == Gesture.StateType.Started)
            {
                direction = BorderWindow.GetDirection(panGesture.Position.X, panGesture.Position.Y);
                if (direction == Window.BorderDirection.Move)
                {
                    if (BorderWindow.IsMaximized() == true)
                    {
                        BorderWindow.Maximize(false);
                    }
                    else
                    {
                        BorderWindow.RequestMoveToServer();
                    }
                }
                else if (direction != Window.BorderDirection.None)
                {
                    OnRequestResize();
                    BorderWindow.RequestResizeToServer((Window.ResizeDirection)direction);
                }
            }
            else if (panGesture.State == Gesture.StateType.Continuing)
            {
                if (direction == Window.BorderDirection.BottomLeft || direction == Window.BorderDirection.BottomRight || direction == Window.BorderDirection.TopLeft || direction == Window.BorderDirection.TopRight)
                {
                    BorderWindow.WindowSize += new Size2D((int)panGesture.ScreenDisplacement.X, (int)panGesture.ScreenDisplacement.Y);
                }
                else if (direction == Window.BorderDirection.Left || direction == Window.BorderDirection.Right)
                {
                    BorderWindow.WindowSize += new Size2D((int)panGesture.ScreenDisplacement.X, 0);
                }
                else if (direction == Window.BorderDirection.Bottom || direction == Window.BorderDirection.Top)
                {
                    BorderWindow.WindowSize += new Size2D(0, (int)panGesture.ScreenDisplacement.Y);
                }
                else if (direction == Window.BorderDirection.Move)
                {
                    BorderWindow.WindowPosition += new Position2D((int)panGesture.ScreenDisplacement.X, (int)panGesture.ScreenDisplacement.Y);
                }
            }
            else if (panGesture.State == Gesture.StateType.Finished || panGesture.State == Gesture.StateType.Cancelled)
            {
                direction = Window.BorderDirection.None;
                ClearWindowGesture();
            }
        }


        /// <summary>
        /// This is an event callback when the left corner icon is touched.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool OnLeftCornerIconTouched(object sender, View.TouchEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
              ClearWindowGesture();
              OnRequestResize();
              BorderWindow.RequestResizeToServer(Window.ResizeDirection.BottomLeft);
            }
            return true;
        }

        /// <summary>
        ///This is an event callback when the right corner icon is touched.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool OnRightCornerIconTouched(object sender, View.TouchEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
              ClearWindowGesture();
              OnRequestResize();
              BorderWindow.RequestResizeToServer(Window.ResizeDirection.BottomRight);
            }
            return true;
        }


        /// <summary>
        /// This is an event callback when the minimize button is touched.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool OnMinimizeIconTouched(object sender, View.TouchEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                ClearWindowGesture();
                BorderWindow.Minimize(true);
            }
            return true;
        }

        /// <summary>
        /// This is an event callback when the maximum button is touched.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool OnMaximizeIconTouched(object sender, View.TouchEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                ClearWindowGesture();
                if (BorderWindow.IsMaximized())
                {
                  BorderWindow.Maximize(false);
                }
                else
                {
                  BorderWindow.Maximize(true);
                }
            }
            return true;
        }

        /// <summary>
        /// This is an event callback when the close button is touched.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool OnCloseIconTouched(object sender, View.TouchEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                BorderWindow.Destroy();
                BorderWindow = null;
            }
            return true;
        }


        private void UpdateIcons()
        {
            if (BorderWindow != null && rootView != null)
            {
                if (BorderWindow.IsMaximized() == true)
                {
                    if (maximalizeIcon != null)
                    {
                        maximalizeIcon.ResourceUrl = DarkPreviousIcon;
                    }
                    if (minimalizeIcon != null)
                    {
                        minimalizeIcon.ResourceUrl = DarkMinimalizeIcon;
                    }
                    if (closeIcon != null)
                    {
                        closeIcon.ResourceUrl = DarkCloseIcon;
                    }
                    if (leftCornerIcon != null)
                    {
                        leftCornerIcon.ResourceUrl = DarkLeftCornerIcon;
                    }
                    if (rightCornerIcon != null)
                    {
                        rightCornerIcon.ResourceUrl = DarkRightCornerIcon;
                    }
                    rootView.CornerRadius = new Vector4(0, 0, 0, 0);
                    rootView.CornerRadiusPolicy = VisualTransformPolicyType.Relative;
                    BorderWindow.SetTransparency(false);
                }
                else
                {
                    if (maximalizeIcon != null)
                    {
                        maximalizeIcon.ResourceUrl = MaximalizeIcon;
                    }
                    if (minimalizeIcon != null)
                    {
                        minimalizeIcon.ResourceUrl = MinimalizeIcon;
                    }
                    if (closeIcon != null)
                    {
                        closeIcon.ResourceUrl = CloseIcon;
                    }
                    if (leftCornerIcon != null)
                    {
                        leftCornerIcon.ResourceUrl = LeftCornerIcon;
                    }
                    if (rightCornerIcon != null)
                    {
                        rightCornerIcon.ResourceUrl = RightCornerIcon;
                    }
                    rootView.CornerRadius = new Vector4(0.03f, 0.03f, 0.03f, 0.03f);
                    rootView.CornerRadiusPolicy = VisualTransformPolicyType.Relative;
                    BorderWindow.SetTransparency(true);
                }
            }
        }


        /// <summary>
        /// Called after the border UI is created.
        /// </summary>
        /// <param name="rootView">The root view on which the border.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnCreated(View rootView)
        {
            if (rootView == null)
            {
                return;
            }
            // Register to resize and move through pan gestures.
            borderPanGestureDetector = new PanGestureDetector();
            borderPanGestureDetector.Attach(rootView);
            borderPanGestureDetector.Detected += OnPanGestureDetected;

            // Register touch event for effect when border is touched.
            rootView.LeaveRequired = true;
            rootView.TouchEvent += (s, e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Started)
                {
                    backgroundColor = new Color(rootView.BackgroundColor);
                    rootView.BackgroundColor = DefaultClickedBackgroundColor;
                }
                else if (e.Touch.GetState(0) == PointStateType.Finished ||
                         e.Touch.GetState(0) == PointStateType.Leave ||
                         e.Touch.GetState(0) == PointStateType.Interrupted)
                {
                    rootView.BackgroundColor = backgroundColor;
                }
                return true;
            };

            borderPinchGestureDetector = new PinchGestureDetector();
            borderPinchGestureDetector.Attach(rootView);
            borderPinchGestureDetector.Detected += OnPinchGestureDetected;

            AddInterceptGesture();
        }


        // Register an intercept touch event on the window.
        private void AddInterceptGesture()
        {
            isWinGestures = false;
            BorderWindow.InterceptTouchEvent += OnWinInterceptedTouch;
        }

        // Intercept touch on window.
        private bool OnWinInterceptedTouch(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Stationary && e.Touch.GetPointCount() == 2)
            {
                if (isWinGestures == false && timer == null)
                {
                    timer = new Timer(300);
                    timer.Tick += OnTick;
                    timer.Start();
                }
            }
            else
            {
                currentGesture = CurrentGesture.None;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }
            return false;
        }

        // If two finger long press is done, create a windowView.
        // then, Register a gesture on the windowView to do a resize or move.
        private bool OnTick(object o, Timer.TickEventArgs e)
        {
            windowView = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                BackgroundColor = new Color(1, 1, 1, 0.5f),
            };
            windowView.TouchEvent += (s, e) =>
            {
                return true;
            };
            BorderWindow.Add(windowView);

            winTapGestureDetector = new TapGestureDetector();
            winTapGestureDetector.Attach(windowView);
            winTapGestureDetector.SetMaximumTapsRequired(3);
            winTapGestureDetector.Detected += OnWinTapGestureDetected;

            winPanGestureDetector = new PanGestureDetector();
            winPanGestureDetector.Attach(windowView);
            winPanGestureDetector.Detected += OnWinPanGestureDetected;

            BorderWindow.InterceptTouchEvent -= OnWinInterceptedTouch;
            isWinGestures = true;
            return false;
        }

        // Behavior when the window is tapped.
        private void OnWinTapGestureDetected(object source, TapGestureDetector.DetectedEventArgs e)
        {
          if (currentGesture <= CurrentGesture.TapGesture)
          {
              currentGesture = CurrentGesture.TapGesture;
              if (e.TapGesture.NumberOfTaps == 2)
              {
                  if (BorderWindow.IsMaximized() == false)
                  {
                    BorderWindow.Maximize(true);
                  }
                  else
                  {
                    BorderWindow.Maximize(false);
                  }
              }
              else
              {
                  ClearWindowGesture();
              }
          }
        }

        // Window moves through pan gestures.
        private void OnWinPanGestureDetected(object source, PanGestureDetector.DetectedEventArgs e)
        {
            if (currentGesture <= CurrentGesture.PanGesture /*&& panGesture.NumberOfTouches == 1*/)
            {
                PanGesture panGesture = e.PanGesture;

                if (panGesture.State == Gesture.StateType.Started)
                {
                    currentGesture = CurrentGesture.PanGesture;
                    if (BorderWindow.IsMaximized() == true)
                    {
                        BorderWindow.Maximize(false);
                    }
                    else
                    {
                        BorderWindow.RequestMoveToServer();
                    }
                }
                else if (panGesture.State == Gesture.StateType.Finished || panGesture.State == Gesture.StateType.Cancelled)
                {
                    currentGesture = CurrentGesture.None;
                    ClearWindowGesture();
                }
            }
        }

        private void ClearWindowGesture()
        {
            if (isWinGestures)
            {
                winPanGestureDetector.Dispose();
                winTapGestureDetector.Dispose();

                isWinGestures = false;
                BorderWindow.Remove(windowView);
                BorderWindow.InterceptTouchEvent += OnWinInterceptedTouch;
            }
        }

        /// <summary>
        /// Called when requesting a resize
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnRequestResize()
        {
        }

        /// <summary>
        /// Called when the window is resized.
        /// </summary>
        /// <param name="width">The width of the resized window</param>
        /// <param name="height">The height of the resized window</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnResized(int width, int height)
        {
            UpdateIcons();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                ClearWindowGesture();
                if (BorderWindow != null)
                {
                    BorderWindow.InterceptTouchEvent -= OnWinInterceptedTouch;
                }
                borderPanGestureDetector?.Dispose();
                borderPinchGestureDetector?.Dispose();
                backgroundColor?.Dispose();
                minimalizeIcon?.Dispose();
                maximalizeIcon?.Dispose();
                closeIcon?.Dispose();
                leftCornerIcon?.Dispose();
                rightCornerIcon?.Dispose();
                timer?.Dispose();
                windowView?.Dispose();
                borderView?.Dispose();
                rootView?.Dispose();
            }
            disposed = true;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            global::System.GC.SuppressFinalize(this);
        }
        #endregion //Methods

    }
}
