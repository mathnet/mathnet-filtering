using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Descriptor for <see cref="SimpleShapeBase"/> derived classes (i.e. most of the simple drawing elements like the <see cref="SimpleRectangle"/> shape).
    /// </summary>
    class ControlDescriptor : DescriptorBase
    {

        protected override void GetValue(object sender, PropertyEventArgs e)
        {
            switch (e.Name)
            {
                case "BackColor":
                    e.Value = (e.Component as DiagramControlBase).BackColor;
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
                case "BackColor":
                    (e.Component as DiagramControlBase).BackColor = (Color) e.Value;
                    break;
                default:
                    base.SetValue(sender, e);
                    break;
            }
        }

        public ControlDescriptor(ControlProvider provider, Type type)
            : base(provider, type)
        {
            this.AddProperty("BackColor", typeof(Color));            
        }


    }
}
