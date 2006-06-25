using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class Group : Collection<Tuple<Signal, Port>>
    {
        private string _label;
        public Group(string label)
            : base()
        {
            _label = label;
        }

        public string Label
        {
            get { return _label; }
        }

        public void AddRange(IEnumerable<Tuple<Signal, Port>> range)
        {
            foreach(Tuple<Signal, Port> item in range)
                Add(item);
        }
    }
}
