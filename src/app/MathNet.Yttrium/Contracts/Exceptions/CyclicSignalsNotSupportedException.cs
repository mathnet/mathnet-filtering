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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Globalization;

namespace MathNet.Symbolics.Exceptions
{
    [Serializable]
    public class CyclicSignalsNotSupportedException : YttriumException
    {
        Signal _signal;

        public CyclicSignalsNotSupportedException()
            : base()
        {
        }

        public CyclicSignalsNotSupportedException(string message)
            : base(message)
        {
        }

        public CyclicSignalsNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


        public CyclicSignalsNotSupportedException(Signal signal, string operation)
            : base(string.Format(CultureInfo.CurrentCulture, MathNet.Symbolics.Properties.Resources.ex_CyclicSignalsNotSupportes, operation))
        {
            _signal = signal;
        }

        protected CyclicSignalsNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _signal = (Signal)info.GetValue("signal", typeof(Signal));
        }

        public Signal Signal
        {
            get { return _signal; }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("signal", _signal, typeof(Signal));
            base.GetObjectData(info, context);
        }
    }
}
