using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// ICommand implementation of the AddShape action
    /// </summary>
    class AddMultiPointShapeCommand : Command
    {
        IShape shape;        

        public IShape Shape
        {
            get { return shape; }
        }

        

        public  AddMultiPointShapeCommand(IController controller, IShape shape) :base(controller)
        {
            if (shape == null)
                throw new ArgumentNullException("The shape is 'null' and cannot be inserted.");            
            this.Text = "Add " + shape.EntityName;          

            
            this.shape = shape;
        }

        public override void Redo()
        {
            Controller.Model.AddShape(shape);
            Rectangle rec = shape.Rectangle;
            rec.Inflate(20, 20);
            Controller.View.Invalidate(rec);            
        }

        public override void Undo()
        {
            Rectangle rec = shape.Rectangle;
            rec.Inflate(20, 20);
            Controller.Model.RemoveShape(shape);

            Controller.View.Invalidate(rec);
            
        }


    }

}
