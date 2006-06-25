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
using MathNet.Symbolics.Backend.Traversing;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Patterns;

namespace MathNet.Symbolics.Backend.Theorems
{
    public delegate void ConfigureTransformation(ITransformationTheorem theorem);

    /// <summary>
    /// Apply a transformation theorem using the manipulation subsystem.
    /// </summary>
    public class TransformationManipulationVisitor : IManipulationVisitor
    {
        TransformationTheoremTypeTable _transformations;
        ConfigureTransformation _configure;

        public TransformationManipulationVisitor(TransformationTheoremTypeTable transformations)
        {
            _transformations = transformations;
        }
        public TransformationManipulationVisitor(TransformationTheoremTypeTable transformations, ConfigureTransformation configure)
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
                throw new MathNet.Symbolics.Backend.Exceptions.TheoremMismatchException();
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
