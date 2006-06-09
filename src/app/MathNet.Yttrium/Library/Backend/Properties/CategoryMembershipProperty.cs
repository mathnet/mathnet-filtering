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
using System.Reflection;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Properties
{
    // TODO: DECISION: Categories (and Category Membership) vs. Properties

    public class CategoryMembershipProperty : Property
    {
        private readonly MathIdentifier _propertyId;
        private readonly bool _isSticky;
        private readonly Category _category;
        private ECategoryMembership _categoryMembership;

        /// <param name="sticky">If true the property will stay attached (but updated) when the membership changes. If false the property will be removed. If the new state is unknown, the property is removed in both cases.</param>
        public CategoryMembershipProperty(Category category, ECategoryMembership categoryMembership, bool sticky)
        {
            _category = category;
            _categoryMembership = categoryMembership;
            _isSticky = sticky;
            _propertyId = category.AssociatedPropertyIdentifier;
        }

        public override MathIdentifier PropertyId
        {
            get { return _propertyId; }
        }

        public bool IsSticky
        {
            get { return _isSticky; }
        }

        public Category Category
        {
            get { return _category; }
        }

        public ECategoryMembership CategoryMembership
        {
            get { return _categoryMembership; }
        }

        #region Property Member
        public override bool StillValidAfterEvent(Signal signal)
        {
            ECategoryMembership newMembership = _category.IsMember(signal, true);
            if(newMembership == ECategoryMembership.Unknown)
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
            ECategoryMembership newMembership = _category.IsMember(signal, true);
            if(newMembership == ECategoryMembership.Unknown)
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

        public override bool Equals(Property other)
        {
            CategoryMembershipProperty cmp = other as CategoryMembershipProperty;
            if(cmp == null)
                return false;
            return _propertyId.Equals(cmp._propertyId) && _isSticky == cmp._isSticky
                && _category.Equals(cmp._category) && _categoryMembership.Equals(cmp._categoryMembership);
        }

        #region Serialization
        protected override void InnerSerialize(XmlWriter writer)
        {
            writer.WriteElementString("Sticky", _isSticky.ToString());
            writer.WriteElementString("Membership", _categoryMembership.ToString());
            Category.Serialize(writer, _category);
        }
        private static CategoryMembershipProperty InnerDeserialize(Context context, XmlReader reader)
        {
            bool sticky = bool.Parse(reader.ReadElementString("Sticky"));
            ECategoryMembership membership = (ECategoryMembership)Enum.Parse(typeof(ECategoryMembership), reader.ReadElementString("Membership"));
            Category category = Category.Deserialize(context, reader);
            return new CategoryMembershipProperty(category, membership, sticky);
        }
        #endregion
    }
}
