using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for more specialized instances of the <see cref="ShapeBase"/> class.
    /// </summary>
    abstract class ShapeBaseDescriptor : DiagramEntityBaseDescriptor
    {

        #region Constants
        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeBaseDescriptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="objectType">Type of the object.</param>
        public ShapeBaseDescriptor(ShapeProvider provider, Type objectType)
            : base(provider, objectType)
        {
            AddBaseProperties();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the properties of the <see cref="ShapeBase"/>
        /// </summary>
        private void AddBaseProperties()
        {
            this.AddProperty("Width", typeof(int), constLayout, "The width of the shape.");
            this.AddProperty("Height", typeof(int), constLayout, "The height of the shape.");
            this.AddProperty("Location", typeof(Point), constLayout, "The location of the shape.", Point.Empty, typeof(UITypeEditor), typeof(Netron.Diagramming.Core.PointConverter));
        }


        /// <summary>
        /// Override this method to return the appropriate value corresponding to the property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void GetValue(object sender, PropertyEventArgs e)
        {
            base.GetValue(sender, e);
            switch (e.Name.ToLower())
            {
                case "width":
                    e.Value = (e.Component as ShapeBase).Width;
                    break;
                case "height":
                    e.Value = (e.Component as ShapeBase).Height;
                    break;
                case "location":
                    e.Value = (e.Component as ShapeBase).Location;
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
            base.SetValue(sender, e);

            switch (e.Name.ToLower())
            {
                case "width":
                    (e.Component as ShapeBase).Width = (int)e.Value;
                    break;
                case "height":
                    (e.Component as ShapeBase).Height = (int)e.Value;
                    break;
                case "location":
                    Point p = (Point)e.Value;
                    (e.Component as ShapeBase).Location = new Point(p.X, p.Y);
                    break;
            }
        }


        #endregion
    }
}
