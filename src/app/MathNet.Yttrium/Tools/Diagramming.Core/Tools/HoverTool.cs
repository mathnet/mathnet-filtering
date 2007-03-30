using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool allows entities to give feedback (e.g. via a change of cursor) when the mouse is hovering over them.
    /// </summary>
    class HoverTool : AbstractTool, IMouseListener
    {

        #region Fields

        private IHoverListener currentListener = null;

        private IDiagramEntity previousHovered = null;


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:HoverTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public HoverTool(string name)
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
            

        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            return false;
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {

            if (!IsSuspended && this.Enabled)
            {
                IHoverListener listener = null;

                CollectionBase<IDiagramEntity> paintables= this.Controller.Model.Paintables;
                IDiagramEntity entity;
                if(paintables.Count==0) return;
                //going from top to the bottom of the z-order
                for (int k=paintables.Count-1; k>=0; k--)
                {
                    entity = paintables[k];
                    if(entity.Rectangle.Contains(e.Location)) //we caught an entity
                    {
                        //unhover the previous, if any
                        if(previousHovered != null)
                            previousHovered.Hovered = false;
                        entity.Hovered = true; //tell the current one it's being hovered
                        //fetch the hovering service, if defined
                        listener = entity.GetService(typeof(IHoverListener)) as IHoverListener;
                        if(listener != null) //the caught entity does listen
                        {
                            if(currentListener == listener) //it's the same as the previous time
                                listener.MouseHover(e);
                            else //we moved from one entity to another listening entity
                            {
                                if(currentListener!=null) //tell the previous entity we are leaving
                                    currentListener.MouseLeave(e);
                                listener.MouseEnter(e); //tell the current one we enter
                                currentListener = listener;
                            }
                        }
                        else //the caught entity does not listen
                        {
                            if(currentListener != null)
                            {
                                currentListener.MouseLeave(e);
                                currentListener = null;
                            }
                        }
                        previousHovered = entity;//remember, for the next time
                        return; //if another entity is listening underneath this entity it will not receive the notification
                    }
                }
                if(currentListener != null)
                {
                    currentListener.MouseLeave(e);
                    currentListener = null;
                }
                //unhover the previous, if any
                if(previousHovered != null)
                    previousHovered.Hovered = false;

            }
        }
      
        public void MouseUp(MouseEventArgs e)
        {
           
        }
        #endregion
    }

}
