using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using Netron.Diagramming.Core;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;

namespace Netron.Diagramming.Core
{
    public partial class SwitchIconMaterial : ClickableIconMaterial
    {

        #region Events
        /// <summary>
        /// Occurs when the icon is switched into the 'collapsed' state.
        /// </summary>
        public event EventHandler OnCollapse;
        /// <summary>
        /// Occurs when the icon is switched into the 'expanded' state.
        /// </summary>
        public event EventHandler OnExpand;
        #endregion
        
        #region Fields
        /// <summary>
        /// the 'down/expanded' icon
        /// </summary>
        private Bitmap downBmp;
        /// <summary>
        /// the 'up/collapsed' icon
        /// </summary>
        private Bitmap upBmp;
        /// <summary>
        /// the mCollapsed field
        /// </summary>
        private bool mCollapsed = true;
        /// <summary>
        /// the current switch type
        /// </summary>
        private SwitchIconType mSwitchType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the switch.
        /// </summary>
        /// <value>The type of the switch.</value>
        public SwitchIconType SwitchType
        {
            get
            {
                return mSwitchType;
            }
            
            internal set
            {
                SwitchToType(value);
                Collapsed = true;
                mSwitchType = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchIconMaterial"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public SwitchIconMaterial(SwitchIconType type)
            : base()
        {
            Gliding = false;
            mSwitchType = type;
            //fetch the two bitmaps
            try
            {
                SwitchToType(type);
                
            }
            catch (Exception exc)
            {
                throw new InconsistencyException("The necessary resource could not be found.", exc);
            }

        }
        /// <summary>
        /// Sets the switch to the given <see cref="SwitchIconType"/>
        /// </summary>
        /// <param name="type"></param>
        private void SwitchToType(SwitchIconType type)
        {
            switch(type)
            {
                case SwitchIconType.UpDown:
                    downBmp = GetBitmap("Resources.down.ico");
                    upBmp = GetBitmap("Resources.up.ico");
                    this.Icon = downBmp;
                    break;
                case SwitchIconType.PlusMinus:
                    downBmp = GetBitmap("Resources.plus.ico");
                    upBmp = GetBitmap("Resources.minus.ico");
                    this.Icon = downBmp;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the mouse-down event. 
        /// </summary>
        /// <param name="e">The <see cref="T:MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>Returns 'true' if the event was handled, otherwise 'false'.</returns>
        public override bool MouseDown(MouseEventArgs e)
        {
            base.MouseDown(e);
            if(e.Clicks == 1)
            {
                Collapsed = !Collapsed;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the Collapsed
        /// </summary>
        public bool Collapsed
        {
            get
            {
                return mCollapsed;
            }
            set
            {
                if(value)
                {
                    this.Icon = downBmp;
                    RaiseOnCollapse();
                }
                else
                {
                    this.Icon = upBmp;
                    RaiseOnExpand();
                }

                mCollapsed = value;
            }
        }


        /// <summary>
        /// Raises the <see cref="OnExpand"/> event.
        /// </summary>
        private void RaiseOnExpand()
        {
            if(OnExpand != null)
                OnExpand(this, EventArgs.Empty);
        }
        /// <summary>
        /// Raises the <see cref="OnCollapse"/> event
        /// </summary>
        private void RaiseOnCollapse()
        {
            if(OnCollapse != null)
                OnCollapse(this, EventArgs.Empty);
        }
        #endregion

    }

    /// <summary>
    /// The two type of bitmaps intrinsically defined by the <see cref="SwitchIconMaterial"/>
    /// </summary>
    public enum SwitchIconType
    { 
        /// <summary>
        /// The up/down arrows are displayed
        /// </summary>
        UpDown,
        /// <summary>
        /// The plus/minus icon are displayed
        /// </summary>
        PlusMinus
    }
}
