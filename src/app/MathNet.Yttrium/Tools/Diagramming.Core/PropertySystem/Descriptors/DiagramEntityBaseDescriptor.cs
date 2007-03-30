using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for all descriptors of diagram entities inheriting from <see cref="DiagramEntityBase"/>.
    /// This descriptor collects all properties common to all the entities inheriting from the <see cref="IDiagramEntity"/> interface.
    /// </summary>
    abstract class DiagramEntityBaseDescriptor : DescriptorBase
    {

        #region Constants
        /// <summary>
        /// the 'Layout' section
        /// </summary>
        protected const string constLayout = "Layout";
        /// <summary>
        /// the 'General' section
        /// </summary>
        protected const string constGeneral = "General";
        /// <summary>
        /// the 'content' section
        /// </summary>
        protected const string constContent = "Content";
        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramEntityBaseDescriptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="objectType">Type of the object.</param>
        public DiagramEntityBaseDescriptor(TypeDescriptionProvider provider, Type objectType)
            : base(provider, objectType)
        {
            AddBaseProperties();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the base properties of the <see cref="ShapeBase"/>
        /// </summary>
private void AddBaseProperties()
{
    this.AddProperty("Entity Name", typeof(string), constGeneral, "The name of the type");
    this.AddProperty("Name", typeof(string), constGeneral, "The name of the entity");
    this.AddProperty("Rectangle", typeof(Rectangle), constLayout, "The bounding rectangle of the entity");
    this.AddProperty("Scene index", typeof(int), constLayout, "The index of the entity in the scane graph.");
    this.AddProperty("Tag", typeof(object), constLayout, "General purpose tag.");
}


        /// <summary>
        /// Override this method to return the appropriate value corresponding to the property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void GetValue(object sender, PropertyEventArgs e)
        {
            switch (e.Name.ToLower())
            {
                case "entity name":
                    e.Value = (e.Component as DiagramEntityBase).EntityName;
                    break;
                case "name":
                    e.Value = (e.Component as DiagramEntityBase).Name;
                    break;
                case "rectangle":
                    e.Value = (e.Component as DiagramEntityBase).Rectangle;
                    break;
                case "scene index":
                    e.Value = (e.Component as DiagramEntityBase).SceneIndex;
                    break;
                case "tag":
                    e.Value = (e.Component as DiagramEntityBase).Tag;
                    break;
            }


        }



        /// <summary>
        /// Override this method to set the appropriate value corresponding to the property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SetValue(object sender, PropertyEventArgs e)
        {
            switch (e.Name.ToLower())
            {
                case "name":
                    (e.Component as DiagramEntityBase).Name = (string)e.Value;
                    break;
                case "tag":
                    (e.Component as DiagramEntityBase).Tag = (object) e.Value;
                    break;
            }
        }


        #endregion
    }
}
