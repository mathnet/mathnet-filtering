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
using System.Reflection;
using System.Collections.Generic;
using MathNet.Symbolics.Backend.Persistence;

namespace MathNet.Symbolics.Packages.ObjectModel
{
    // TODO: DECISION: Categories (and Category Membership) vs. Properties

    public class CategoryMembershipProperty : PropertyBase
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("CategoryMembership", "Std");
        private readonly bool _isSticky;
        private readonly ICategory _category;
        private CategoryMembershipLevel _categoryMembership;

        /// <param name="sticky">If true the property will stay attached (but updated) when the membership changes. If false the property will be removed. If the new state is unknown, the property is removed in both cases.</param>
        public CategoryMembershipProperty(ICategory category, CategoryMembershipLevel categoryMembership, bool sticky)
        {
            _category = category;
            _categoryMembership = categoryMembership;
            _isSticky = sticky;
        }

        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }

        public bool IsSticky
        {
            get { return _isSticky; }
        }

        public ICategory Category
        {
            get { return _category; }
        }

        public CategoryMembershipLevel CategoryMembership
        {
            get { return _categoryMembership; }
        }

        #region Property Member
        public override bool StillValidAfterEvent(Signal signal)
        {
            CategoryMembershipLevel newMembership = _category.IsMember(signal, true);
            if(newMembership == CategoryMembershipLevel.Unknown)
            {
                _categoryMembership = newMembership;
                return false;
            }
            if(_categoryMembership != newMembership)
            {
                _categoryMembership = newMembership;
                if(_isSticky)
                    return true;
                else
                    return false;
            }
            return true;
        }

        public override bool StillValidAfterDrive(Signal signal)
        {
            CategoryMembershipLevel newMembership = _category.IsMember(signal, true);
            if(newMembership == CategoryMembershipLevel.Unknown)
            {
                _categoryMembership = newMembership;
                return false;
            }
            if(_categoryMembership != newMembership)
            {
                _categoryMembership = newMembership;
                if(_isSticky)
                    return true;
                else
                    return false;
            }
            return true;
        }

        public override bool StillValidAfterUndrive(Signal signal)
        {
            return true;
        }

        #endregion

        public override bool Equals(IProperty other)
        {
            CategoryMembershipProperty cmp = other as CategoryMembershipProperty;
            if(cmp == null)
                return false;
            return _isSticky == cmp._isSticky && _category.Equals(cmp._category)
                && _categoryMembership.Equals(cmp._categoryMembership);
        }

        #region Serialization
        //protected override void InnerSerialize(XmlWriter writer)
        //{
        //    writer.WriteElementString("Sticky", _isSticky.ToString());
        //    writer.WriteElementString("Membership", _categoryMembership.ToString());
        //    Category.Serialize(writer, _category);
        //}
        //private static CategoryMembershipProperty InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    bool sticky = bool.Parse(reader.ReadElementString("Sticky"));
        //    ECategoryMembership membership = (ECategoryMembership)Enum.Parse(typeof(ECategoryMembership), reader.ReadElementString("Membership"));
        //    Category category = Category.Deserialize(context, reader);
        //    return new CategoryMembershipProperty(category, membership, sticky);
        //}

        // SEE MICROKERNEL SERIALIZER
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteElementString("Sticky", _isSticky.ToString());
            writer.WriteElementString("Membership", _categoryMembership.ToString());
            Serializer.Serialize(_category, writer, signalMappings, busMappings);
            //Category.Serialize(writer, _category);
        }
        private static CategoryMembershipProperty Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            bool sticky = bool.Parse(reader.ReadElementString("Sticky"));
            CategoryMembershipLevel membership = (CategoryMembershipLevel)Enum.Parse(typeof(CategoryMembershipLevel), reader.ReadElementString("Membership"));
            ICategory category = Serializer.Deserialize<ICategory>(reader, signals, buses);
            //Category.Deserialize(context, reader);
            return new CategoryMembershipProperty(category, membership, sticky);
        }
        #endregion
    }
}
