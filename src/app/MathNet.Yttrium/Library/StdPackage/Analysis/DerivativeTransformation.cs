#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;

using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Analysis
{
    public delegate ManipulationPlan EstimateDerivePlan(Port port, Signal variable);
    public delegate IEnumerable<Signal> Derive(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs);

    public class DerivativeTransformation : ITransformationTheorem
    {
        private static readonly MathIdentifier _transformationTypeId = new MathIdentifier("Derive", "Std");
        private readonly MathIdentifier _id;
        private Entity _supportedEntity;
        private Derive _derive;
        private EstimateDerivePlan _plan;
        private Signal _variable;

        public DerivativeTransformation(Entity supportedEntity, Derive derive)
            : this(supportedEntity.EntityId.DerivePostfix("Derivative"), supportedEntity, DefaultEstimate, derive) { }
        public DerivativeTransformation(Entity supportedEntity, EstimateDerivePlan plan, Derive derive)
            : this(supportedEntity.EntityId.DerivePostfix("Derivative"), supportedEntity, plan, derive) { }
        public DerivativeTransformation(MathIdentifier id, Entity supportedEntity, EstimateDerivePlan plan, Derive derive)
        {
            _id = id;
            _supportedEntity = supportedEntity;
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

        public bool SupportsPort(Port port)
        {
            return _supportedEntity.EqualsById(port.Entity);
        }

        public ManipulationPlan EstimatePlan(Port port)
        {
            return _plan(port, _variable);
        }

        public IEnumerable<Signal> ManipulatePort(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
        {
            return _derive(port, manipulatedInputs, _variable, hasManipulatedInputs);
        }

        private static ManipulationPlan DefaultEstimate(Port port, Signal variable)
        {
            return ManipulationPlan.DoAlter;
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            if(!isReplaced)
            {
                if(_variable.Equals(original))
                    return IntegerValue.ConstantOne(_variable.Context);
                if(original.IsSourceSignal)
                    return IntegerValue.ConstantZero(_variable.Context);
                if(original.Hold)
                {
                    Port p = original.Context.Library.LookupEntity(new MathIdentifier("Derive", "Std")).InstantiatePort(original.Context, original, _variable);
                    return p.OutputSignals[0];
                }
            }
            return replacement;
        }
    }
}
