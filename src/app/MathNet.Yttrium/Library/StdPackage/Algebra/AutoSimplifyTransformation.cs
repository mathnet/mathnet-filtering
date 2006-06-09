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

namespace MathNet.Symbolics.StdPackage.Algebra
{
    public class AutoSimplifyTransformation : ITransformationTheorem
    {
        private static readonly MathIdentifier _transformationTypeId = new MathIdentifier("AutoSimplify", "Std");
        private readonly MathIdentifier _id;
        private ManipulatePort _simplify;
        private EstimatePlan _plan;
        private Predicate<Port> _supportsPort;

        public AutoSimplifyTransformation(Entity supportedEntity, EstimatePlan plan, ManipulatePort simplify)
            : this(supportedEntity.EntityId.DerivePostfix("AutoSimplify"), delegate(Port p) { return supportedEntity.EqualsById(p.Entity); }, plan, simplify) { }
        public AutoSimplifyTransformation(string label, string domain, Predicate<Port> supportsPort, EstimatePlan plan, ManipulatePort simplify)
            : this(new MathIdentifier(label, domain), supportsPort, plan, simplify) { }
        public AutoSimplifyTransformation(MathIdentifier id, Predicate<Port> supportsPort, EstimatePlan plan, ManipulatePort simplify)
        {
            _id = id;
            _supportsPort = supportsPort;
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

        public bool SupportsPort(Port port)
        {
            return _supportsPort(port);
        }

        public ManipulationPlan EstimatePlan(Port port)
        {
            return _plan(port);
        }

        public IEnumerable<Signal> ManipulatePort(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
        {
            return _simplify(port, manipulatedInputs, hasManipulatedInputs);
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            return replacement;
        }
    }
}
