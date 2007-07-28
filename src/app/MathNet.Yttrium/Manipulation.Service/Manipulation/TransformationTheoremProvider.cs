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

namespace MathNet.Symbolics.Manipulation
{
    public class TransformationTheoremProvider : KeyedCollection<MathIdentifier,ITransformationTheorem>, ITransformationTheoremProvider
    {
        private MathIdentifier _transformationTypeId;
        private ITransformationTheorem _default;

        private CoalescedTreeNode _root;
        private List<CoalescedTreeNode> _tree;

        public TransformationTheoremProvider(MathIdentifier transformationTypeId)
        {
            _transformationTypeId = transformationTypeId;
            _tree = CoalescedTreeNode.CreateRootTree(out _root);
        }

        public MathIdentifier TheoremTypeId
        {
            get { return _transformationTypeId; }
        }

        public ITransformationTheorem Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public CoalescedTreeNode Root
        {
            get { return _root; }
        }

        protected override MathIdentifier GetKeyForItem(ITransformationTheorem item)
        {
            return item.TheoremId;
        }

        protected override void InsertItem(int index, ITransformationTheorem item)
        {
            base.InsertItem(index, item);

            Pattern pattern = item.CreatePattern();
            pattern.MergeToCoalescedTree(item.TheoremId, _tree);

            if(_default == null)
                _default = item;
        }

        public bool TryGetValue(MathIdentifier id, out ITransformationTheorem theorem)
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
        public ITransformationTheorem LookupFirst(Signal output, Port port, out GroupCollection groups)
        {
            Match match = MatchFirst(output, port);
            groups = match.Groups;
            return this[match.PatternId];
        }
        public ITransformationTheorem LookupBest(Signal output, Port port, out GroupCollection groups)
        {
            Match match = MatchBest(output, port);
            groups = match.Groups;
            return this[match.PatternId];
        }
        public bool TryLookupFirst(Signal output, Port port, out ITransformationTheorem theorem, out GroupCollection groups)
        {
            Match match;
            if(TryMatchFirst(output, port, out match))
            {
                groups = match.Groups;
                if(this.TryGetValue(match.PatternId, out theorem))
                    return true;
            }
            groups = null;
            theorem = null;
            return false;
        }
        public bool TryLookupBest(Signal output, Port port, out ITransformationTheorem theorem, out GroupCollection groups)
        {
            Match match;
            if(TryMatchBest(output, port, out match))
            {
                groups = match.Groups;
                if(this.TryGetValue(match.PatternId, out theorem))
                    return true;
            }
            groups = null;
            theorem = null;
            return false;
        }
        #endregion
    }
}
