using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Provides data for the GetValue and SetValue events of the custom descriptors.
	/// </summary>
	public class PropertyEventArgs : EventArgs
	{
		#region Fields
        /// <summary>
        /// the mComponent of which the properties are examined
        /// </summary>
        private object mComponent;
        /// <summary>
        /// the value of the property
        /// </summary>
		private object mValue;
        /// <summary>
        /// the name of the property
        /// </summary>
        private string mName;                
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the mName of the property.
        /// </summary>
        /// <value>The mName.</value>
        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }
		/// <summary>
		/// Gets or sets the current value of the property.
		/// </summary>
		public object Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		/// <summary>
		/// Gets the component whose properties is examined
		/// </summary> 
		public object Component
		{
			get
			{ 
				return mComponent;
			}
		}
		#endregion

		#region Constructors
        /// <summary>
        /// Initializes a new instance of the PropertyEventArgs class.
        /// </summary>
        /// <param name="mComponent">The m component.</param>
        /// <param name="mName">The name of the property.</param>
        /// <param name="mValue">The value.</param>
		public PropertyEventArgs(object mComponent, string mName, object mValue)
		{
            this.mComponent = mComponent;
			this.mValue = mValue;
            this.mName = mName;
		}
		
		
		#endregion
	}

}
