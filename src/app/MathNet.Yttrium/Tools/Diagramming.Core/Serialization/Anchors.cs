using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    public struct Anchor
    {
        
        /// <summary>
        /// the Parent field
        /// </summary>
        private Guid mParent;
        /// <summary>
        /// Gets or sets the Parent
        /// </summary>
	      public Guid Parent
	      {
		      get { return mParent;}
		      set { mParent = value;}
	      }

        
        /// <summary>
        /// the Instance field
        /// </summary>
        private IConnector mInstance;
        /// <summary>
        /// Gets or sets the Instance
        /// </summary>
	      public IConnector Instance
	      {
		      get { return mInstance;}
		      set { mInstance = value;}
	      }


          #region Constructor
          ///<summary>
          ///Default constructor
          ///</summary>
          public Anchor(Guid parent, IConnector instance)
          {
              this.mParent = parent;
              this.mInstance = instance;
          }
          #endregion
  

    }
    public static class Anchors 
    {
        private static Dictionary<Guid, Anchor> innerList = new Dictionary<Guid, Anchor>();

        public static void Clear()
        {
            innerList.Clear();
        }

        public static void Add(Guid uid, Anchor anchor)
        {
            innerList.Add(uid, anchor);
        }

        public static bool ContainsKey(Guid uid)
        {
            return innerList.ContainsKey(uid);
        }

        public static Dictionary<Guid, Anchor>.Enumerator GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        public static Anchor GetAnchor(Guid uid)
        {
                return innerList[uid];
        }
    }
}
