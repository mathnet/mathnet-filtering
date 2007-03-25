#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;

namespace MathNet.Symbolics.Patterns.Toolkit
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

        #region Static Match Helpers
        public static MatchCollection MatchAll(Signal output, Port port, CoalescedTreeNode tree)
        {
            if(tree == null)
                throw new ArgumentNullException("tree");

            return tree.MatchAll(output, port, 1);
        }
        public static Match MatchFirst(Signal output, Port port, CoalescedTreeNode tree)
        {
            if(tree == null)
                throw new ArgumentNullException("tree");

            Match res = tree.MatchFirst(output, port);
            if(res == null)
                throw new MathNet.Symbolics.Exceptions.NotFoundException();
            return res;
        }
        public static bool TryMatchFirst(Signal output, Port port, CoalescedTreeNode tree, out Match match)
        {
            if(tree == null)
                throw new ArgumentNullException("tree");

            match = tree.MatchFirst(output, port);
            return match != null;
        }
        public static Match MatchBest(Signal output, Port port, CoalescedTreeNode tree)
        {
            if(tree == null)
                throw new ArgumentNullException("tree");

            MatchCollection res = tree.MatchAll(output, port, 1);
            Match bestMatch = null;
            int bestScore = -1;
            foreach(Match m in res)
                if(m.Score > bestScore)
                {
                    bestMatch = m;
                    bestScore = m.Score;
                }
            if(bestScore == -1)
                throw new MathNet.Symbolics.Exceptions.NotFoundException();
            return bestMatch;
        }
        public static bool TryMatchBest(Signal output, Port port, CoalescedTreeNode tree, out Match match)
        {
            if(tree == null)
                throw new ArgumentNullException("tree");

            MatchCollection res = tree.MatchAll(output, port, 1);
            Match bestMatch = null;
            int bestScore = -1;
            foreach(Match m in res)
                if(m.Score > bestScore)
                {
                    bestMatch = m;
                    bestScore = m.Score;
                }
            match = bestMatch;
            return bestScore != -1;
        }
        #endregion

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
            if(match.Score > _score)
                _score = match.Score;
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
        public void AppendGroup(string label, IList<Tuple<Signal, Port>> values)
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
