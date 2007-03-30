using System;
using System.Collections.Generic;
using System.Text;
using Netron.Diagramming.Core.Analysis;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class providing convenience methods for tree layout algorithms.
    /// </summary>
    abstract class TreeLayoutBase : LayoutBase
    {

        #region Fields
        /// <summary>
        /// the root of the tree
        /// </summary>
        private IShape mLayoutRoot;
    
        #endregion

        #region Consstructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TreeLayoutBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="controller">The controller.</param>
        protected TreeLayoutBase(string name, IController controller)
            : base(name, controller)
        {

        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the layout root.
        /// </summary>
        /// <value>The layout root.</value>
        public IShape LayoutRoot
        {
            get
            {
                return mLayoutRoot;
            }
            set
            {
                mLayoutRoot = value;
                
            }
        }
     
        #endregion
        
    }
}