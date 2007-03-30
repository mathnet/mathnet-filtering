using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The base class for any tool <see cref="ITool"/>
    /// </summary>
    public abstract class AbstractTool : ITool
    {

        #region Fields
        /// <summary>
        /// the Name field
        /// </summary>
        private string mName;
        /// <summary>
        /// the Enabled field
        /// </summary>
        private bool mEnabled = true;
        /// <summary>
        /// a pointer to the controller
        /// </summary>
        private IController mController;
        /// <summary>
        /// the tool's cursor
        /// </summary>
        private Cursor mCursor = Cursors.Default;
        /// <summary>
        /// whether the tool is currently active
        /// </summary>
        private bool mIsActive;
        /// <summary>
        /// keeps a reference to the previous cursor
        /// </summary>
        private Cursor prevCursor ;
        /// <summary>
        /// the suspend state of the tool
        /// </summary>
        private bool mIsSuspended;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is suspended. A tool enters in a suspended mode when another tool has been activated and disallows another to continue its normal activity. For example, the <see cref="MoveTool"/> and <see cref="SelectionTool"/> are
        /// mutually exclusive and similarly for the drawing tools and the selection tool.
        /// <para>This suspended state is independent of the <see cref="IsActive"/> and the <see cref="Enabled"/> states.</para>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuspended
        {
            get { return mIsSuspended; }
            set { mIsSuspended = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tool is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return mIsActive; }
            set { mIsActive = value; }
        }
        /// <summary>
        /// Gets or sets the name of the tool.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        public IController Controller
        {
            get
            {
                return mController;
            }

            set
            {
                mController = value;
            }
        }
        /// <summary>
        /// Gets or sets the cursor.
        /// </summary>
        /// <value>The cursor.</value>
        public Cursor Cursor
        {
            get { return mCursor; }
            set { mCursor = value;}
        }

        /// <summary>
        /// Gets or sets the Enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.mEnabled;
            }

            set
            {
                //disable the tool first if it is active
                if (!value && IsActive)
                {                       
                    DeactivateTool();
                }
                mEnabled = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tool excludes other tools.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is exclusive; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsExclusive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tool can activated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can activate; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanActivate
        {
            get
            {
                if (mEnabled )
                {
                    return !IsActive ;
                }
                else
                {
                    return false;
                }
            }
            
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public AbstractTool(string name)
        {
            this.mName = name;
        }
        #endregion

        #region Methods
        protected void RestoreCursor()
        {
            if (prevCursor != null)
            {
                if (Controller != null)
                {
                    Controller.View.CurrentCursor = prevCursor;
                }
                prevCursor = null;
            }
        }
        #region Activation & deactivation

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        /// <returns></returns>
         public bool DeactivateTool()
        {
         
            if (IsActive)
            {
                OnDeactivateTool();
                IsActive = false;
                RestoreCursor();
                UnsuspendTools();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Activates the tool.
        /// </summary>
        /// <returns></returns>
        public bool ActivateTool()
        {
            //halt other actions
            SuspendOtherTools();
            
            if (Enabled && !IsActive)
            {
                prevCursor = this.Controller.View.CurrentCursor;
                IsActive = true;
                OnActivateTool();
            }
            return IsActive;
        }
        /// <summary>
        /// Suspends the other tools.
        /// </summary>
        public void SuspendOtherTools()
        {
            foreach (ITool tool in Controller.Tools)
            {
                if (tool != this)
                    tool.IsSuspended = true;
            }
        }
        /// <summary>
        /// Releases the previously suspeneded tools <see cref="SuspendOtherTools"/>
        /// </summary>
        public void UnsuspendTools()
        {
            foreach (ITool tool in Controller.Tools)
            {                  
               tool.IsSuspended = false;
            }
        }
        #endregion
        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected virtual void OnActivateTool(){}

        /// <summary>
        /// Called when the tool is deactivated.
        /// </summary>
        protected virtual void OnDeactivateTool(){}

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return null; 
        }
        #endregion

    }
}
