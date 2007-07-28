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

namespace MathNet.Symbolics
{
    public class Identifier<TFriendlyIdentifier>
        : IEquatable<Identifier<TFriendlyIdentifier>>,
          IComparable<Identifier<TFriendlyIdentifier>>
        where TFriendlyIdentifier
        : IEquatable<TFriendlyIdentifier>,
          IComparable<TFriendlyIdentifier>
    {
        private TFriendlyIdentifier _friendly;
        private Guid _id;

        public Identifier(TFriendlyIdentifier friendly, IdentifierService<TFriendlyIdentifier> owner)
        {
            _friendly = friendly;
            _id = Guid.NewGuid();
            if(owner != null)
                owner.RegisterIdentifier(this); // should fail if friendly already exists -> construction fails
        }

        public TFriendlyIdentifier Friendly
        {
            get { return _friendly; }
        }

        public int CompareTo(Identifier<TFriendlyIdentifier> other)
        {
            return _id.CompareTo(other._id);
        }
        public bool Equals(Identifier<TFriendlyIdentifier> other)
        {
            if(other == null)
                return false;
            return _id.Equals(other._id);
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Identifier<TFriendlyIdentifier> id = obj as Identifier<TFriendlyIdentifier>;
            if(id == null)
                return false;
            return Equals(id);
        }
        public static bool operator ==(Identifier<TFriendlyIdentifier> left, Identifier<TFriendlyIdentifier> right)
        {
            if(object.ReferenceEquals(left, right))
                return true;
            if((object)left == null || (object)right == null)
                return false;
            return left.Equals(right);
        }
        public static bool operator !=(Identifier<TFriendlyIdentifier> left, Identifier<TFriendlyIdentifier> right)
        {
            return !(left == right);
        }
    }
}
