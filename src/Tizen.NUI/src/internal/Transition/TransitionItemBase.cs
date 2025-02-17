/*
 * Copyright(c) 2021 Samsung Electronics Co., Ltd.
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
    internal class TransitionItemBase : BaseHandle
    {
        private bool appearingTransition = true;

        /// <summary>
        /// Creates an initialized TransitionItemBase.<br />
        /// </summary>
        public TransitionItemBase(View target, bool appearingTransition, TimePeriod timePeriod, AlphaFunction alphaFunction) : this(Interop.TransitionItemBase.New(), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            
            AppearingTransition = appearingTransition;
            TimePeriod = timePeriod;
            AlphaFunction = alphaFunction;
        }

        internal TransitionItemBase(global::System.IntPtr cPtr, bool cMemoryOwn) : base(cPtr, cMemoryOwn)
        {
        }

        /// <summary>
        /// Gets or sets the TimePeriod
        /// </summary>
        public TimePeriod TimePeriod
        {
            set
            {
                Interop.TransitionItemBase.SetTimePeriod(SwigCPtr, value.SwigCPtr);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            }
            get
            {
                TimePeriod ret = new TimePeriod(Interop.TransitionItemBase.GetTimePeriod(SwigCPtr), true);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
                return ret;
            }
        }

        /// <summary>
        /// Gets or sets the AlphaFunction
        /// </summary>
        public AlphaFunction AlphaFunction
        {
            set
            {
                Interop.TransitionItemBase.SetAlphaFunction(SwigCPtr, value.SwigCPtr);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            }
            get
            {
                AlphaFunction ret = new AlphaFunction(Interop.TransitionItemBase.GetAlphaFunction(SwigCPtr), true);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
                return ret;
            }
        }

        /// <summary>
        /// Gets or sets whether the View moves with child or not.
        /// </summary>
        public virtual bool TransitionWithChild
        {
            set
            {
                Interop.TransitionItemBase.TransitionWithChild(SwigCPtr, value);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            }
        }

        /// <summary>
        /// Gets or sets whether this transition is appearing transition or not.
        /// </summary>
        public bool AppearingTransition
        {
            set
            {
                appearingTransition = value;
                Interop.TransitionItemBase.SetAppearingTransition(SwigCPtr, value);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            }
            get
            {
                return appearingTransition;
            }
        }

        /// <summary>
        /// Downcasts a handle to TransitionItemBase handle.<br />
        /// If handle points to an TransitionItemBase object, the downcast produces a valid handle.<br />
        /// If not, the returned handle is left uninitialized.<br />
        /// </summary>
        /// <param name="handle">Handle to an object.</param>
        /// <returns>Handle to an TransitionItemBase object or an uninitialized handle.</returns>
        /// <exception cref="ArgumentNullException"> Thrown when handle is null. </exception>
        public static TransitionItemBase DownCast(BaseHandle handle)
        {
            if (handle == null)
            {
                throw new ArgumentNullException(nameof(handle));
            }
            TransitionItemBase ret = Registry.GetManagedBaseHandleFromNativePtr(handle) as TransitionItemBase;
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }

        internal TransitionItemBase(TransitionItemBase handle) : this(Interop.TransitionItemBase.NewTransitionItemBase(TransitionItemBase.getCPtr(handle)), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        internal TransitionItemBase Assign(TransitionItemBase rhs)
        {
            TransitionItemBase ret = new TransitionItemBase(Interop.TransitionItemBase.Assign(SwigCPtr, TransitionItemBase.getCPtr(rhs)), false);
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }

        /// <summary>
        /// To make TransitionItemBase instance be disposed.
        /// </summary>
        protected override void Dispose(DisposeTypes type)
        {
            if (disposed)
            {
                return;
            }
            base.Dispose(type);
        }

        /// This will not be public opened.
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ReleaseSwigCPtr(System.Runtime.InteropServices.HandleRef swigCPtr)
        {
            if (swigCPtr.Handle == IntPtr.Zero || this.HasBody() == false)
            {
                Tizen.Log.Fatal("NUI", $"[ERROR] TransitionItemBase ReleaseSwigCPtr()! IntPtr=0x{swigCPtr.Handle:X} HasBody={this.HasBody()}");
                return;
            }
            Interop.TransitionItemBase.Delete(swigCPtr);
        }

        private float MilliSecondsToSeconds(int millisec)
        {
            return (float)millisec / 1000.0f;
        }

        private int SecondsToMilliSeconds(float sec)
        {
            return (int)(sec * 1000);
        }
    }
}
