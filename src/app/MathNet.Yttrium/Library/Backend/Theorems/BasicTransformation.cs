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
using MathNet.Symbolics.Backend.Traversing;

namespace MathNet.Symbolics.Backend.Theorems
{
    public class BasicTransformation : ITransformationTheorem
    {
        private readonly MathIdentifier _id;
        private readonly MathIdentifier _transformationTypeId;
        private ManipulatePort _transform;
        private EstimatePlan _plan;
        private Predicate<Port> _supportsPort;

        public BasicTransformation(string label, string domain, string transTypeLabel, string transTypeDomain, Predicate<Port> supportsPort, EstimatePlan plan, ManipulatePort transform)
            : this(new MathIdentifier(label, domain), new MathIdentifier(transTypeLabel, transTypeDomain), supportsPort, plan, transform) { }
        public BasicTransformation(MathIdentifier id, MathIdentifier transformationTypeId, Predicate<Port> supportsPort, EstimatePlan plan, ManipulatePort transform)
        {
            _id = id;
            _transformationTypeId = transformationTypeId;
            _supportsPort = supportsPort;
            _transform = transform;
            _plan = plan;
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
            return _transform(port, manipulatedInputs, hasManipulatedInputs);
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            return replacement;
        }
    }
}
