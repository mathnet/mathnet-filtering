using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Descriptor for <see cref="SimpleShapeBase"/> derived classes (i.e. most of the simple drawing elements like the <see cref="SimpleRectangle"/> shape).
    /// </summary>
    class ConnectionDescriptor : ConnectionBaseDescriptor
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
                case "Demo":
                    e.Value = 123456;
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
                default:
                    base.SetValue(sender, e);
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectionDescriptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="type">The type.</param>
        public ConnectionDescriptor(ConnectionProvider provider, Type type)
            : base(provider, type)
        {
            this.AddProperty("Demo", typeof(int));            
        }


    }
}
