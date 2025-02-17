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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Tizen.NUI.BaseComponents;

namespace Tizen.NUI
{
    public partial class Window
    {
        #region Constant Fields
        #endregion //Constant Fields

        #region Fields
        private IBorderInterface borderInterface = null;
        private Layer borderWindowRootLayer = null;
        private Layer borderWindowBottomLayer = null;
        private bool isBorderWindow = false;
        private bool isInterceptTouch = false;

        private Timer overlayTimer;
        private Color overlayBackgroundColor;

        // for border area
        private View rootView = null;
        #endregion //Fields

        #region Constructors
        #endregion //Constructors

        #region Distructors
        #endregion //Distructors

        #region Delegates
        #endregion //Delegates

        #region Events
        #endregion //Events

        #region Enums
        /// <summary>
        /// This is an enum for the resize direction or move value when the border area is touched.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public enum BorderDirection
        {
            None        = ResizeDirection.None,
            TopLeft     = ResizeDirection.TopLeft,
            Top         = ResizeDirection.Top,
            TopRight    = ResizeDirection.TopRight,
            Left        = ResizeDirection.Left,
            Right       = ResizeDirection.Right,
            BottomLeft  = ResizeDirection.BottomLeft,
            Bottom      = ResizeDirection.Bottom,
            BottomRight = ResizeDirection.BottomRight,
            Move,
        }
        #endregion //Enums

        #region Interfaces
        #endregion //Interfaces

        #region Properties
        /// <summary>
        /// Whether the border is enabled.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsBorderEnabled => isBorderWindow;
        #endregion //Properties

        #region Indexers
        #endregion //Indexers

        #region Methods
        /// <summary>
        /// Enable the border window with IBorderInterface.
        /// This adds a border area to the Window.
        /// The border's UI is configured using IBorderInterface.
        /// Users can reisze and move by touching the border area.
        /// </summary>
        /// <param name="borderInterface">The IBorderInterface.</param>
        /// <returns>Whether the border window is enabled</returns>
        internal bool EnableBorder(IBorderInterface borderInterface = null)
        {
            if (isBorderWindow == true)
            {
                Tizen.Log.Error("NUI", $"Already EnableBorderWindow\n");
                return false;
            }

            if (borderInterface == null)
            {
                borderInterface = new DefaultBorder();
            }
            this.borderInterface = borderInterface;

            GetDefaultLayer().Name = "OriginalRootLayer";

            Resized += OnBorderWindowResized;

            isBorderWindow = true;

            // The current window is as below
            //    *****
            //    *****
            // Increase the window size as much as the border area.
            //  +++++++
            //  +*****+
            //  +*****+
            //  +=====+
            //  +=====+
            // '+' is BorderLineThickness
            // '=' is BorderHeight
            WindowSize += new Size2D((int)borderInterface.BorderLineThickness * 2, (int)(borderInterface.BorderHeight + borderInterface.BorderLineThickness));

            if (CreateBorder() == false)
            {
                WindowSize -= new Size2D((int)borderInterface.BorderLineThickness * 2, (int)(borderInterface.BorderHeight + borderInterface.BorderLineThickness));
                Resized -= OnBorderWindowResized;
                isBorderWindow = false;
                this.borderInterface = null;
                return false;
            }


            SetTransparency(true);
            BackgroundColor = Color.Transparent;
            borderInterface.BorderWindow = this;

            EnableFloatingMode(true);

            borderInterface.OnCreated(rootView);

            return true;
        }

        /// Create the border UI.
        private bool CreateBorder()
        {
            rootView = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                BackgroundColor = Color.Transparent,
            };

            // Gets the Border's UI.
            borderInterface.CreateBorderView(rootView);
            if (rootView == null)
            {
                return false;
            }
            else
            {
                if (borderInterface.OverlayMode == true)
                {
                    rootView.InterceptTouchEvent += OverlayInterceptTouch;
                }
                // Add a view to the border layer.
                GetBorderWindowBottomLayer().Add(rootView);

                return true;
            }
        }

