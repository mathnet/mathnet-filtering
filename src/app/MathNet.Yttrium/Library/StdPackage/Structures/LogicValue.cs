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
using System.Xml;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.StdPackage.Structures
{
    public enum ELogicX01
    {
        Unknown,
        True,
        False
    }

    public enum ELogicIeee1164
    {
        Uninitialized,
        ForcingUnknown,
        ForcingZero,
        ForcingOne,
        HighImpedance,
        WeakUnknown,
        WeakZero,
        WeakOne,
        DoNotCare
    }

    /// <summary>logic value - X01 subset of IEEE 1164 standard (aka MVL-9)</summary>
    public class LogicValue : ValueStructure, IEquatable<LogicValue>, IComparable<LogicValue>, IAlgebraicCommutativeRingWithUnity<LogicValue>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Logic", "Std");
        private readonly ELogicX01 _dataValue; // = ELogicX01.Uninitialized;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(LogicValue));
            _router.AddSourceNeighbor(IntegerValue.Converter, false, LocalConvertFrom);
        }
        private static ValueStructure LocalConvertFrom(ValueStructure value)
        {
            IntegerValue inv = value as IntegerValue;
            if(inv != null) return new LogicValue(inv.Value == 0 ? ELogicX01.False : ELogicX01.True);
            return (LogicValue)value;
        }
        public static LogicValue ConvertFrom(ValueStructure value)
        {
            return (LogicValue)_router.ConvertFrom(value);
        }
        #endregion

        #region Constructors
        protected LogicValue()
        {
            _dataValue = ELogicX01.Unknown;
        }
        protected LogicValue(ELogicX01 value)
        {
            _dataValue = value;
        }
        #endregion

        #region Basic Operation Processes
        // ### AND ###
        public static Process CreateAndProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.True; },
                delegate(LogicValue accu, LogicValue item) { return accu.And(item); },
                inInput, inInternal, outOutput, outInternal);
        }
        public static Process CreateAndProcess(int firstInput, int lastInput, int output)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.True; },
                delegate(LogicValue accu, LogicValue item) { return accu.And(item); },
                firstInput, lastInput, output);
        }
        // ### NAND ###
        public static Process CreateNandProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.False; },
                delegate(LogicValue accu, LogicValue item) { return accu.Nand(item); },
                inInput, inInternal, outOutput, outInternal);
        }
        public static Process CreateNandProcess(int firstInput, int lastInput, int output)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.False; },
                delegate(LogicValue accu, LogicValue item) { return accu.Nand(item); },
                firstInput, lastInput, output);
        }
        // ### OR ###
        public static Process CreateOrProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.False; },
                delegate(LogicValue accu, LogicValue item) { return accu.Or(item); },
                inInput, inInternal, outOutput, outInternal);
        }
        public static Process CreateOrProcess(int firstInput, int lastInput, int output)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.False; },
                delegate(LogicValue accu, LogicValue item) { return accu.Or(item); },
                firstInput, lastInput, output);
        }
        // ### NOR ###
        public static Process CreateNorProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.True; },
                delegate(LogicValue accu, LogicValue item) { return accu.Nor(item); },
                inInput, inInternal, outOutput, outInternal);
        }
        public static Process CreateNorProcess(int firstInput, int lastInput, int output)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.True; },
                delegate(LogicValue accu, LogicValue item) { return accu.Nor(item); },
                firstInput, lastInput, output);
        }
        // ### XOR ###
        public static Process CreateXorProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.Unknown; },
                delegate(LogicValue accu, LogicValue item) { return accu.Xor(item); },
                inInput, inInternal, outOutput, outInternal);
        }
        public static Process CreateXorProcess(int firstInput, int lastInput, int output)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.Unknown; },
                delegate(LogicValue accu, LogicValue item) { return accu.Xor(item); },
                firstInput, lastInput, output);
        }
        // ### XNOR ###
        public static Process CreateXnorProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.Unknown; },
                delegate(LogicValue accu, LogicValue item) { return accu.Xnor(item); },
                inInput, inInternal, outOutput, outInternal);
        }
        public static Process CreateXnorProcess(int firstInput, int lastInput, int output)
        {
            return new GenericStdFunctionProcess<LogicValue>(
                delegate() { return LogicValue.Unknown; },
                delegate(LogicValue accu, LogicValue item) { return accu.Xnor(item); },
                firstInput, lastInput, output);
        }
        // ### NOT ###
        public static Process CreateNotProcess(int firstInput, int firstOutput, int count, bool inputIsInternal, bool outputIsInternal)
        {
            return new GenericStdParallelProcess<LogicValue, LogicValue>(
                delegate(LogicValue item) { return item.Not(); },
                firstInput, firstOutput, count, inputIsInternal, outputIsInternal);
        }

        #region OLD Implementations (commented out)
        /*
        public class AndProcess : GenericFunctionProcess
        {
            public AndProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public AndProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action()
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(LogicValue.True);
                else
                {
                    LogicValue lv = (LogicValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                        lv = lv.And((LogicValue)Inputs[i].Value);
                    PublishToOutputs(lv);
                }
            }
        }
        public class NandProcess : GenericFunctionProcess
        {
            public NandProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public NandProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action()
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(LogicValue.False);
                else
                {
                    LogicValue lv = (LogicValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                        lv = lv.Nand((LogicValue)Inputs[i].Value);
                    PublishToOutputs(lv);
                }
            }
        }
        public class OrProcess : GenericFunctionProcess
        {
            public OrProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public OrProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action()
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(LogicValue.False);
                else
                {
                    LogicValue lv = (LogicValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                        lv = lv.Or((LogicValue)Inputs[i].Value);
                    PublishToOutputs(lv);
                }
            }
        }
        public class NorProcess : GenericFunctionProcess
        {
            public NorProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public NorProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action()
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(LogicValue.True);
                else
                {
                    LogicValue lv = (LogicValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                        lv = lv.Nor((LogicValue)Inputs[i].Value);
                    PublishToOutputs(lv);
                }
            }
        }
        public class XorProcess : GenericFunctionProcess
        {
            public XorProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public XorProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action()
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(LogicValue.Unknown);
                else
                {
                    LogicValue lv = (LogicValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                        lv = lv.Xor((LogicValue)Inputs[i].Value);
                    PublishToOutputs(lv);
                }
            }
        }
        public class XnorProcess : GenericFunctionProcess
        {
            public XnorProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public XnorProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action()
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(LogicValue.Unknown);
                else
                {
                    LogicValue lv = (LogicValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                        lv = lv.Xnor((LogicValue)Inputs[i].Value);
                    PublishToOutputs(lv);
                }
            }
        }
        */
        #endregion
        #endregion

        public ELogicX01 Value
        {
            get { return _dataValue; }
        }

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
        }

        #region Logic Operations
        public LogicValue And(LogicValue op)
        {
            if(_dataValue == ELogicX01.False || op._dataValue == ELogicX01.False)
                return LogicValue.False;
            if(_dataValue == ELogicX01.Unknown || op._dataValue == ELogicX01.Unknown)
                return LogicValue.Unknown;
            else
                return LogicValue.True;
        }
        public LogicValue Nand(LogicValue op)
        {
            if(_dataValue == ELogicX01.False || op._dataValue == ELogicX01.False)
                return LogicValue.True;
            if(_dataValue == ELogicX01.Unknown || op._dataValue == ELogicX01.Unknown)
                return LogicValue.Unknown;
            else
                return LogicValue.False;
        }
        public LogicValue Or(LogicValue op)
        {
            if(_dataValue == ELogicX01.True || op._dataValue == ELogicX01.True)
                return LogicValue.True;
            if(_dataValue == ELogicX01.Unknown || op._dataValue == ELogicX01.Unknown)
                return LogicValue.Unknown;
            else
                return LogicValue.False;
        }
        public LogicValue Nor(LogicValue op)
        {
            if(_dataValue == ELogicX01.True || op._dataValue == ELogicX01.True)
                return LogicValue.False;
            if(_dataValue == ELogicX01.Unknown || op._dataValue == ELogicX01.Unknown)
                return LogicValue.Unknown;
            else
                return LogicValue.True;
        }
        public LogicValue Xor(LogicValue op)
        {
            if(_dataValue == ELogicX01.Unknown || op._dataValue == ELogicX01.Unknown)
                return LogicValue.Unknown;
            if(_dataValue == op._dataValue)
                return LogicValue.False;
            else
                return LogicValue.True;
        }
        public LogicValue Xnor(LogicValue op)
        {
            if(_dataValue == ELogicX01.Unknown || op._dataValue == ELogicX01.Unknown)
                return LogicValue.Unknown;
            if(_dataValue == op._dataValue)
                return LogicValue.True;
            else
                return LogicValue.False;
        }
        public LogicValue Not()
        {
            if(_dataValue == ELogicX01.True)
                return LogicValue.False;
            if(_dataValue == ELogicX01.False)
                return LogicValue.True;
            else
                return LogicValue.Unknown;
        }
        #endregion

        #region Arithmetic Operations
        public LogicValue Add(LogicValue op)
        {
            return Or(op);
        }
        public LogicValue Multiply(LogicValue op)
        {
            return And(op);
        }
        #endregion

        #region Static Templates
        private static readonly LogicValue logicTrue = new LogicValue(ELogicX01.True);
        public static LogicValue True
        {
            get { return logicTrue; }
        }
        public static LogicValue One
        {
            get { return logicTrue; }
        }
        public static LogicValue MultiplicativeIdentity
        {
            get { return logicTrue; }
        }
        private static readonly LogicValue logicFalse = new LogicValue(ELogicX01.False);
        public static LogicValue False
        {
            get { return logicFalse; }
        }
        public static LogicValue Zero
        {
            get { return logicFalse; }
        }
        public static LogicValue AdditiveIdentity
        {
            get { return logicFalse; }
        }
        private static readonly LogicValue logicUnknown = new LogicValue(ELogicX01.Unknown);
        public static LogicValue Unknown
        {
            get { return logicUnknown; }
        }
        #endregion

        public override string ToString()
        {
            switch(_dataValue)
            {
                case ELogicX01.True:
                    return base.ToString() + "(1)";
                case ELogicX01.False:
                    return base.ToString() + "(0)";
                default:
                    return base.ToString() + "(X)";
            }
        }

        public override bool Equals(ValueStructure other)
        {
            LogicValue logicValue = other as LogicValue;
            if(logicValue != null)
                return Equals(logicValue);

            return other == this;
        }
        public bool Equals(LogicValue other)
        {
            return other != null && _dataValue == other._dataValue;
        }
        public int CompareTo(LogicValue other)
        {
            return _dataValue.CompareTo(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicRingWithUnity Members
        public bool IsMultiplicativeIdentity
        {
            get { return _dataValue == ELogicX01.True ; }
        }
        ValueStructure IAlgebraicRingWithUnity.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue == ELogicX01.False; }
        }
        ValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_dataValue.ToString());
        }
        private static LogicValue InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return new LogicValue((ELogicX01)Enum.Parse(typeof(ELogicX01), reader.ReadString()));
        }
        #endregion
    }
}
