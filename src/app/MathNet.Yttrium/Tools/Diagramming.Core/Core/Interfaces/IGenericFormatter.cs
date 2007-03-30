using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Generic formatter interface
    /// </summary>
    public interface IGenericFormatter
    {
        /// <summary>
        /// Deserializes the specified serialization stream.
        /// </summary>
        /// <param name="serializationStream">The serialization stream.</param>
        /// <returns></returns>
        T Deserialize<T>(Stream serializationStream);
        /// <summary>
        /// Serializes the specified serialization stream.
        /// </summary>
        /// <param name="serializationStream">The serialization stream.</param>
        /// <param name="graph">A parameter of the generics Type T</param>
        void Serialize<T>(Stream serializationStream, T graph);
    }
}
