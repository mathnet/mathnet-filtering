using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// ICommand implementation of the AddShape action
    /// </summary>
    public class AddShapeCommand : Command
    {
        #region Fields
        IShape shape;        
        Point location;
        #endregion

        #region Properties
        public IShape Shape
        {
            get { return shape; }
        }
        #endregion



        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AddShapeCommand"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="location">The location.</param>
        public  AddShapeCommand(IController controller, IShape shape, Point location) :base(controller)
        {
            if (shape == null)
                throw new ArgumentNullException("The shape is 'null' and cannot be inserted.");            
            this.Text = "Add " + shape.EntityName;          

            this.location = location;
            this.shape = shape;
        }

        /// <summary>
        /// Perform redo of this command.
        /// </summary>
        public override void Redo()
        {
            Controller.Model.AddShape(shape);
            
            shape.Transform(location.X, location.Y, shape.Width, shape.Height);
            Rectangle rec = shape.Rectangle;
            rec.Inflate(20, 20);
            Controller.View.Invalidate(rec);            
        }

        /// <summary>
        /// Perform undo of this command.
        /// </summary>
        public override void Undo()
        {
            Rectangle rec = shape.Rectangle;
            rec.Inflate(20, 20);
            Controller.Model.RemoveShape(shape);

            Controller.View.Invalidate(rec);

        }

        #endregion

    }

}
