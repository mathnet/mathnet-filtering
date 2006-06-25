using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;
using MathNet.Symbolics.Backend.Patterns;

namespace MathNet.Symbolics.Backend.Containers
{
    public sealed class TransformationTheoremTypeTable
    {
        private MathIdentifier _transformationTypeId;
        private IdentifierDictionary<ITransformationTheorem> _theorems;
        private ITransformationTheorem _default;
        private CoalescedTreeNode _root;
        private List<CoalescedTreeNode> _tree;

        public TransformationTheoremTypeTable(MathIdentifier typeId)
        {
            _transformationTypeId = typeId;
            _theorems = new IdentifierDictionary<ITransformationTheorem>(4, 16);
            _tree = CoalescedTreeNode.CreateRootTree(out _root);
        }

        public MathIdentifier TransformationTypeId
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

        public ITransformationTheorem this[MathIdentifier theoremId]
        {
            get { return _theorems.GetValue(theoremId); }
        }

        public bool TryGetValue(MathIdentifier id, out ITransformationTheorem theorem)
        {
            return _theorems.TryGetValue(id, out theorem);
        }

        public bool ContainsTheorem(MathIdentifier theoremId)
        {
            return _theorems.ContainsId(theoremId);
        }
        public bool ContainsTheorem(ITransformationTheorem theorem)
        {
            return _theorems.ContainsId(theorem.TheoremId);
        }

        public void AddTheorem(ITransformationTheorem theorem)
        {
            System.Diagnostics.Debug.Assert(theorem.TransformationTypeId.Equals(_transformationTypeId));
            _theorems.Add(theorem.TheoremId, theorem);
            Pattern pattern = theorem.CreatePattern();
            pattern.MergeToCoalescedTree(theorem.TheoremId, _tree);
            if(_default == null)
                _default = theorem;
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
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
            return res;
        }
        public bool TryMatchFirst(Signal output, Port port, out Match match)
        {
            match = _root.MatchFirst(output, port);
            return match != null;
        }
        public Match MatchBest(Signal output, Port port)
        {
            MatchCollection res = _root.MatchAll(output, port, 1);
            Match bestMatch = null;
            int bestScore = -1;
            foreach(Match m in res)
                if(m.Score > bestScore)
                {
                    bestMatch = m;
                    bestScore = m.Score;
                }
            if(bestScore == -1)
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
            return bestMatch;
        }
        public bool TryMatchBest(Signal output, Port port, out Match match)
        {
            MatchCollection res = _root.MatchAll(output, port, 1);
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

        #region Theorem Matching
        public ITransformationTheorem LookupFirst(Signal output, Port port, out GroupCollection groups)
        {
            Match match = MatchFirst(output, port);
            groups = match.Groups;
            return _theorems.GetValue(match.PatternId);
        }
        public ITransformationTheorem LookupBest(Signal output, Port port, out GroupCollection groups)
        {
            Match match = MatchBest(output, port);
            groups = match.Groups;
            return _theorems.GetValue(match.PatternId);
        }
        public bool TryLookupFirst(Signal output, Port port, out ITransformationTheorem theorem, out GroupCollection groups)
        {
            Match match;
            if(TryMatchFirst(output, port, out match))
            {
                groups = match.Groups;
                if(_theorems.TryGetValue(match.PatternId, out theorem))
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
                if(_theorems.TryGetValue(match.PatternId, out theorem))
                    return true;
            }
            groups = null;
            theorem = null;
            return false;
        }
        #endregion
    }
}
