using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// ICommand implementation of the AddConnection action
    /// </summary>
    class AddConnectionCommand : Command
    {
        IConnection connection;        

        public IConnection Connection
        {
            get { return connection; }
        }

        

        public  AddConnectionCommand(IController controller, IConnection connection) :base(controller)
        {
            if (connection == null)
                throw new ArgumentNullException("The connection is 'null' and cannot be inserted.");            
            this.Text = "Add " + connection.EntityName;          
            this.connection = connection;
        }

        public override void Redo()
        {
            Controller.Model.AddConnection(connection);
            
            //connection.Location = location;
            Rectangle rec = connection.Rectangle;
            rec.Inflate(20, 20);
            Controller.View.Invalidate(rec);            
        }

        public override void Undo()
        {
            Rectangle rec = connection.Rectangle;
            rec.Inflate(20, 20);
            Controller.Model.Remove(connection);
            Controller.View.Invalidate(rec);
            
        }


    }

}
