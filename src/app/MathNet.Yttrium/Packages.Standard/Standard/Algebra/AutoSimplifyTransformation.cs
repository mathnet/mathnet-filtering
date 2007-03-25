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

using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Patterns.Toolkit;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Algebra
{
    public delegate Pattern CreatePattern();

    public class AutoSimplifyTransformation : ITransformationTheorem
    {
        private static readonly MathIdentifier _transformationTypeId = new MathIdentifier("AutoSimplify", "Std");
        private static ITransformationTheoremProvider _provider;

        public static ITransformationTheoremProvider Provider
        {
            get
            {
                if(_provider == null)
                {
                    ITheoremProvider tp;
                    if(Service<ILibrary>.Instance.TryLookupTheoremType(_transformationTypeId, out tp))
                        _provider = (ITransformationTheoremProvider)tp;
                    else
                    {
                        _provider = Binder.GetInstance<ITransformationTheoremProvider, MathIdentifier>(_transformationTypeId);
                        Service<ILibrary>.Instance.AddTheoremType(_provider);
                    }
                }
                return _provider;
            }
        }
        
        private readonly MathIdentifier _id;
        private ManipulatePort _simplify;
        private EstimatePlan _plan;
        private CreatePattern _pattern;

        public AutoSimplifyTransformation(MathIdentifier supportedEntityId, EstimatePlan plan, ManipulatePort simplify)
            : this(supportedEntityId.DerivePostfix("AutoSimplify"), delegate() { return new Pattern(new EntityCondition(supportedEntityId)); }, plan, simplify) { }
        public AutoSimplifyTransformation(MathIdentifier id, CreatePattern pattern, EstimatePlan plan, ManipulatePort simplify)
        {
            _id = id;
            _pattern = pattern;
            _simplify = simplify;
            _plan = plan;
        }

        public static MathIdentifier TransformationTypeIdentifier
        {
            get { return _transformationTypeId; }
        }
        public MathIdentifier TransformationTypeId
        {
            get { return _transformationTypeId; }
        }
        public MathIdentifier TheoremId
        {
            get { return _id; }
        }

        public Pattern CreatePattern()
        {
            return _pattern();
        }

        public ManipulationPlan EstimatePlan(Port port, GroupCollection groups)
        {
            return _plan(port);
        }

        public IEnumerable<Signal> ManipulatePort(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs, GroupCollection groups)
        {
            return _simplify(port, manipulatedInputs, hasManipulatedInputs);
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            return replacement;
        }

    }
}
