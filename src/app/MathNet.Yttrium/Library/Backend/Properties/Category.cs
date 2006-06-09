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
using System.Xml;
using System.Reflection;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Properties
{
    // TODO: DECISION: Categories (and Category Membership) vs. Properties

    public abstract class Category : IEquatable<Category>
    {
        private MathIdentifier id;
        //private MathIdentifier propertyId;

        protected Category(string label, string domain)
            : this(new MathIdentifier(label, domain)) { }
        protected Category(MathIdentifier id)
        {
            this.id = id;
            //this.propertyId = id.DerivePostfix("Category");
        }

        public abstract ECategoryMembership IsMember(Signal signal, bool ignoreCache);
        public abstract ECategoryMembership IsMember(Port port);

        protected bool TryGetCachedMembership(Signal signal, out ECategoryMembership membership)
        {
            Property p;
            if(signal.Properties.TryLookupProperty(AssociatedPropertyIdentifier, out p))
            {
                CategoryMembershipProperty property = p as CategoryMembershipProperty;
                if(property != null)
                {
                    membership = property.CategoryMembership;
                    return true;
                }
            }
            membership = ECategoryMembership.Unknown;
            return false;
        }

        protected ECategoryMembership EvaluateJointChildMembership(IList<Signal> signals)
        {
            ECategoryMembership membership = ECategoryMembership.Member;
            foreach(Signal s in signals)
            {
                ECategoryMembership ret = IsMember(s, false);
                if(ret == ECategoryMembership.NotMember)
                    return ECategoryMembership.NotMember;
                if(ret == ECategoryMembership.Unknown)
                    membership = ECategoryMembership.Unknown;
            }
            return membership;
        }

        public MathIdentifier Identifier
        {
            get { return id; }
        }
        public MathIdentifier AssociatedPropertyIdentifier
        {
            get { return id; } //propertyId;
        }

        /// <summary>Adds a category membership property to the signal</summary>
        /// <param name="sticky">If true the property will stay attached (but updated) when the membership changes. If false the property will be removed. If the new state is unknown, the property is removed in both cases.</param>
        public CategoryMembershipProperty TagSignal(Signal signal, bool sticky)
        {
            return TagSignal(signal, IsMember(signal, false), sticky);
        }
        protected CategoryMembershipProperty TagSignal(Signal signal, ECategoryMembership membership, bool sticky)
        {
            CategoryMembershipProperty property = new CategoryMembershipProperty(this, membership, sticky);
            signal.Properties.AddProperty(property);
            return property;
        }

        public virtual bool Equals(Category other)
        {
            return id.Equals(other.id);
        }

        #region Serialization
        protected abstract void InnerSerialize(XmlWriter writer);
        public static void Serialize(XmlWriter writer, Category category)
        {
            writer.WriteStartElement("Category", Context.YttriumNamespace);
            writer.WriteAttributeString("type", category.GetType().AssemblyQualifiedName);
            category.InnerSerialize(writer);
            writer.WriteEndElement();
        }
        public static Category Deserialize(Context context, XmlReader reader)
        {
            reader.ReadStartElement("Category", Context.YttriumNamespace);
            Type type = Type.GetType(reader.GetAttribute("type"), true);
            MethodInfo mi = type.GetMethod("InnerDeserialize", BindingFlags.Static);
            Category ret = (Category)mi.Invoke(null, new object[] { context, reader });
            reader.ReadEndElement();
            return ret;
        }
        #endregion

        
    }
}
