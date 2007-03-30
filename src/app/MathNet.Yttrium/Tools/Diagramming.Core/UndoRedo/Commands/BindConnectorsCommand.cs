using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    class BindConnectorsCommand : Command
    {
        IConnector parent;
        IConnector child;

        public override void Redo()
        {
            parent.AttachConnector(child);
        }
        public override void Undo()
        {
            parent.DetachConnector(child);
        }

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public BindConnectorsCommand(IController controller, IConnector parent, IConnector child) : base(controller)
        {
            this.parent = parent;
            this.child = child;
        }
        #endregion
  
    }
}
