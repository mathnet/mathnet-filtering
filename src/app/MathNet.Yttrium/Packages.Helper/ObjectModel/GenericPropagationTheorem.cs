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

namespace MathNet.Symbolics.Packages.ObjectModel
{
    //public delegate bool ImpliesProperty(Signal signal);
    //public delegate void BuildProperty(Signal signal);

    //public class GenericPropagationTheorem : IPropagationTheorem
    //{
    //    private readonly MathIdentifier id;
    //    private readonly MathIdentifier providedPropertyId;
    //    private readonly ImpliesProperty impliesProperty;
    //    private readonly BuildProperty buildProperty;

    //    public GenericPropagationTheorem(MathIdentifier id, MathIdentifier providedPropertyId, ImpliesProperty impliesProperty, BuildProperty buildProperty)
    //    {
    //        this.id = id;
    //        this.providedPropertyId = providedPropertyId;
    //        this.impliesProperty = impliesProperty;
    //        this.buildProperty = buildProperty;
    //    }

    //    public bool WouldBePropagatedTo(Signal target)
    //    {
    //        return impliesProperty(target);
    //    }

    //    public bool PropagatePropertyIfApplicable(Signal target)
    //    {
    //        if(target == null) throw new ArgumentNullException("target");
    //        if(!target.HasProperty(providedPropertyId) && WouldBePropagatedTo(target))
    //            buildProperty(target);
    //        return target.HasProperty(providedPropertyId);
    //    }

    //    public MathIdentifier PropertyTypeId
    //    {
    //        get { return providedPropertyId; }
    //    }
    //    public MathIdentifier TheoremId
    //    {
    //        get { return id; }
    //    }
    //}
}
