using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class Match : IEnumerable<KeyValuePair<string,List<Tuple<Signal, Port>>>>
    {
        private MathIdentifier _patternId;
        private Dictionary<string, List<Tuple<Signal, Port>>> _groups;

        public Match(MathIdentifier patternId)
        {
            _patternId = patternId;
            _groups = new Dictionary<string, List<Tuple<Signal, Port>>>();
        }

        public MathIdentifier PatternId
        {
            get { return _patternId; }
        }

        public List<Tuple<Signal, Port>> Groups(string label)
        {
            return _groups[label];
        }

        public void AppendGroup(string label, Tuple<Signal, Port> value)
        {
            List<Tuple<Signal, Port>> list;
            if(_groups.TryGetValue(label, out list)) // merge label
                list.Add(value);
            else // add new label
            {
                List<Tuple<Signal, Port>> values = new List<Tuple<Signal,Port>>();
                values.Add(value);
                _groups.Add(label, values);
            }
        }
        public void AppendGroup(string label, List<Tuple<Signal, Port>> values)
        {
            List<Tuple<Signal, Port>> list;
            if(_groups.TryGetValue(label, out list)) // merge label
                list.AddRange(values);
            else // add new label
                _groups.Add(label, values);
        }

        #region IEnumerable Members
        public IEnumerator<KeyValuePair<string, List<Tuple<Signal, Port>>>> GetEnumerator()
        {
            return _groups.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _groups.GetEnumerator();
        }
        #endregion
    }
}
