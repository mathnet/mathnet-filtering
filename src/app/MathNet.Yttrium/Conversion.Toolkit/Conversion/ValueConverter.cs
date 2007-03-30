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

namespace MathNet.Symbolics.Conversion
{
    public static class ValueConverter<TTarget>
    {
        //lazy initialization, since there are no other field members
        private static readonly IConversionRouter _router = Binder.GetInstance<IConversionRouter, Type>(typeof(TTarget));

        //// Explicit static constructor to tell C# compiler
        //// not to mark type as beforefieldinit
        //static ValueConverter()
        //{
        //}

        public static TTarget ConvertFrom(ICustomData value)
        {
            return (TTarget)_router.ConvertFrom(value);
        }

        public static TTarget ConvertFrom(object value)
        {
            return (TTarget)_router.ConvertFrom(value);
        }

        public static IConversionRouter Router
        {
            get { return _router; }
        }

        public static void AddConverterFrom(IConversionRouter sourceRouter, bool lossless, Converter<object, object> directConvert)
        {
            _router.AddSourceNeighbor(sourceRouter, lossless, directConvert);
        }

        public static void AddConverterTo(IConversionRouter destinationRouter, bool lossless, Converter<object, object> directConvert)
        {
            destinationRouter.AddSourceNeighbor(_router, lossless, directConvert);
        }

        public static void AddConverterFrom<TSource>(bool lossless, Converter<TSource, TTarget> directConvert)
            where TSource : ICustomData
        {
            // TODO: Modify conversion core to operate directly on typed converters
            _router.AddSourceNeighbor(ValueConverter<TSource>.Router, lossless, delegate(object value) { return directConvert((TSource)value); });
        }

        public static void AddConverterTo<TDestination>(bool lossless, Converter<TTarget, TDestination> directConvert)
            where TDestination : ICustomData
        {
            // TODO: Modify conversion core to operate directly on typed converters
            ValueConverter<TDestination>.Router.AddSourceNeighbor(_router, lossless, delegate(object value) { return directConvert((TTarget)value); });
        }
    }
}
