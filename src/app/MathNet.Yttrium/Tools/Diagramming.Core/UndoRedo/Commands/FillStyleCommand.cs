using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Fill style command
    /// </summary>
    class FillStyleCommand : Command
    {
        #region Fields
        CollectionBase<IDiagramEntity> bundle;
        IController controller;

        IPaintStyle newStyle;
        Dictionary<string, IPaintStyle> previousStyles = new Dictionary<string, IPaintStyle>();           
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
        /// Initializes a new instance of the <see cref="T:FillStyleCommand"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="bundle">The bundle.</param>
        /// <param name="paintStyle">The paint style.</param>
        public FillStyleCommand(IController controller, CollectionBase<IDiagramEntity> bundle, IPaintStyle paintStyle)
            : base(controller)
        {
            this.Text = "Fill style";
            this.controller = controller;
            this.bundle = bundle;//the bundle should contain only IShape and IConnection entities!
            this.newStyle = paintStyle;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Perform redo of this command.
        /// </summary>
        public override void Redo()
        {
            if (bundle == null || bundle.Count == 0)
                return;
            previousStyles.Clear();
            foreach (IDiagramEntity entity in bundle)
            {
                if (entity is IShape)
                {
                    previousStyles.Add(entity.Uid.ToString(), entity.PaintStyle);
                    (entity as IShape).PaintStyle = newStyle;
                }                
            }
        }

        /// <summary>
        /// Perform undo of this command.
        /// </summary>
        public override void Undo()
        {
            if (bundle == null || bundle.Count == 0)
                return;
            foreach (IDiagramEntity entity in bundle)
            {
                if (entity is IShape)
                    (entity as IShape).PaintStyle = previousStyles[entity.Uid.ToString()];
            }

           


        }


        #endregion
    }

}