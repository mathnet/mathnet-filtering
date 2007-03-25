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
using MathNet.Symbolics.Patterns.Toolkit;
using MathNet.Symbolics.Traversing;

namespace MathNet.Symbolics.Manipulation
{
    /// <summary>
    /// Apply a transformation theorem using the manipulation subsystem.
    /// </summary>
    public class TransformationManipulationVisitor : IManipulationVisitor
    {
        ITransformationTheoremProvider _transformations;
        ConfigureTransformation _configure;

        public TransformationManipulationVisitor(ITransformationTheoremProvider transformations)
        {
            _transformations = transformations;
        }
        public TransformationManipulationVisitor(ITransformationTheoremProvider transformations, ConfigureTransformation configure)
        {
            _transformations = transformations;
            _configure = configure;
        }

        public ManipulationPlan EstimatePlan(Port port)
        {
            ITransformationTheorem trans;
            GroupCollection groups;
            if(_transformations.TryLookupBest(null, port, out trans, out groups))
            {
                if(_configure != null)
                    _configure(trans);
                return trans.EstimatePlan(port, groups);
            }
            else
                return ManipulationPlan.CloneIfChildsAltered;
        }

        public IEnumerable<Signal> ManipulatePort(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
        {
            ITransformationTheorem trans;
            GroupCollection groups;
            if(_transformations.TryLookupBest(null, port, out trans, out groups))
            {
                if(_configure != null)
                    _configure(trans);
                return trans.ManipulatePort(port, manipulatedInputs, hasManipulatedInputs, groups);
            }
            else
                throw new MathNet.Symbolics.Exceptions.TheoremMismatchException();
        }

        public Signal ManipulateSignal(Signal original, Signal replacement, bool isReplaced)
        {
            ITransformationTheorem trans = _transformations.Default;
            if(trans != null)
            {
                if(_configure != null)
                    _configure(trans);
                return trans.ManipulateSignal(original, replacement, isReplaced);
            }
            return replacement;
        }
    }
}
