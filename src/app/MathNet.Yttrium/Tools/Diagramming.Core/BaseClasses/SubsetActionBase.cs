using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    abstract class SubsetActionBase  : ActionBase, ISubsetAction
    {
        #region Fields
        CollectionBase<IDiagramEntity> mSubset = new CollectionBase<IDiagramEntity>();
        #endregion

        public CollectionBase<IDiagramEntity> Subset
        {
            get
            {
                return mSubset;
            }
            
        }

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public SubsetActionBase(string name):base(name)
        {

        }
        #endregion
  
    }
}
