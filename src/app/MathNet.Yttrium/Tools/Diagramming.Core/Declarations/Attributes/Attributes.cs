using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Abstract base class for the attributes related to serialization
    /// </summary>
    public abstract class BaseAttribute : System.Attribute
    {
        #region Fields
        /// <summary>
        /// the key of the shape, usually a GUID
        /// </summary>
        protected string mKey;
        /// <summary>
        /// the name of the shape
        /// </summary>
        protected string mName;       
        /// <summary>
        /// a description
        /// </summary>
        protected string mDescription = "No description available.";
        /// <summary>
        /// whether the shape is only accessible via code or internally
        /// </summary>
        protected bool mIsInternal;
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets a mDescription of the shape
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }
        /// <summary>
        /// Gets the unique identifier of the shape
        /// </summary>
        public string Key
        {
            get { return mKey; }
        }
        /// <summary>
        /// Gets the shape name
        /// </summary>
        public string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// Gets whether the entity is available via the interface or false if only via code
        /// </summary>
        public bool IsInternal
        {
            get { return mIsInternal; }
        }
        #endregion
    }
    /// <summary>
    /// Attribute to tag a class as a diagram shape
    /// </summary>
    [Serializable, AttributeUsage(AttributeTargets.Class)]
    public class ShapeAttribute : BaseAttribute
    {
        #region Fields

        /// <summary>
        /// the cateogry under which it will stay
        /// </summary>
        protected string mCategory;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the category of the shape under which it will reside in a viewer
        /// </summary>
        public string Category
        {
            get { return mCategory; }

        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor, marks a class as a shape-class for the Netron graph library
        /// </summary>
        /// <param name="mShapeName"></param>
        /// <param name="mShapeKey"></param>
        /// <param name="mCategory"></param>
        /// <param name="reflectionName"></param>
        public ShapeAttribute(string shapeName, string shapeKey, string category)
        {
            this.mName = shapeName;
            this.mKey = shapeKey;
            this.mCategory = category;
        }
        /// <summary>
        /// Constructor, marks a class as a shape-class for the Netron graph library
        /// </summary>
        /// <param name="mShapeName"></param>
        /// <param name="mShapeKey"></param>
        /// <param name="mCategory"></param>
        /// <param name="reflectionName"></param>
        /// <param name="mDescription"></param>
        public ShapeAttribute(string shapeName, string shapeKey, string category, string description) 
            : this(shapeName, shapeKey, category)
        {
            this.mDescription = description;
        }
        /// <summary>
        /// Constructor, marks a class as a shape-class for the Netron graph library
        /// </summary>
        /// <param name="mShapeName"></param>
        /// <param name="mShapeKey"></param>
        /// <param name="mCategory"></param>
        /// <param name="reflectionName"></param>
        /// <param name="mDescription"></param>
        /// <param name="internalUsage"></param>
        public ShapeAttribute(string shapeName, string shapeKey, string category, string description, bool internalUsage)
            : this(shapeName, shapeKey, category, description)
        {
            this.mIsInternal = internalUsage;
        }
        #endregion

        #region Methods


        #endregion
    }

    /// <summary>
    /// Attribute to tag a class as a Netron connection
    /// </summary>
    [Serializable]
    public class ConnectionAttribute : BaseAttribute
    {
        #region Fields


        #endregion

        #region Properties


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="key"></param>
        /// <param name="reflectionName"></param>
        public ConnectionAttribute(string connectionName, string key)
        {
            this.mKey = key;
            this.mName = connectionName;
        }

        #endregion

        #region Methods


        #endregion
    }
}
