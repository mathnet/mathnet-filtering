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
    /// <summary>symbol: negative real infinity</summary>
    public sealed class NegativeInfinitySymbol : ValueStructureBase, IEquatable<NegativeInfinitySymbol>, IComparable<NegativeInfinitySymbol>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("NegativeInfinity", "Std");
        private NegativeInfinitySymbol() { }

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<NegativeInfinitySymbol>(Instance);
            //library.AddCustomDataType(new CustomDataRef(typeof(NegativeInfinitySymbol), ValueConverter<NegativeInfinitySymbol>.Router, Instance));
        }
        public static NegativeInfinitySymbol ConvertFrom(IValueStructure value)
        {
            return Instance;
        }
        public static bool CanConvertLosslessFrom(IValueStructure value)
        {
            return ValueConverter<NegativeInfinitySymbol>.Router.CanConvertLosslessFrom(value);
        }
        #endregion

        #region Singleton
        private static NegativeInfinitySymbol _instance;
        public static NegativeInfinitySymbol Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new NegativeInfinitySymbol();
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
                    ret.Label = "NegativeInfinity";
                    ret.AddConstraint(Properties.ConstantSignalProperty.Instance);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public override bool Equals(IValueStructure other)
        {
            return other is NegativeInfinitySymbol;
        }
        public bool Equals(NegativeInfinitySymbol other)
        {
            return true;
        }
        public int CompareTo(NegativeInfinitySymbol other)
        {
            return 0;
        }

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //}
        //private static NegativeInfinitySymbol InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return NegativeInfinitySymbol._instance;
        //}
        private static NegativeInfinitySymbol Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return NegativeInfinitySymbol._instance;
        }
        #endregion
    }
}
