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

namespace MathNet.Symbolics.Backend.ValueConversion
{
    //public class StructureConverter
    //{
    //    private readonly Type outputType;
    //    private Dictionary<Type, MulticastDelegate> losslessDirectConverters;
    //    private Dictionary<Type, MulticastDelegate> lossyDirectConverters;
    //    private Dictionary<Type, StructureConverter> losslessIndirectConverters;

    //    public StructureConverter(Type outputType)
    //    {
    //        this.outputType = outputType;
    //        losslessDirectConverters = new Dictionary<Type, MulticastDelegate>(4);
    //        lossyDirectConverters = new Dictionary<Type, MulticastDelegate>(0);
    //        losslessIndirectConverters = new Dictionary<Type, StructureConverter>(2);
    //    }

    //    public Type OutputType
    //    {
    //        get { return outputType; }
    //    }

    //    #region Convert
    //    public TOutput Convert<TInput, TOutput>(TInput value)
    //    {
    //        Converter<TInput, TOutput> c;
    //        if(TryGetDirectConverter<TInput, TOutput>(out c))
    //            return c(value);
    //        else
    //        {
    //            Type fromType = typeof(TInput);
    //            StructureConverter vc = losslessIndirectConverters[fromType];
    //            object immediate = vc.Convert(fromType, value);
    //            return (TOutput)losslessDirectConverters[vc.outputType].DynamicInvoke(immediate);
    //        }
    //    }
    //    public TOutput Convert<TInput, TOutput>(TInput value, bool allowPrecisionLoss)
    //    {
    //        Converter<TInput, TOutput> c;
    //        if(TryGetDirectConverter<TInput, TOutput>(out c, allowPrecisionLoss))
    //            return c(value);
    //        else
    //        {
    //            Type fromType = typeof(TInput);
    //            StructureConverter vc = losslessIndirectConverters[fromType];
    //            object immediate = vc.Convert(fromType, value);
    //            return (TOutput)GetDirectConverter(vc.outputType, allowPrecisionLoss).DynamicInvoke(immediate);
    //        }
    //    }
    //    public object Convert(Type fromType, object value)
    //    {
    //        MulticastDelegate md;
    //        if(TryGetDirectConverter(fromType, out md))
    //            return md.DynamicInvoke(value);
    //        else
    //        {
    //            StructureConverter vc = losslessIndirectConverters[fromType];
    //            object immediate = vc.Convert(fromType, value);
    //            return losslessDirectConverters[vc.outputType].DynamicInvoke(immediate);
    //        }
    //    }
    //    public object Convert(Type fromType, object value, bool allowPrecisionLoss)
    //    {
    //        MulticastDelegate md;
    //        if(TryGetDirectConverter(fromType, out md, allowPrecisionLoss))
    //            return md.DynamicInvoke(value);
    //        else
    //        {
    //            StructureConverter vc = losslessIndirectConverters[fromType];
    //            object immediate = vc.Convert(fromType, value);
    //            return GetDirectConverter(vc.outputType, allowPrecisionLoss).DynamicInvoke(immediate);
    //        }
    //    }
    //    #endregion

    //    #region Direct Converters
    //    public bool CanConvertDirectlyFrom(Type fromType)
    //    {
    //        return losslessDirectConverters.ContainsKey(fromType);
    //    }
    //    public bool CanConvertDirectlyFrom(Type fromType, bool allowPrecisionLoss)
    //    {
    //        return allowPrecisionLoss && lossyDirectConverters.ContainsKey(fromType)
    //            ||  losslessDirectConverters.ContainsKey(fromType);
    //    }

    //    public Converter<TInput, TOutput> GetDirectConverter<TInput, TOutput>()
    //    {
    //        return (Converter<TInput, TOutput>)losslessDirectConverters[typeof(TInput)];
    //    }
    //    public Converter<TInput, TOutput> GetDirectConverter<TInput, TOutput>(bool allowPrecisionLoss)
    //    {
    //        Converter<TInput, TOutput> c;
    //        if(TryGetDirectConverter<TInput, TOutput>(out c))
    //            return c;
    //        else
    //            return (Converter<TInput, TOutput>)lossyDirectConverters[typeof(TInput)];
    //    }
    //    public MulticastDelegate GetDirectConverter(Type fromType)
    //    {
    //        return losslessDirectConverters[fromType];
    //    }
    //    public MulticastDelegate GetDirectConverter(Type fromType, bool allowPrecisionLoss)
    //    {
    //        MulticastDelegate md;
    //        if(TryGetDirectConverter(fromType,out md,allowPrecisionLoss))
    //            return md;
    //        else
    //            return lossyDirectConverters[fromType];
    //    }

    //    public bool TryGetDirectConverter<TInput, TOutput>(out Converter<TInput, TOutput> converter)
    //    {
    //        MulticastDelegate md;
    //        if(losslessDirectConverters.TryGetValue(typeof(TInput), out md) && md != null)
    //        {
    //            converter = (Converter<TInput, TOutput>)md;
    //            return true;
    //        }
    //        else
    //        {
    //            converter = null;
    //            return false;
    //        }
    //    }
    //    public bool TryGetDirectConverter<TInput, TOutput>(out Converter<TInput, TOutput> converter, bool allowPrecisionLoss)
    //    {
    //        MulticastDelegate md;
    //        if(losslessDirectConverters.TryGetValue(typeof(TInput), out md) && md != null)
    //        {
    //            converter = (Converter<TInput, TOutput>)md;
    //            return true;
    //        }
    //        else if(allowPrecisionLoss && lossyDirectConverters.TryGetValue(typeof(TInput), out md) && md != null)
    //        {
    //            converter = (Converter<TInput, TOutput>)md;
    //            return true;
    //        }
    //        else
    //        {
    //            converter = null;
    //            return false;
    //        }
    //    }
    //    public bool TryGetDirectConverter(Type fromType, out MulticastDelegate converter)
    //    {
    //        return losslessDirectConverters.TryGetValue(fromType, out converter);
    //    }
    //    public bool TryGetDirectConverter(Type fromType, out MulticastDelegate converter, bool allowPrecisionLoss)
    //    {
    //        if(losslessDirectConverters.TryGetValue(fromType, out converter))
    //            return true;
    //        else if(allowPrecisionLoss && lossyDirectConverters.TryGetValue(fromType, out converter) && converter != null)
    //            return true;
    //        else
    //            return false;
    //    }
    //    #endregion

    //    #region Indirect Converters
    //    public bool CanConvertFrom(Type fromType)
    //    {
    //        return CanConvertDirectlyFrom(fromType) || losslessIndirectConverters.ContainsKey(fromType);
    //    }
    //    public bool CanConvertFrom(Type fromType, bool allowPrecisionLoss)
    //    {
    //        return CanConvertDirectlyFrom(fromType, allowPrecisionLoss) || losslessIndirectConverters.ContainsKey(fromType);
    //    }

    //    public Converter<TInput, TOutput> GetConverter<TInput, TOutput>()
    //    {
    //        return Convert<TInput,TOutput>;
    //    }
    //    public Converter<TInput, TOutput> GetConverter<TInput, TOutput>(bool allowPrecisionLoss)
    //    {
    //        return delegate(TInput value)
    //        {
    //            return Convert<TInput, TOutput>(value, allowPrecisionLoss);
    //        };
    //    }
    //    #endregion
    //}
}
