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
using System.Collections.ObjectModel;
using System.Text;

using MathNet.Symbolics.Library;
using MathNet.Symbolics.Patterns.Toolkit;

namespace MathNet.Symbolics.AutoEvaluation
{
    public class AutoEvaluationTheoremProvider<TAspect> : KeyedCollection<MathIdentifier, IAutoEvaluationTheorem<TAspect>>, IAutoEvaluationTheoremProvider<TAspect>
        where TAspect : Identifier<MathIdentifier>
    {
        private MathIdentifier _aspectId;

        private CoalescedTreeNode _root;
        private List<CoalescedTreeNode> _tree;

        public AutoEvaluationTheoremProvider(MathIdentifier aspectId)
        {
            _aspectId = aspectId;
            _tree = CoalescedTreeNode.CreateRootTree(out _root);
        }

        public MathIdentifier TheoremTypeId
        {
            get { return _aspectId; }
        }

        public CoalescedTreeNode Root
        {
            get { return _root; }
        }

        protected override MathIdentifier GetKeyForItem(IAutoEvaluationTheorem<TAspect> item)
        {
            return item.TheoremId;
        }

        protected override void InsertItem(int index, IAutoEvaluationTheorem<TAspect> item)
        {
            base.InsertItem(index, item);

            Pattern pattern = item.CreatePattern();
            pattern.MergeToCoalescedTree(item.TheoremId, _tree);
        }

        public bool TryGetValue(MathIdentifier id, out IAutoEvaluationTheorem<TAspect> theorem)
        {
            theorem = null;
            if(!Contains(id))
                return false;
            theorem = base[id];
            return true;
        }

        #region Pattern Matching
        public MatchCollection MatchAll(Signal output, Port port)
        {
            return _root.MatchAll(output, port, 1);
        }
        public Match MatchFirst(Signal output, Port port)
        {
            Match res = _root.MatchFirst(output, port);
            if(res == null)
                throw new MathNet.Symbolics.Exceptions.NotFoundException();
            return res;
        }
        public bool TryMatchFirst(Signal output, Port port, out Match match)
        {
            match = _root.MatchFirst(output, port);
            return match != null;
        }
        public Match MatchBest(Signal output, Port port)
        {
            Match bestMatch = _root.MatchBest(output, port);
            if(bestMatch == null)
                throw new MathNet.Symbolics.Exceptions.NotFoundException();
            return bestMatch;
        }
        public bool TryMatchBest(Signal output, Port port, out Match match)
        {
            match = _root.MatchBest(output, port);
            return match != null;
        }
        #endregion

        #region Theorem Matching
        public IAutoEvaluationTheorem<TAspect> LookupBest(Signal signal, out GroupCollection groups)
        {
            Match match = MatchBest(signal, signal.DrivenByPort);
            groups = match.Groups;
            return this[match.PatternId];
        }
        public IAutoEvaluationTheorem<TAspect> LookupBest(Port port, out GroupCollection groups)
        {
            Match match = MatchBest(null, port);
            groups = match.Groups;
            return this[match.PatternId];
        }
        public IAutoEvaluationTheorem<TAspect> LookupBest(Bus bus, out GroupCollection groups)
        {
            throw new NotImplementedException();
        }
        public bool TryLookupBest(Signal signal, out IAutoEvaluationTheorem<TAspect> theorem, out GroupCollection groups)
        {
            Match match;
            if(TryMatchBest(signal, signal.DrivenByPort, out match))
            {
                groups = match.Groups;
                if(this.TryGetValue(match.PatternId, out theorem))
                    return true;
            }
            groups = null;
            theorem = null;
            return false;
        }
        public bool TryLookupBest(Port port, out IAutoEvaluationTheorem<TAspect> theorem, out GroupCollection groups)
        {
            Match match;
            if(TryMatchBest(null, port, out match))
            {
                groups = match.Groups;
                if(this.TryGetValue(match.PatternId, out theorem))
                    return true;
            }
            groups = null;
            theorem = null;
            return false;
        }
        public bool TryLookupBest(Bus bus, out IAutoEvaluationTheorem<TAspect> theorem, out GroupCollection groups)
        {
            throw new NotImplementedException();
            //groups = null;
            //theorem = null;
            //return false;
        }
        #endregion
    }
}
