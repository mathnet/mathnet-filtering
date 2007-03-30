using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// An inherited property descriptor which can be created dynamically on the basis of a <see cref="PropertySpec"/> specification.
    /// </summary>
    internal class PropertySpecDescriptor : PropertyDescriptor
    {
        #region events
        /// <summary>
        /// Occurs when the property grid accesses the value of the property
        /// </summary>
        public event EventHandler<PropertyEventArgs> OnGetValue;
        /// <summary>
        /// Occurs when the property grid tries to set the value of the property
        /// </summary>
        public event EventHandler<PropertyEventArgs> OnSetValue;
		 
	    #endregion

        #region Fields
        private PropertySpec item;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PropertySpecDescriptor"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="attributes">the attributes to be used on the descriptor. Note that the attributes of the <see cref="PropertySpec"/> have to be previously taken over and overloaded.</param>
        public PropertySpecDescriptor(PropertySpec item, Attribute[] attributes) : base(item.Name, attributes)
        {
            this.item = item;
        }
        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"></see> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"></see> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"></see> methods are invoked, the object specified might be an instance of this type.</returns>
        public override Type ComponentType
        {
            get
            {
                return item.GetType();
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the property is read-only; otherwise, false.</returns>
        public override bool IsReadOnly
        {
            get
            {
                return (Attributes.Matches(ReadOnlyAttribute.Yes));
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"></see> that represents the type of the property.</returns>
        public override Type PropertyType
        {
            get
            {
                return Type.GetType(item.TypeName);
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            if(item.DefaultValue == null)
                return false;
            else
                return !this.GetValue(component).Equals(item.DefaultValue);
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            // Have the property bag raise an event to get the current value
            // of the property.

            PropertyEventArgs e = new PropertyEventArgs(component, base.Name, null);
            RaiseOnGetValue(e);
            return e.Value;
        }

        /// <summary>
        /// Raises the on get value.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.PropertyEventArgs"/> instance containing the event data.</param>
        private void RaiseOnGetValue(PropertyEventArgs e)
        {
            EventHandler<PropertyEventArgs> handler = OnGetValue;
            if(handler != null)
                handler(this,e);
        }


        /// <summary>
        /// Raises the on set value.
        /// </summary>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.PropertyEventArgs"/> instance containing the event data.</param>
        private void RaiseOnSetValue(PropertyEventArgs e)
        {
            EventHandler<PropertyEventArgs> handler = OnSetValue;
            if(handler != null)
                handler(this,e);
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            SetValue(component, item.DefaultValue);
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            // Have the property bag raise an event to set the current value
            // of the property.

            PropertyEventArgs e = new PropertyEventArgs(component, Name, value);
            RaiseOnSetValue(e);
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            object val = this.GetValue(component);

            if(item.DefaultValue == null && val == null)
                return false;
            else
                return !val.Equals(item.DefaultValue);
        }

        #endregion
    }
}
