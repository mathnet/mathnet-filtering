using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Group tool
    /// </summary>
    class SendToFrontTool : AbstractTool
    {

        #region Fields
     
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SendToFrontTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public SendToFrontTool(string name)
            : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            if(Selection.SelectedItems != null && Selection.SelectedItems.Count > 0)
            {
                #region Preparation of the ordering
                //the items have to be moved in the order of the Paintables
                //Usually this is a good moment to make a little drawing or example
                //to see how things function.

                SortedList<int, IDiagramEntity> list = new SortedList<int, IDiagramEntity>();
                foreach(IDiagramEntity entity in Selection.FlattenedSelectionItems)
                {
                    //the addition will automatically put the item in increasing order
                    list.Add(this.Controller.Model.Paintables.IndexOf(entity), entity);
                }
                #endregion
                //send them backwards
                for(int k = 0; k < list.Count; k++)
                {
                    this.Controller.Model.SendToFront(list.Values[k]);
                }
            }
            DeactivateTool();
        }
        
        #endregion


    }

}
