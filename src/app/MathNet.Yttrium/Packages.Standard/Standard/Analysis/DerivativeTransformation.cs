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
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Patterns.Toolkit;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Manipulation;

namespace MathNet.Symbolics.Packages.Standard.Analysis
{
    public delegate ManipulationPlan EstimateDerivePlan(Port port, Signal variable);
    public delegate IEnumerable<Signal> Derive(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs);

    public class DerivativeTransformation : ITransformationTheorem
    {
        private static readonly MathIdentifier _transformationTypeId = new MathIdentifier("Derive", "Std");
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
        private MathIdentifier _supportedEntityId;
        private Derive _derive;
        private EstimateDerivePlan _plan;
        private Signal _variable;

        public DerivativeTransformation(MathIdentifier supportedEntityId, Derive derive)
            : this(supportedEntityId.DerivePostfix("Derivative"), supportedEntityId, DefaultEstimate, derive) { }
        public DerivativeTransformation(MathIdentifier supportedEntityId, EstimateDerivePlan plan, Derive derive)
            : this(supportedEntityId.DerivePostfix("Derivative"), supportedEntityId, plan, derive) { }
        public DerivativeTransformation(MathIdentifier id, MathIdentifier supportedEntityId, EstimateDerivePlan plan, Derive derive)
        {
            _id = id;
            _supportedEntityId = supportedEntityId;
            _derive = derive;
            _plan = plan;
        }

        public Signal Variable
        {
            get { return _variable; }
            set { _variable = value; }
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
            return new Pattern(new EntityCondition(_supportedEntityId));
        }

        public ManipulationPlan EstimatePlan(Port port, GroupCollection groups)
        {
            return _plan(port, _variable);
        }

        public IEnumerable<Signal> ManipulatePort(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs, GroupCollection groups)
        {
            return _derive(port, manipulatedInputs, _variable, hasManipulatedInputs);
        }

        private static ManipulationPlan DefaultEstimate(Port port, Signal variable)
        {
            return ManipulationPlan.DoAlter;
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            if(original == null)
                throw new ArgumentNullException("original");

            if(!isReplaced)
            {
                if(_variable.Equals(original))
                    return IntegerValue.ConstantOne;
                if(original.IsSourceSignal)
                    return IntegerValue.ConstantZero;
                if(original.Hold)
                {
                    Port p = Service<ILibrary>.Instance.LookupEntity(new MathIdentifier("Derive", "Std")).InstantiatePort(original, _variable);
                    return p.OutputSignals[0];
                }
            }
            return replacement;
        }
    }
}
