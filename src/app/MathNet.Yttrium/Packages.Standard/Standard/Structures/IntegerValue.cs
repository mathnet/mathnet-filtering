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
    /// <summary>signed integer</summary>
    public class IntegerValue : ValueStructureBase, IEquatable<IntegerValue>, IComparable<IntegerValue>, IAlgebraicCommutativeRingWithUnity<IntegerValue>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Integer", "Std");
        private readonly long _dataValue; // = 0;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<IntegerValue>();
            library.AddArbitraryType(typeof(long));
            ValueConverter<IntegerValue>.AddConverterFrom(ValueConverter<long>.Router, true,
                delegate(object value) { return new IntegerValue((long)value); });
            ValueConverter<IntegerValue>.AddConverterTo(ValueConverter<long>.Router, true,
                delegate(object value) { return ((IntegerValue)value).Value; });

            //library.AddCustomDataType(new CustomDataRef(typeof(IntegerValue), ValueConverter<IntegerValue>.Router));
        }
        public static IntegerValue ConvertFrom(ICustomData value)
        {
            return (IntegerValue)ValueConverter<IntegerValue>.ConvertFrom(value);
        }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<IntegerValue>.Router.CanConvertLosslessFrom(value);
        }
        #endregion

        #region Constructors
        public IntegerValue()
        {
            //this.dataValue = 0;
        }
        public IntegerValue(long value)
        {
            _dataValue = value;
        }
        #endregion

        #region Basic Operation Processes
        internal class AddProcess : GenericFunctionProcess
        {
            public AddProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public AddProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(IntegerValue.AdditiveIdentity);
                else
                {
                    IntegerValue sum = (IntegerValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        sum = sum.Add(integer);
                    }
                    PublishToOutputs(sum);
                }
            }
        }
        internal class SubtractProcess : GenericFunctionProcess
        {
            public SubtractProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public SubtractProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(IntegerValue.AdditiveIdentity);
                else
                {
                    IntegerValue sum = (IntegerValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        sum = sum.Subtract(integer);
                    }
                    PublishToOutputs(sum);
                }
            }
        }
        internal class MultiplyProcess : GenericFunctionProcess
        {
            public MultiplyProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public MultiplyProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(IntegerValue.MultiplicativeIdentity);
                else
                {
                    IntegerValue product = (IntegerValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        product = product.Multiply(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        internal class DivideProcess : GenericFunctionProcess
        {
            public DivideProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public DivideProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(RationalValue.MultiplicativeIdentity);
                else
                {
                    RationalValue product = new RationalValue((IntegerValue)Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        product = product.Divide(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        #endregion

        public long Value
        {
            get { return _dataValue; }
        }

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        #region Direct Operator Implementations
        public static bool operator <(IntegerValue left, IntegerValue right)
        {
            return left._dataValue < right._dataValue;
        }
        public static bool operator >(IntegerValue left, IntegerValue right)
        {
            return left._dataValue > right._dataValue;
        }
        #endregion

        #region Direct Function Implementations
        #region Arithmetic Operations
        public IntegerValue Add(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new IntegerValue(_dataValue + op._dataValue);
        }
        public IntegerValue Subtract(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new IntegerValue(_dataValue - op._dataValue);
        }
        public IntegerValue Negate()
        {
            return new IntegerValue(-_dataValue);
        }
        public IntegerValue Multiply(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new IntegerValue(_dataValue * op._dataValue);
        }
        public RationalValue Divide(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(_dataValue, op._dataValue);
        }
        public RationalValue Invert()
        {
            return new RationalValue(1, _dataValue);
        }
        public IntegerValue Power(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");
            
            return new IntegerValue((long)Math.Round(Math.Pow(_dataValue, op._dataValue)));
        }
        public IntegerValue Absolute()
        {
            return (_dataValue < 0) ? new IntegerValue(-_dataValue) : this;
        }
        public IntegerValue Max(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return (_dataValue >= op._dataValue) ? this : op;
        }
        public IntegerValue Min(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return (_dataValue <= op._dataValue) ? this : op;
        }
        #endregion
        #endregion

        #region Constants
        public static Signal ParseConstant(string value)
        {
            return Constant(long.Parse(value, Config.InternalNumberFormat));
        }
        public static Signal Constant(long value)
        {
            Signal s = Binder.CreateSignal(new IntegerValue(value));
            s.Label = s.Value.ToString() + "_Constant";
            s.EnableFlag(StdAspect.ConstantFlag);
            //s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantZero
        {
            get
            {
                MathIdentifier id = new MathIdentifier("IntegerValueConstantZero", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(0);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public static Signal ConstantOne
        {
            get
            {
                MathIdentifier id = new MathIdentifier("IntegerValueConstantOne", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(1);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public static Signal ConstantTwo
        {
            get
            {
                MathIdentifier id = new MathIdentifier("IntegerValueConstantTwo", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(2);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public static Signal ConstantMinusOne
        {
            get
            {
                MathIdentifier id = new MathIdentifier("IntegerValueConstantMinusOne", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(-1);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public static Signal ConstantAdditiveIdentity
        {
            get { return ConstantZero; }
        }

        public static Signal ConstantMultiplicativeIdentity
        {
            get { return ConstantOne; }
        }

        public static IntegerValue Zero
        {
            get { return new IntegerValue(0); } //TODO: Think about Singleton like design...
        }

        public static IntegerValue One
        {
            get { return new IntegerValue(1); } //TODO: Think about Singleton like design...
        }

        public static IntegerValue AdditiveIdentity
        {
            get { return new IntegerValue(0); } //TODO: Think about Singleton like design...
        }

        public static IntegerValue MultiplicativeIdentity
        {
            get { return new IntegerValue(1); } //TODO: Think about Singleton like design...
        }
        #endregion

        public double ToDouble()
        {
            return (double)_dataValue;
        }

        public override string ToString()
        {
            return base.ToString() + "(" + _dataValue.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ")";
        }

        public override bool Equals(IValueStructure other)
        {
            IntegerValue integerValue = other as IntegerValue;
            if(integerValue != null)
                return Equals(integerValue);

            RationalValue rationalValue = other as RationalValue;
            if(rationalValue != null)
                return rationalValue.Equals(this);

            RealValue realValue = other as RealValue;
            if(realValue != null)
                return realValue.Equals(this);

            return other == this;
        }
        public bool Equals(IntegerValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public int CompareTo(IntegerValue other)
        {
            return _dataValue.CompareTo(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicMultiplicativeIdentityElement Members
        public bool IsMultiplicativeIdentity
        {
            get { return _dataValue == 1; }
        }
        IValueStructure IAlgebraicMultiplicativeIdentityElement.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicAdditiveIdentityElement Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue == 0; }
        }
        IValueStructure IAlgebraicAdditiveIdentityElement.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteString(_dataValue.ToString(Config.InternalNumberFormat));
        //}
        //private static IntegerValue InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return new IntegerValue(long.Parse(reader.ReadString(), Config.InternalNumberFormat));
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteString(_dataValue.ToString(Config.InternalNumberFormat));
        }
        private static IntegerValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return new IntegerValue(long.Parse(reader.ReadString(), Config.InternalNumberFormat));
        }
        #endregion
    }
}
