using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Send forward tool which moves the selection up in the z-order.
    /// </summary>
    class SendForwardsTool : AbstractTool
    {

        #region Fields
     
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SendBackwards"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public SendForwardsTool(string name)
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
            /*
             * A lot of stuff is calculated here for something which might seem at first sight
             * a really simple action. In general the calculations will be short since a user does not
             * usually shift many shapes at the same time and the overlap with the selection
             * is small.
             */
            if(Selection.SelectedItems!=null && Selection.SelectedItems.Count>0)
            {
                
                /*
                 * They should give me a Nobel prize for so much thinking early in the morning...
                 */
                #region Preparation of the ordering
                Debug.Assert(Selection.SelectedItems[0] != null, "A selection cannot contain a 'null' entity.");                
                //the items have to be moved in the order of the Paintables; the SortedList automatically orders things for us.
                SortedList<int, IDiagramEntity> list = new SortedList<int,IDiagramEntity>();
                //We fetch a flattened selection, which means that if there is a group the constituents will be
                //returned rather than the group itself.
                foreach(IDiagramEntity entity in Selection.FlattenedSelectionItems)
                {
                    //the addition will automatically put the item in increasing order
                    list.Add(this.Controller.Model.Paintables.IndexOf(entity), entity);                    
                }
                //if the highest z-value is the last one in the paintables we cannot shift anything, so we quit
                if (list.Keys[list.Count - 1] == this.Controller.Model.Paintables.Count - 1) return;
                
                /*Send them forwards but make sure it's a visible effect!
                It's not enough to move it only once since the shape(s) above might be of
                high z-order degree, so we have to find which is the first shape overlapping with 
                the selection and take as many steps as it takes to surpass it.
                 If there is no overlap we'll shift the z-order with just one unit.
                */
                int delta = 1;
                int lowestZInSelection = list.Keys[0];
                bool found = false;
                //we can speed up the loop by noticing that the next shape in the z-stack is necessarily
                //above the first one of the selection
                for (int m = lowestZInSelection+1; m < this.Controller.Model.Paintables.Count && !found; m++)
                { 
                    //the overlap has to be with an entity, not from the selection
                    if(list.ContainsValue(this.Controller.Model.Paintables[m])) continue;
                    for(int s=0; s<list.Count; s++)
                    {
                        //if there is an overlap we found the required index
                        if (this.Controller.Model.Paintables[m].Rectangle.IntersectsWith(list.Values[s].Rectangle))
                        {
                            //an additional complication here; if the found shape is part of a group we have
                            //to take the upper z-value of the group...
                            if (this.Controller.Model.Paintables[m].Group != null)
                            {
                                int max = -1;
                                CollectionBase<IDiagramEntity> leafs = new CollectionBase<IDiagramEntity>();
                                Utils.TraverseCollect(this.Controller.Model.Paintables[m].Group, ref leafs);
                                foreach (IDiagramEntity groupMember in leafs)
                                {
                                    max = Math.Max(max, this.Controller.Model.Paintables.IndexOf(groupMember));
                                }
                                //take the found z-value of the group rather than the one of the group-child
                                m = max;
                            }
                            delta = m - lowestZInSelection;
                            found = true;
                            break;
                        }

                    }
                }
                #endregion
                Debug.Assert(delta >= 1, "The shift cannot be less than one since we checked previous situations earlier.");
                for (int k = 0; k < list.Count; k++)
                {
                    this.Controller.Model.SendForwards(list.Values[k], delta);
                }
            }
            DeactivateTool();
        }
        
        #endregion


    }

}
