using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Descriptor for <see cref="SimpleShapeBase"/> derived classes (i.e. most of the simple drawing elements like the <see cref="SimpleRectangle"/> shape).
    /// </summary>
    class ClassShapeDescriptor : ShapeBaseDescriptor
    {

        /// <summary>
        /// Override this method to return the appropriate value corresponding to the property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void GetValue(object sender, PropertyEventArgs e)
        {
            switch (e.Name)
            {
                case "Title":
                    e.Value = (e.Component as ClassShape).Title;
                    break;
                case "SubTitle":
                    e.Value = (e.Component as ClassShape).SubTitle;
                    break;
                case "FreeText":
                    e.Value = (e.Component as ClassShape).FreeText;
                    break;
                case "BodyType":
                    e.Value = (e.Component as ClassShape).BodyType;
                    break;
                case "Folders":
                    e.Value = (e.Component as ClassShape).Folders;
                    break;
                default:
                    base.GetValue(sender, e);
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
            switch (e.Name)
            {
                case "Title":
                   (e.Component as ClassShape).Title = e.Value as string;
                    break;
                case "SubTitle":
                    (e.Component as ClassShape).SubTitle = e.Value as string;
                    break;
                case "FreeText":
                    (e.Component as ClassShape).FreeText = e.Value as string;
                    break;
                case "BodyType":
                    (e.Component as ClassShape).BodyType =(BodyType) Enum.Parse(typeof(BodyType), e.Value.ToString()) ;
                    break;
                case "Folders":
                    (e.Component as ClassShape).Folders = e.Value as CollectionBase<FolderMaterial>;
                    break;
                default:
                    base.SetValue(sender, e);
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ClassShapeDescriptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="type">The type.</param>
        public ClassShapeDescriptor(ShapeProvider provider, Type type)
            : base(provider, type)
        {
            this.AddProperty("Title", typeof(string),constContent,"The title appearing at the top of the shape.");
            this.AddProperty("SubTitle", typeof(string), constContent, "The sub-title appearing underneath the title.");
            this.AddProperty("FreeText", typeof(string), constContent, "The text of the body when the shape is set to free-text mode.");
            this.AddProperty("BodyType", typeof(BodyType), constContent, "The body type.");
            this.AddProperty("Folders", typeof(CollectionBase<FolderMaterial>), constContent, "The folders when the shape is set to list mode.");            

        }


    }
}

