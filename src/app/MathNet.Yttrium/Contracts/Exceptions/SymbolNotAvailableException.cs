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

namespace MathNet.Symbolics.Exceptions
{
    [Serializable]
    public class SymbolNotAvailableException : MathNetSymbolicsException
    {
        private string symbol;

        public SymbolNotAvailableException(string symbol)
        {
            this.symbol = symbol;
        }

        protected SymbolNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            symbol = info.GetString("symbol");
        }

        public string Symbol
        {
            get { return symbol; }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("symbol", symbol, typeof(string));
            base.GetObjectData(info, context);
        }
    }
}
