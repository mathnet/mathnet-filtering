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
using System.Xml;

using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Repository;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>symbol: complex infinity</summary>
    public sealed class ComplexInfinitySymbol : ValueStructureBase, IEquatable<ComplexInfinitySymbol>, IComparable<ComplexInfinitySymbol>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("ComplexInfinity", "Std");
        private ComplexInfinitySymbol() { }

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<ComplexInfinitySymbol>(Instance);
            //library.AddCustomDataType(new CustomDataRef(typeof(ComplexInfinitySymbol), ValueConverter<ComplexInfinitySymbol>.Router, Instance));
            ValueConverter<ComplexInfinitySymbol>.AddConverterFrom<NegativeInfinitySymbol>(false, ConvertFrom);
            ValueConverter<ComplexInfinitySymbol>.AddConverterFrom<PositiveInfinitySymbol>(false, ConvertFrom);
        }
        public static ComplexInfinitySymbol ConvertFrom(IValueStructure value)
        {
            return Instance;
        }
        public static ComplexInfinitySymbol ConvertFrom(NegativeInfinitySymbol value) { return Instance; }
        public static explicit operator ComplexInfinitySymbol(NegativeInfinitySymbol value) { return Instance; }
        public static ComplexInfinitySymbol ConvertFrom(PositiveInfinitySymbol value) { return Instance; }
        public static explicit operator ComplexInfinitySymbol(PositiveInfinitySymbol value) { return Instance; }
        public static bool CanConvertLosslessFrom(IValueStructure value)
        {
            return ValueConverter<ComplexInfinitySymbol>.Router.CanConvertLosslessFrom(value);
        }        
        #endregion

        #region Singleton
        private static ComplexInfinitySymbol _instance;
        public static ComplexInfinitySymbol Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new ComplexInfinitySymbol();
                return _instance;
            }
        }
        #endregion

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        public static Signal Constant
        {
            get
            {
                MathIdentifier id = _customTypeId.DerivePostfix("Constant");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Binder.CreateSignal(Instance);
                    ret.Label = "ComplexInfinity";
                    ret.AddConstraint(Properties.ConstantSignalProperty.Instance);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public override bool Equals(IValueStructure other)
        {
            return other is ComplexInfinitySymbol;
        }
        public bool Equals(ComplexInfinitySymbol other)
        {
            return true;
        }
        public int CompareTo(ComplexInfinitySymbol other)
        {
            return 0;
        }

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //}
        //private static ComplexInfinitySymbol InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return ComplexInfinitySymbol._instance;
        //}
        private static ComplexInfinitySymbol Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return ComplexInfinitySymbol._instance;
        }
        #endregion
    }
}
