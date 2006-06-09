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

namespace MathNet.Symbolics.Core
{
    [Serializable]
    public struct MathIdentifier : IEquatable<MathIdentifier>, IComparable<MathIdentifier>
    {
        private readonly string domain, label;

        public MathIdentifier(string label, string domain)
        {
            this.domain = domain;
            this.label = label;
        }

        public string Label
        {
            get { return label; }
        }

        public string Domain
        {
            get { return domain; }
        }

        public MathIdentifier DerivePostfix(string labelPostfix)
        {
            return new MathIdentifier(label + labelPostfix, domain);
        }
        public MathIdentifier DerivePrefix(string labelPrefix)
        {
            return new MathIdentifier(labelPrefix + label, domain);
        }

        public bool Equals(string label, string domain)
        {
            return this.domain == domain && this.label == label;
        }
        public bool Equals(MathIdentifier other)
        {
            return domain == other.domain && label == other.label;
        }
        public override bool Equals(object obj)
        {
            if(obj != null && obj is MathIdentifier)
                return Equals((MathIdentifier)obj);
            else
                return false;
        }
        public static bool operator ==(MathIdentifier left, MathIdentifier right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(MathIdentifier left, MathIdentifier right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return domain.GetHashCode() ^ label.GetHashCode();
        }

        public int CompareTo(MathIdentifier other)
        {
            int c = domain.CompareTo(other.domain);
            if(c != 0)
                return c;
            else
                return label.CompareTo(other.label);
        }

        public override string ToString()
        {
            return domain + "." + label;
        }

        public static MathIdentifier Parse(string value)
        {
            if(value == null) throw new ArgumentNullException("value");
            string[] parts = value.Split('.');
            return new MathIdentifier(parts[1], parts[0]);
        }
    }
}