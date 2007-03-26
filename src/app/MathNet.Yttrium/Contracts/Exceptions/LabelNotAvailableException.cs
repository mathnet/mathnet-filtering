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
    public class LabelNotAvailableException : YttriumException
    {
        private string domain, label;

        public LabelNotAvailableException()
            : base()
        {
        }

        public LabelNotAvailableException(string message)
            : base(message)
        {
        }

        public LabelNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public LabelNotAvailableException(string label, string domain)
            : base()
        {
            this.domain = domain;
            this.label = label;
        }

        protected LabelNotAvailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            domain = info.GetString("domain");
            label = info.GetString("label");
        }

        public string Domain
        {
            get { return domain; }
        }

        public string Label
        {
            get { return label; }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("domain", domain, typeof(string));
            info.AddValue("label", label, typeof(string));
            base.GetObjectData(info, context);
        }
    }
}
