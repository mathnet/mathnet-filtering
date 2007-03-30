using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Pen style command
    /// </summary>
    class PenStyleCommand : Command
    {
        #region Fields
        CollectionBase<IDiagramEntity> bundle;
        IController controller;

        IPenStyle newStyle;
        Dictionary<string, IPenStyle> previousStyles = new Dictionary<string, IPenStyle>();           
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
        /// Initializes a new instance of the <see cref="T:PenStyleCommand"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="bundle">The bundle.</param>
        /// <param name="penStyle">The pen style.</param>
        public PenStyleCommand(IController controller, CollectionBase<IDiagramEntity> bundle, IPenStyle penStyle)
            : base(controller)
        {
            this.Text = "Fill style";
            this.controller = controller;
            this.bundle = bundle;//the bundle should contain only IShape and IConnection entities!
            this.newStyle = penStyle;
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
                if (entity is IConnection)
                {
                    previousStyles.Add(entity.Uid.ToString(), entity.PenStyle);
                    (entity as IConnection).PenStyle  = newStyle;
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
                if (entity is IConnection)
                    (entity as IConnection).PenStyle = previousStyles[entity.Uid.ToString()];
            }

           


        }


        #endregion
    }

}