using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for custom descriptors based on our properties mechanism.
    /// <remarks>
    /// Usually you do not inherit from this class to describe properties of entities, use the appropriate 
    /// specializations like the <see cref="ShapeBaseDescriptor"/> or <see cref="ConnectionBaseDescriptor"/> classes instead.
    /// Also, this class does not define any properties for the property grid but contains methods and basic stuff to make the construction of
    /// concrete descriptors easier.
    /// </remarks>
    /// </summary>
    abstract class DescriptorBase : CustomTypeDescriptor
    {
        #region Fields
        //the provider to which this descriptor is attached
        private TypeDescriptionProvider provider;
        /// <summary>
        /// the type being served
        /// </summary>
        private Type type;
        /// <summary>
        /// the collection of properties displayed
        /// </summary>
        private PropertyDescriptorCollection mProperties;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of property descrtiptors
        /// </summary>
        protected PropertyDescriptorCollection Properties
        {
            get { return mProperties; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DescriptorBase"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="parentdescriptor">The parentdescriptor.</param>
        /// <param name="objectType">Type of the object.</param>
        public DescriptorBase(ShapeProvider provider, ICustomTypeDescriptor parentdescriptor, Type objectType)
            : base(parentdescriptor)
        {
            this.provider = provider;
            this.type = objectType;
            mProperties = new PropertyDescriptorCollection(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DescriptorBase"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="objectType">Type of the object.</param>
        public DescriptorBase(TypeDescriptionProvider provider, Type objectType)
            : base()
        {
            this.provider = provider;
            this.type = objectType;
            mProperties = new PropertyDescriptorCollection(null);            
        }
        #endregion

        #region Methods

        #region AddProperty overloads
        /// <summary>
        /// Adds a new property to the property descriptor collection.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        public void AddProperty(string name, Type type)
        {
            PropertySpec widthSpec = new PropertySpec(name, type);
            PropertySpecDescriptor pd = widthSpec.ToPropertyDescriptor();
            pd.OnGetValue += new EventHandler<PropertyEventArgs>(GetValue);
            pd.OnSetValue += new EventHandler<PropertyEventArgs>(SetValue);
            mProperties.Add(pd);
        }
        /// <summary>
        /// Adds a new property to the property descriptor collection.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        public void AddProperty(string name, Type type, string category)
        {
            PropertySpec widthSpec = new PropertySpec(name, type, category);
            PropertySpecDescriptor pd = widthSpec.ToPropertyDescriptor();
            pd.OnGetValue += new EventHandler<PropertyEventArgs>(GetValue);
            pd.OnSetValue += new EventHandler<PropertyEventArgs>(SetValue);
            mProperties.Add(pd);
        }
        /// <summary>
        /// Adds a new property to the property descriptor collection.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        public void AddProperty(string name, Type type, string category, string description)
        {
            PropertySpec widthSpec = new PropertySpec(name, type, category, description);
            PropertySpecDescriptor pd = widthSpec.ToPropertyDescriptor();
            pd.OnGetValue += new EventHandler<PropertyEventArgs>(GetValue);
            pd.OnSetValue += new EventHandler<PropertyEventArgs>(SetValue);
            mProperties.Add(pd);
        }
        /// <summary>
        /// Adds a new property to the property descriptor collection.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        public void AddProperty(string name, Type type, string category, string description, object defaultValue)
        {
            PropertySpec widthSpec = new PropertySpec(name, type, category, description, defaultValue);
            PropertySpecDescriptor pd = widthSpec.ToPropertyDescriptor();
            pd.OnGetValue += new EventHandler<PropertyEventArgs>(GetValue);
            pd.OnSetValue += new EventHandler<PropertyEventArgs>(SetValue);
            mProperties.Add(pd);
        }
        /// <summary>
        /// Adds a new property to the property descriptor collection.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The Type that represents the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public void AddProperty(string name, Type type, string category, string description, object defaultValue, Type editor, Type typeConverter)
        {
            PropertySpec widthSpec = new PropertySpec(name, type, category, description, defaultValue, editor, typeConverter);
            PropertySpecDescriptor pd = widthSpec.ToPropertyDescriptor();
            pd.OnGetValue += new EventHandler<PropertyEventArgs>(GetValue);
            pd.OnSetValue += new EventHandler<PropertyEventArgs>(SetValue);
            mProperties.Add(pd);
        }
        #endregion


        

        /// <summary>
        /// Overrides the method to retun the collection of properties we defined internally
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return mProperties;
        }

        /// <summary>
        /// Returns a collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"></see> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"></see>.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }
        /// <summary>
        /// Override this method to return the appropriate value corresponding to the property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GetValue(object sender, PropertyEventArgs e)
        {

        }

        /// <summary>
        /// Override this method to set the appropriate value corresponding to the property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SetValue(object sender, PropertyEventArgs e)
        {
        } 
        #endregion
    }
}
