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
using System.Xml;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.Standard.Trigonometry;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>vectors with arbitrary components</summary>
    public class VectorValue<TScalar> : ValueStructureBase, IEquatable<VectorValue<TScalar>>, IAlgebraicVectorSpace<VectorValue<TScalar>, TScalar>
        where TScalar : IValueStructure, IAlgebraicField<TScalar>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Vector", "Std").DerivePrefix(typeof(TScalar).Name);
        private readonly TScalar[] _dataValue;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<VectorValue<TScalar>>();
            //library.AddCustomDataType(new CustomDataRef(typeof(VectorValue<TScalar>), ValueConverter<VectorValue<TScalar>>.Router));
        }
        public static VectorValue<TScalar> ConvertFrom(ICustomData value)
        {
            return (VectorValue<TScalar>)ValueConverter<VectorValue<TScalar>>.ConvertFrom(value);
        }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<VectorValue<TScalar>>.Router.CanConvertLosslessFrom(value);
        }        
        #endregion

        #region Constructors
        public VectorValue()
        {
            _dataValue = new TScalar[0];
        }
        public VectorValue(TScalar[] value)
        {
            _dataValue = value;
        }
        public VectorValue(IList<TScalar> values)
        {
            _dataValue = new TScalar[values.Count];
            values.CopyTo(_dataValue,0);
        }
        #endregion

        public TScalar[] Value
        {
            get { return _dataValue; }
        }
        public TScalar this[int index]
        {
            get { return _dataValue[index]; }
        }
        public int Count
        {
            get { return _dataValue.Length; }
        }
        //public double Norm
        //{
        //    get { return _dataValue.Modulus; }
        //}
        //public double NormSquared
        //{
        //    get { return _dataValue.ModulusSquared; }
        //}
        //public bool IsZero
        //{
        //    get { return _dataValue.IsZero; }
        //}

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        #region Direct Function Implementations
        #region Arithmetic Function
        public static VectorValue<TScalar> Add(VectorValue<TScalar> summand1, VectorValue<TScalar> summand2) { return summand1.Add(summand2); }
        public VectorValue<TScalar> Add(VectorValue<TScalar> summand)
        {
            int max = Math.Max(Count, summand.Count);
            int min = Math.Min(Count, summand.Count);
            TScalar[] otherValue = summand._dataValue;
            TScalar[] value = new TScalar[max];

            if(Count > summand.Count)
                Array.Copy(_dataValue,min,value,min,max-min);
            else
                Array.Copy(otherValue, min, value, min, max - min);
            for(int i = 0; i < min; i++)
                value[i] = _dataValue[i].Add(otherValue[i]);

            return new VectorValue<TScalar>(value);
        }
        public static VectorValue<TScalar> Subtract(VectorValue<TScalar> minuend, VectorValue<TScalar> subtrahend) { return minuend.Subtract(subtrahend); }
        public VectorValue<TScalar> Subtract(VectorValue<TScalar> subtrahend)
        {
            int max = Math.Max(Count, subtrahend.Count);
            int min = Math.Min(Count, subtrahend.Count);
            TScalar[] otherValue = subtrahend._dataValue;
            TScalar[] value = new TScalar[max];

            if(Count > subtrahend.Count)
                Array.Copy(_dataValue, min, value, min, max - min);
            else
                for(int i = min; i < max; i++)
                    value[i] = otherValue[i].Negate();
            for(int i = 0; i < min; i++)
                value[i] = _dataValue[i].Subtract(otherValue[i]);

            return new VectorValue<TScalar>(value);
        }
        public static VectorValue<TScalar> Negate(VectorValue<TScalar> subtrahend) { return subtrahend.Negate(); }
        public VectorValue<TScalar> Negate()
        {
            TScalar[] value = new TScalar[_dataValue.Length];
            
            for(int i = 0; i < _dataValue.Length; i++)
                value[i] = _dataValue[i].Negate();

            return new VectorValue<TScalar>(value);
        }
        //public static TScalar Norm(VectorValue<TScalar> value) { return value.Norm(); }
        //public TScalar Norm()
        //{
        //    if(Count == 0)
        //        return ???;

        //    ...

        //    return new RealValue(_dataValue.Modulus);
        //}
        public VectorValue<TScalar> Scale(TScalar scalar)
        {
            TScalar[] value = new TScalar[_dataValue.Length];

            for(int i = 0; i < _dataValue.Length; i++)
                value[i] = _dataValue[i].Multiply(scalar);

            return new VectorValue<TScalar>(value);
        }
        #endregion
        #endregion

        #region Constants
        public static Signal Constant(TScalar[] value)
        {
            return Constant(value, value.ToString());
        }
        public static Signal Constant(TScalar[] value, string name)
        {
            Signal s = Binder.CreateSignal(new VectorValue<TScalar>(value));
            s.Label = name + "_Constant";
            s.EnableFlag(StdAspect.ConstantFlag);
            //s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static VectorValue<TScalar> Zero
        {
            get { return new VectorValue<TScalar>(); } //TODO: Think about Singleton like design...
        }

        public static VectorValue<TScalar> AdditiveIdentity
        {
            get { return new VectorValue<TScalar>(); } //TODO: Think about Singleton like design...
        }
        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for(int i=0;i<_dataValue.Length;i++)
            {
                if(i>0)
                    sb.Append(',');
                sb.Append(_dataValue[i].ToString());
            }
            sb.Append(']');
            return sb.ToString();
        }

        public override bool Equals(IValueStructure other)
        {
            VectorValue<TScalar> vectorValue = other as VectorValue<TScalar>;
            if(vectorValue != null)
                return Equals(vectorValue);

            return other == this;
        }
        public bool Equals(VectorValue<TScalar> other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicAdditiveIdentityElement Members
        public bool IsAdditiveIdentity
        {
            get
            {
                for(int i = 0; i < _dataValue.Length; i++)
                    if(!_dataValue[i].IsAdditiveIdentity)
                        return false;
                return true;
            }
        }
        IValueStructure IAlgebraicAdditiveIdentityElement.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        //protected override void InnerSerialize(XmlWriter writer)
        //{
        //    writer.WriteElementString("Count", _dataValue.Length.ToString());
        //    for(int i = 0; i < _dataValue.Length; i++)
        //        ValueStructure.Serialize(writer,_dataValue[i]);
        //}
        //private static VectorValue<TScalar> InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    int cnt = int.Parse(reader.ReadElementString("Count"));
        //    TScalar[] values = new TScalar[cnt];
        //    for(int i = 0; i < cnt; i++)
        //        values[i] = (TScalar)ValueStructure.Deserialize(context, reader);
        //    return new VectorValue<TScalar>(values);
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            Persistence.Serializer.SerializeList<TScalar>(_dataValue, writer, signalMappings, busMappings, "Components");
        }
        private static VectorValue<TScalar> Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            List<TScalar> values = Persistence.Serializer.DeserializeList<TScalar>(reader, signals, buses, "Components");
            return new VectorValue<TScalar>(values);
        }
        #endregion

        
    }
}
