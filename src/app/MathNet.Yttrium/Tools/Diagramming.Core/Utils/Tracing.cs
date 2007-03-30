using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace Netron.Diagramming.Core
{
    static class Tracing
    {
        #region Fields
        /// <summary>
        /// the serialization switch
        /// </summary>
        private static BooleanSwitch mBinarySerializationSwitch = new BooleanSwitch("BinarySerializationSwitch", "This switch enables/disables the ouput of binary serialization information.");
        /// <summary>
        /// the deserialization switch
        /// </summary>
        private static BooleanSwitch mBinaryDeserializationSwitch = new BooleanSwitch("BinaryDeserializationSwitch", "This switch enables/disables the ouput of binary deserialization information.");
        #endregion

        #region Properties
        /// <summary>
        /// Gets the deserialization switch.
        /// </summary>
        /// <value>The deserialization switch.</value>
        public static BooleanSwitch BinaryDeserializationSwitch
        {
            get
            {
                return mBinaryDeserializationSwitch;
            }
        }
        /// <summary>
        /// Gets the serialization switch.
        /// </summary>
        /// <value>The serialization switch.</value>
        public static BooleanSwitch BinarySerializationSwitch
        {
            get
            {
                return mBinarySerializationSwitch;
            }
        }
        #endregion


    }
}
