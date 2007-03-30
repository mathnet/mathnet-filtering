using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Ungroup command
    /// </summary>
    class UngroupCommand : Command
    {
        #region Fields
        IBundle bundle;
        IController controller;
        IGroup mGroup;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the newly created group after <see cref="Redo"/> was called
        /// </summary>
        public IGroup Group
        {
            get { return mGroup; }
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <value>The shape.</value>
        public CollectionBase<IDiagramEntity> Entities
        {
            get { return bundle.Entities; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UngroupCommand"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="group">The group.</param>
        public UngroupCommand(IController controller, IGroup group)
            : base(controller)
        {
            this.Text = "Ungroup";
            this.controller = controller;
            this.mGroup = group;
            bundle = new Bundle();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Perform redo of this command.
        /// </summary>
        public override void Redo()
        {
            //remove the group from the layer
            this.Controller.Model.DefaultPage.DefaultLayer.Entities.Remove(mGroup);
            //detach the entities from the group
            foreach (IDiagramEntity entity in mGroup.Entities)
            {
                //this will be recursive if an entity is itself an IGroup
                entity.Group = null;
                bundle.Entities.Add(entity);
                //mGroup.Entities.Remove(entity);
            }
            //change the visuals such that the entities in the group are selected

            CollectionBase<IDiagramEntity> col = new CollectionBase<IDiagramEntity>();
            col.AddRange(mGroup.Entities);
            
            Selection.SelectedItems = col;

            mGroup.Invalidate();
            mGroup = null;

            //note that the entities have never been disconnected from the layer
            //so they don't have to be re-attached to the anything.
            //The insertion of the Group simply got pushed in the scene-graph.


        }

        /// <summary>
        /// Perform undo of this command.
        /// </summary>
        public override void Undo()
        {

            //create a new group
            GroupShape group = new GroupShape(this.Controller.Model);
            //asign the entities to the group
            group.Entities = bundle.Entities;

            foreach (IDiagramEntity entity in group.Entities)
            {
                //this will be recursive if an entity is itself an IGroup
                entity.Group = group;
            }
            //add the new group to the layer
            this.Controller.Model.DefaultPage.DefaultLayer.Entities.Add(group);

            mGroup = group;

            //select the newly created group
            CollectionBase<IDiagramEntity> col = new CollectionBase<IDiagramEntity>();
            col.Add(mGroup);
            Selection.SelectedItems = col;
            mGroup.Invalidate();

         
        }


        #endregion
    }

}