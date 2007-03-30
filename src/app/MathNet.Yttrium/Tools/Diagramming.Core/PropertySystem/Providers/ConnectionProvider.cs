using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Type descriptor provider for all descendants of the <see cref="ConnectionBase"/> classes. 
    /// </summary>
    class ConnectionProvider : TypeDescriptionProvider
    {
        #region Fields
       
        /// <summary>
        /// connection descriptor
        /// </summary>
        private static ConnectionDescriptor connectionDescriptor;

        
        #endregion

        #region Properties
        
        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectionProvider"/> class.
        /// </summary>
        public ConnectionProvider()
        {
            connectionDescriptor = new ConnectionDescriptor(this, typeof(ConnectionBase));
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
            if (typeof(ConnectionBase).IsInstanceOfType(instance))
            {
                return connectionDescriptor;
            }            
            else //if nothing found use the base descriptor
               return base.GetTypeDescriptor(objectType, instance);

       }
        #endregion

   }
  
}
