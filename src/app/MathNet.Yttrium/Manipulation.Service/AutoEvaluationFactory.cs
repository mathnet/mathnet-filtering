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

using MathNet.Symbolics.AutoEvaluation;

namespace MathNet.Symbolics
{
    internal class AutoEvaluationFactory :
        IFactory<IAutoEvaluator>,
        IFactory<IAutoEvaluationTheoremProvider<NodeFlag>, MathIdentifier>,
        IFactory<IAutoEvaluationTheoremProvider<NodeProperty>, MathIdentifier>
    {
        IAutoEvaluator IFactory<IAutoEvaluator>.GetInstance()
        {
            return new AutoEvaluator();
        }

        IAutoEvaluationTheoremProvider<NodeFlag> IFactory<IAutoEvaluationTheoremProvider<NodeFlag>>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: MathIdentifier");
        }
        IAutoEvaluationTheoremProvider<NodeFlag> IFactory<IAutoEvaluationTheoremProvider<NodeFlag>, MathIdentifier>.GetInstance(MathIdentifier p1)
        {
            return new AutoEvaluationTheoremProvider<NodeFlag>(p1);
        }

        IAutoEvaluationTheoremProvider<NodeProperty> IFactory<IAutoEvaluationTheoremProvider<NodeProperty>>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: MathIdentifier");
        }
        IAutoEvaluationTheoremProvider<NodeProperty> IFactory<IAutoEvaluationTheoremProvider<NodeProperty>, MathIdentifier>.GetInstance(MathIdentifier p1)
        {
            return new AutoEvaluationTheoremProvider<NodeProperty>(p1);
        }
    }
}