        /// <summary>
        /// Calculates which direction to resize or to move.
        /// </summary>
        /// <param name="xPosition">The X position.</param>
        /// <param name="yPosition">The Y position.</param>
        /// <returns>The BorderDirection</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BorderDirection GetDirection(float xPosition, float yPosition)
        {
            BorderDirection direction = BorderDirection.None;

            // check bottom left corner
            if (xPosition < borderInterface.TouchThickness && yPosition > WindowSize.Height + borderInterface.BorderHeight - borderInterface.TouchThickness)
            {
                direction = BorderDirection.BottomLeft;
            }
            // check bottom right corner
            else if (xPosition > WindowSize.Width + borderInterface.BorderLineThickness * 2 - borderInterface.TouchThickness && yPosition > WindowSize.Height + borderInterface.BorderHeight - borderInterface.TouchThickness)
            {
                direction = BorderDirection.BottomRight;
            }
            // check top left corner
            else if (xPosition < borderInterface.TouchThickness && yPosition <  borderInterface.TouchThickness)
            {
                direction = BorderDirection.TopLeft;
            }
            // check top right corner
            else if (xPosition > WindowSize.Width + borderInterface.BorderLineThickness * 2 - borderInterface.TouchThickness && yPosition < borderInterface.TouchThickness)
            {
                direction = BorderDirection.TopRight;
            }
            // check left side
            else if (xPosition < borderInterface.TouchThickness)
            {
                direction = BorderDirection.Left;
            }
            // check right side
            else if (xPosition > WindowSize.Width + borderInterface.BorderLineThickness*2 - borderInterface.TouchThickness)
            {
                direction = BorderDirection.Right;
            }
            // check bottom side
            else if (yPosition > WindowSize.Height + borderInterface.BorderHeight + borderInterface.BorderLineThickness - borderInterface.TouchThickness)
            {
                direction = BorderDirection.Bottom;
            }
            // check top side
            else if (yPosition < borderInterface.TouchThickness)
            {
                direction = BorderDirection.Top;
            }
            // check move
            else if (yPosition > WindowSize.Height)
            {
                direction = BorderDirection.Move;
            }

            return direction;
        }


        private bool OverlayInterceptTouch(object sender, View.TouchEventArgs e)
        {
            if (isInterceptTouch == true && overlayTimer != null)
            {
                overlayTimer.Start();
            }
            return false;
        }

        private bool OnTick(object o, Timer.TickEventArgs e)
        {
            GetBorderWindowBottomLayer().LowerToBottom();
            if (rootView != null)
            {
                rootView.Hide();
            }
            isInterceptTouch = false;

            overlayTimer.Stop();
            overlayTimer.Dispose();
            overlayTimer = null;
            return false;
        }

