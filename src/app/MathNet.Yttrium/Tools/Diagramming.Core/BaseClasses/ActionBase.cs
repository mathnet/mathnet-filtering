using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    abstract class ActionBase : ActivityBase, IAction
    {

        #region Fields
        private IModel mModel;
        #endregion

        #region Properties
        public IModel Model
        {
            get
            {
                return mModel;    
            }
            set
            {
                mModel = value;
            }
        }
        #endregion


        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public ActionBase(string name) : base(name)
        {

        }
        #endregion
  
    }
}
