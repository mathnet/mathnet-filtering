using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Type descriptor provider for all descendants of the <see cref="ShapeBase"/> classes. 
    /// </summary>
    class ShapeProvider : TypeDescriptionProvider
    {
        #region Fields
        
        /// <summary>
        /// simple shape descriptor
        /// </summary>
        private static SimpleShapeBaseDescriptor simpleShapeBaseDescriptor;
        /// <summary>
        /// complex shape descriptor
        /// </summary>
        private static ComplexShapeBaseDescriptor complexShapeBaseDescriptor;


        private static ClassShapeDescriptor classShapeDescriptor;

        #endregion

        #region Properties


        #endregion

        #region Constructor
        public ShapeProvider()
        {
            simpleShapeBaseDescriptor = new SimpleShapeBaseDescriptor(this, typeof(SimpleShapeBase));
            complexShapeBaseDescriptor = new ComplexShapeBaseDescriptor(this, typeof(ComplexShapeBase));
            classShapeDescriptor = new ClassShapeDescriptor(this, typeof(ClassShape));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"></see>.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"></see> that can provide metadata for the type.
        /// </returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            if (typeof(SimpleShapeBase).IsInstanceOfType(instance))
            {
                return simpleShapeBaseDescriptor;
            }
            else if (typeof(ClassShape).IsInstanceOfType(instance))
            {
                return classShapeDescriptor;
            }
            else if (typeof(ComplexShapeBase).IsInstanceOfType(instance))
            {
                return complexShapeBaseDescriptor;
            }
            else //if nothing found use the base descriptor
                return base.GetTypeDescriptor(objectType, instance);

        }
        #endregion

    }
}

