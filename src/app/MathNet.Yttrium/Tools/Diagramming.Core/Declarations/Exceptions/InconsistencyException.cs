using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This exception is rather generic in the sense that it's being used a bit everywhere in the code.
    /// A more refined division could be constructed but since this is open source it's easy enough to 
    /// pin down the problem and a complex exception handling is not necessary.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable()]
    public sealed class InconsistencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:InconsistencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InconsistencyException(string message) : base(message){ }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:InconsistencyException"/> class.
        /// </summary>
        public InconsistencyException() : base(){}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:InconsistencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        public InconsistencyException(string message, Exception e) :  base(message, e){}
    }
}
