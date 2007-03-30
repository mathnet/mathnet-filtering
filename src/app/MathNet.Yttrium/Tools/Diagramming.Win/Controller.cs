using System;
using System.Collections.Generic;
using System.Text;
using Netron.Diagramming.Core;
namespace Netron.Diagramming.Win
{
    /// <summary>
    /// WinForm implementation of the <see cref="IController"/> interface.
    /// </summary>
    class Controller : ControllerBase
    {

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public Controller(IDiagramControl surface) : base(surface)
        {
            this.AddTool(new TextTool("Text Tool"));
        }
        #endregion
  
    }
}
