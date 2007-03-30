using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Descriptor for <see cref="SimpleShapeBase"/> derived classes (i.e. most of the simple drawing elements like the <see cref="SimpleRectangle"/> shape).
    /// </summary>
    class SimpleShapeBaseDescriptor : ShapeBaseDescriptor
    {

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

        protected override void SetValue(object sender, PropertyEventArgs e)
        {
            switch (e.Name)
            {                
                default:
                    base.SetValue(sender, e);
                    break;
            }
        }

        public SimpleShapeBaseDescriptor(ShapeProvider provider, Type type)
            : base(provider, type)
        {
            this.AddProperty("Demo", typeof(int));            
        }


    }
}
