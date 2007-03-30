using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
namespace Netron.Diagramming.Core
{
    class ConnectionBaseDescriptor : DiagramEntityBaseDescriptor
    {

        #region Constants
        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectionBaseDescriptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="objectType">Type of the object.</param>
        public ConnectionBaseDescriptor(ConnectionProvider provider, Type objectType)
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
            this.AddProperty("From", typeof(Point), constLayout, "The location of the initial connector.", Point.Empty, typeof(UITypeEditor), typeof(Netron.Diagramming.Core.PointConverter));
            this.AddProperty("To", typeof(Point), constLayout, "The locationof the end connector.", Point.Empty, typeof(UITypeEditor), typeof(Netron.Diagramming.Core.PointConverter));
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
                case "to":
                    e.Value = (e.Component as ConnectionBase).To.Point;
                    break;
                case "from":
                    e.Value = (e.Component as ConnectionBase).From.Point;
                    break;



            }


        }

        #endregion
    }
}
