using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Backend.Traversing
{
    /// <returns>true to continue, false if finished.</returns>
    public delegate bool ActionContinue<T>(T obj);
}
