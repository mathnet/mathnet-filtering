using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Ungroup command
    /// </summary>
    class DeleteCommand : Command
    {
        #region Fields
        CollectionBase<IDiagramEntity> bundle;
        IController controller;
      
        #endregion

        #region Properties
       

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <value>The shape.</value>
        public CollectionBase<IDiagramEntity> Entities
        {
            get { return bundle; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DeleteCommand"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="bundle">The bundle.</param>
        public DeleteCommand(IController controller, CollectionBase<IDiagramEntity> bundle)
            : base(controller)
        {
            this.Text = "Delete";
            this.controller = controller;
            this.bundle = bundle;
            
        }
        #endregion

        #region Methods

        /// <summary>
        /// Perform redo of this command.
        /// </summary>
        public override void Redo()
        {
            if (bundle.Count == 0) return;

            foreach(IDiagramEntity entity in bundle)
                this.Controller.Model.DefaultPage.DefaultLayer.Entities.Remove(entity);
            this.Controller.View.HideTracker();
            Rectangle rec = Utils.BoundingRectangle(bundle);
            rec.Inflate(30, 30);
            this.Controller.View.Invalidate(rec);

        }

        /// <summary>
        /// Perform undo of this command.
        /// </summary>
        public override void Undo()
        {

            if (bundle.Count == 0) return;

            foreach (IDiagramEntity entity in bundle)
                this.Controller.Model.DefaultPage.DefaultLayer.Entities.Add(entity);
            this.Controller.View.HideTracker();

            Rectangle rec = Utils.BoundingRectangle(bundle);
            rec.Inflate(30, 30);
            this.Controller.View.Invalidate(rec);
                       

         
        }


        #endregion
    }

}