        // Intercept touch on window.
        private bool OnWinInterceptTouch(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Up)
            {
                if (isInterceptTouch == false && overlayTimer == null)
                {
                    overlayTimer = new Timer(3000);
                    overlayTimer.Tick += OnTick;
                    overlayTimer.Start();
                    GetBorderWindowBottomLayer().RaiseToTop();
                    if (rootView != null)
                    {
                        rootView.Show();
                    }
                    isInterceptTouch = true;
                }
            }
            return false;
        }

        private void OverlayMode(bool enable)
        {
            if (borderInterface.OverlayMode == true)
            {
                if (enable == true)
                {
                    InterceptTouchEvent += OnWinInterceptTouch;
                    if (rootView != null)
                    {
                        overlayBackgroundColor = new Color(rootView.BackgroundColor);
                        rootView.BackgroundColor = new Color(1, 1, 1, 0.3f);
                        rootView.Hide();
                    }
                }
                else
                {
                    if (overlayTimer != null)
                    {
                        overlayTimer.Stop();
                        overlayTimer.Dispose();
                        overlayTimer = null;
                    }
                    isInterceptTouch = false;
                    InterceptTouchEvent -= OnWinInterceptTouch;
                    GetBorderWindowBottomLayer().LowerToBottom();
                    if (rootView != null)
                    {
                        rootView.BackgroundColor = overlayBackgroundColor;
                        rootView.Show();
                    }
                }
            }
        }


        // Called when the window size has changed.
        private void OnBorderWindowResized(object sender, Window.ResizedEventArgs e)
        {
            Tizen.Log.Info("NUI", $"OnBorderWindowResized {e.WindowSize.Width},{e.WindowSize.Height}\n");
            int resizeWidth = e.WindowSize.Width;
            int resizeHeight = e.WindowSize.Height;
            if (borderInterface.MinSize != null)
            {
                resizeWidth = borderInterface.MinSize.Width > resizeWidth ? (int)borderInterface.MinSize.Width : resizeWidth;
                resizeHeight = borderInterface.MinSize.Height > resizeHeight ? (int)borderInterface.MinSize.Height : resizeHeight;
            }

            if (borderInterface.MaxSize != null)
            {
                resizeWidth = borderInterface.MaxSize.Width < resizeWidth ? (int)borderInterface.MaxSize.Width : resizeWidth;
                resizeHeight = borderInterface.MaxSize.Height < resizeHeight ? (int)borderInterface.MaxSize.Height : resizeHeight;
            }

            if (resizeWidth != e.WindowSize.Width || resizeHeight != e.WindowSize.Height)
            {
                WindowSize = new Size2D(resizeWidth, resizeHeight);
            }

            if (borderInterface.OverlayMode == true && IsMaximized() == true)
            {
                Interop.ActorInternal.SetSize(GetBorderWindowRootLayer().SwigCPtr, resizeWidth, resizeHeight);
                Interop.ActorInternal.SetSize(GetBorderWindowBottomLayer().SwigCPtr, resizeWidth, resizeHeight);
                OverlayMode(true);
            }
            else
            {
                Interop.ActorInternal.SetSize(GetBorderWindowRootLayer().SwigCPtr, resizeWidth, resizeHeight);
                Interop.ActorInternal.SetSize(GetBorderWindowBottomLayer().SwigCPtr, resizeWidth + borderInterface.BorderLineThickness * 2, resizeHeight+borderInterface.BorderHeight + borderInterface.BorderLineThickness);
                OverlayMode(false);
            }

            borderInterface.OnResized(resizeWidth, resizeHeight);

            if (NDalicPINVOKE.SWIGPendingException.Pending) { throw NDalicPINVOKE.SWIGPendingException.Retrieve(); }
        }

        internal Layer GetBorderWindowBottomLayer()
        {
            if (borderWindowBottomLayer == null)
            {
                borderWindowBottomLayer = new Layer();
                borderWindowBottomLayer.Name = "BorderWindowBottomLayer";
                Interop.ActorInternal.SetParentOrigin(borderWindowBottomLayer.SwigCPtr, Tizen.NUI.ParentOrigin.TopCenter.SwigCPtr);
                Interop.Actor.SetAnchorPoint(borderWindowBottomLayer.SwigCPtr, Tizen.NUI.PivotPoint.TopCenter.SwigCPtr);
                Interop.Actor.Add(rootLayer.SwigCPtr, borderWindowBottomLayer.SwigCPtr);
                Interop.ActorInternal.SetSize(borderWindowBottomLayer.SwigCPtr, WindowSize.Width+borderInterface.BorderLineThickness * 2, WindowSize.Height + borderInterface.BorderLineThickness);
                borderWindowBottomLayer.SetWindow(this);
                borderWindowBottomLayer.LowerToBottom();

                if (NDalicPINVOKE.SWIGPendingException.Pending) { throw NDalicPINVOKE.SWIGPendingException.Retrieve(); }
            }
            return borderWindowBottomLayer;
        }

        internal Layer GetBorderWindowRootLayer()
        {
            if (borderWindowRootLayer == null)
            {
                borderWindowRootLayer = new Layer();
                borderWindowRootLayer.Name = "RootLayer";
                Interop.ActorInternal.SetParentOrigin(borderWindowRootLayer.SwigCPtr, Tizen.NUI.ParentOrigin.TopCenter.SwigCPtr);
                Interop.Actor.SetAnchorPoint(borderWindowRootLayer.SwigCPtr, Tizen.NUI.PivotPoint.TopCenter.SwigCPtr);
                Interop.Actor.Add(rootLayer.SwigCPtr, borderWindowRootLayer.SwigCPtr);
                Interop.ActorInternal.SetSize(borderWindowRootLayer.SwigCPtr, WindowSize.Width, WindowSize.Height-borderInterface.BorderHeight - borderInterface.BorderLineThickness);
                Interop.ActorInternal.SetPosition(borderWindowRootLayer.SwigCPtr, 0, borderInterface.BorderLineThickness);
                Tizen.NUI.Object.SetProperty(borderWindowRootLayer.SwigCPtr, Tizen.NUI.BaseComponents.View.Property.ClippingMode, new Tizen.NUI.PropertyValue((int)Tizen.NUI.ClippingModeType.ClipToBoundingBox));

                if (NDalicPINVOKE.SWIGPendingException.Pending) { throw NDalicPINVOKE.SWIGPendingException.Retrieve(); }
            }

            return borderWindowRootLayer;
        }

        internal void DisposeBorder()
        {
            Resized -= OnBorderWindowResized;
            if (borderInterface.OverlayMode == true && rootView != null)
            {
                rootView.InterceptTouchEvent -= OverlayInterceptTouch;
            }
            borderInterface.Dispose();
            GetBorderWindowBottomLayer().Dispose();
        }

        private void convertBorderWindowSizeToRealWindowSize(Uint16Pair size)
        {
            if (isBorderWindow == true)
            {
                var height = (ushort)(size.GetHeight() + borderInterface.BorderHeight + borderInterface.BorderLineThickness);
                var width = (ushort)(size.GetWidth() + borderInterface.BorderLineThickness * 2);
                size.SetHeight(height);
                size.SetWidth(width);
            }
        }

        private void convertRealWindowSizeToBorderWindowSize(Uint16Pair size)
        {
            if (isBorderWindow == true && !(borderInterface.OverlayMode == true && IsMaximized() == true))
            {
                var height = (ushort)(size.GetHeight() - borderInterface.BorderHeight - borderInterface.BorderLineThickness);
                var width = (ushort)(size.GetWidth() - borderInterface.BorderLineThickness * 2);
                size.SetHeight(height);
                size.SetWidth(width);
            }
        }
        #endregion //Methods

        #region Structs
        #endregion //Structs

        #region Classes
        #endregion //Classes
    }



}
