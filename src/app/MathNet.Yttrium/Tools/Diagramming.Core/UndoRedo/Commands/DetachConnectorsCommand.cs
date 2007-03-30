using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    class DetachConnectorCommand : Command
    {
        IConnector parent;
        IConnector child;

        public override void Redo()
        {
            parent.DetachConnector(child);            
        }
        public override void Undo()
        {
            parent.AttachConnector(child);            
        }

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public DetachConnectorCommand(IController controller, IConnector parent, IConnector child) : base(controller)
        {
            this.parent = parent;
            this.child = child;
        }
        #endregion
  
    }
}
