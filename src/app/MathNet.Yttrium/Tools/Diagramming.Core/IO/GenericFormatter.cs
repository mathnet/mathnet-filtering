using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Generic implementation of the <see cref="IGenericFormatter"/> interface.
    /// </summary>
    /// <typeparam name="F"></typeparam>
    public class GenericFormatter<F> : IGenericFormatter where F : IFormatter, new()
    {
        IFormatter m_Formatter = new F();

        /// <summary>
        /// Deserializes the specified serialization stream.
        /// </summary>
        /// <param name="serializationStream">The serialization stream.</param>
        /// <returns></returns>
        public T Deserialize<T>(Stream serializationStream)
        {
            return (T) m_Formatter.Deserialize(serializationStream);
        }
        /// <summary>
        /// Serializes the specified serialization stream.
        /// </summary>
        /// <param name="serializationStream">The serialization stream.</param>
        /// <param name="graph">A parameter of the generics Type T</param>
        public void Serialize<T>(Stream serializationStream, T graph)
        {
            m_Formatter.Serialize(serializationStream, graph);
        }
    }
    
}
