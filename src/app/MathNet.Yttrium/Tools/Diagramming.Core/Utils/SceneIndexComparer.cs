using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This <see cref="IComparer"/> compares two diagram entities with respect to their scene index.
    /// A diagram entity is 'less' than another if the scene index is smaller. Note that two distinct entities can never
    /// have the same scene index. Since this comparer is used in the first place to re-order the <see cref="IPaintable"/> collection
    /// it will throw an exception if two indices are found to be the same.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class SceneIndexComparer<T> : IComparer<T> where T:IDiagramEntity
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zerox is less than y.Zerox equals y.Greater than zerox is greater than y.
        /// </returns>
        public int Compare(T x, T y)
        {
            if(x.SceneIndex == y.SceneIndex)
                throw new InconsistencyException("Inconsistent stack: two entities had the same scene index.");

            if(x.SceneIndex < y.SceneIndex)
                return -1;
            else
                return +1;
        }


    }
}
