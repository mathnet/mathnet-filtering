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
    class ComplexShapeBaseDescriptor : ShapeBaseDescriptor
    {

        protected override void GetValue(object sender, PropertyEventArgs e)
        {
            switch (e.Name)
            {
                case "Materials":
                    e.Value = (e.Component as ComplexShapeBase).Children;
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

        public ComplexShapeBaseDescriptor(ShapeProvider provider, Type type)
            : base(provider, type)
        {
            this.AddProperty("Materials", typeof(CollectionBase<IShapeMaterial>),constLayout,"The collection of shape materials.");            
        }


    }
}
