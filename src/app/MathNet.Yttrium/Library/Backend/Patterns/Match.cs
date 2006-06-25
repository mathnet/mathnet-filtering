using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class Match : IEnumerable<Group>
    {
        private MathIdentifier _patternId;
        private GroupCollection _groups;
        private int _score;

        public Match(MathIdentifier patternId, int score)
        {
            _patternId = patternId;
            _groups = new GroupCollection();
            _score = score;
        }

        public MathIdentifier PatternId
        {
            get { return _patternId; }
        }

        public int Score
        {
            get { return _score; }
        }

        public void MergeWithMatch(Match match)
        {
            if(match._score > _score)
                _score = match._score;
            foreach(Group group in match)
                AppendGroup(group);
        }

        public Group this[string groupLabel]
        {
            get { return _groups[groupLabel]; }
        }

        public int GroupCount
        {
            get { return _groups.Count; }
        }

        public GroupCollection Groups
        {
            get { return _groups; }
        }

        public void AppendGroup(string label, Tuple<Signal, Port> value)
        {
            _groups.Append(label, value);
        }
        public void AppendGroup(string label, List<Tuple<Signal, Port>> values)
        {
            _groups.Append(label, values);
        }
        public void AppendGroup(Group group)
        {
            _groups.Append(group);
        }

        #region IEnumerable Members
        public IEnumerator<Group> GetEnumerator()
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
