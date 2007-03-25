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
using System.Reflection;

namespace MathNet.Symbolics.Packages.ObjectModel
{
    // TODO: DECISION: Categories (and Category Membership) vs. Properties

    public abstract class CategoryBase : ICategory, IEquatable<CategoryBase>
    {
        private MathIdentifier _id;

        protected CategoryBase(string label, string domain)
            : this(new MathIdentifier(label, domain)) { }
        protected CategoryBase(MathIdentifier id)
        {
            _id = id;
        }

        public abstract CategoryMembershipLevel IsMember(Signal signal, bool ignoreCache);
        public abstract CategoryMembershipLevel IsMember(Port port);

        protected bool TryGetCachedMembership(Signal signal, out CategoryMembershipLevel membership)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            // TODO: uncomment and fix

            //IProperty p;
            //if(((Signal)signal).Properties.TryLookupProperty(AssociatedPropertyId, out p))
            //{
            //    CategoryMembershipProperty property = p as CategoryMembershipProperty;
            //    if(property != null)
            //    {
            //        membership = property.CategoryMembership;
            //        return true;
            //    }
            //}
            membership = CategoryMembershipLevel.Unknown;
            return false;
        }

        protected CategoryMembershipLevel EvaluateJointChildMembership(IList<Signal> signals)
        {
            if(signals.Count == 0)
                return CategoryMembershipLevel.Unknown;
            CategoryMembershipLevel membership = CategoryMembershipLevel.Member;
            foreach(Signal s in signals)
            {
                CategoryMembershipLevel ret = IsMember(s, false);
                if(ret == CategoryMembershipLevel.NotMember)
                    return CategoryMembershipLevel.NotMember;
                if(ret == CategoryMembershipLevel.Unknown)
                    membership = CategoryMembershipLevel.Unknown;
            }
            return membership;
        }

        public MathIdentifier TypeId
        {
            get { return _id; }
        }
        public MathIdentifier AssociatedPropertyId
        {
            get { return _id; } //propertyId;
        }

        ///// <summary>Adds a category membership property to the signal</summary>
        ///// <param name="sticky">If true the property will stay attached (but updated) when the membership changes. If false the property will be removed. If the new state is unknown, the property is removed in both cases.</param>
        //public CategoryMembershipProperty TagSignal(Signal signal, bool sticky)
        //{
        //    return TagSignal(signal, IsMember(signal, false), sticky);
        //}
        //protected CategoryMembershipProperty TagSignal(Signal signal, CategoryMembershipLevel membership, bool sticky)
        //{
        //    if(signal == null)
        //        throw new ArgumentNullException("signal");

        //    CategoryMembershipProperty property = new CategoryMembershipProperty(this, membership, sticky);
        //    ((Signal)signal).Properties.AddProperty(property);
        //    return property;
        //}

        public virtual bool Equals(CategoryBase other)
        {
            return _id.Equals(other._id);
        }

        //#region Serialization
        //protected abstract void InnerSerialize(XmlWriter writer);
        //public static void Serialize(XmlWriter writer, CategoryBase category)
        //{
        //    if(writer == null)
        //        throw new ArgumentNullException("writer");

        //    writer.WriteStartElement("Category", Context.YttriumNamespace);
        //    writer.WriteAttributeString("type", category.GetType().AssemblyQualifiedName);
        //    category.InnerSerialize(writer);
        //    writer.WriteEndElement();
        //}
        //public static CategoryBase Deserialize(IContext context, XmlReader reader)
        //{
        //    if(reader == null)
        //        throw new ArgumentNullException("reader");

        //    reader.ReadStartElement("Category", Context.YttriumNamespace);
        //    Type type = Type.GetType(reader.GetAttribute("type"), true);
        //    MethodInfo mi = type.GetMethod("InnerDeserialize", BindingFlags.Static);
        //    Category ret = (Category)mi.Invoke(null, new object[] { context, reader });
        //    reader.ReadEndElement();
        //    return ret;
        //}
        //#endregion

        // SEE MICROKERNEL SERIALIZER
        public virtual void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
        }
        //private static Property Deserialize(IContext context, XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses);

        public bool EqualsById(ICategory other)
        {
            return _id.Equals(other.TypeId);
        }

        public bool EqualsById(MathIdentifier otherCategoryId)
        {
            return _id.Equals(otherCategoryId);
        }

        public virtual bool ReferencesCoreObjects
        {
            get { return false; }
        }

        public virtual IEnumerable<Signal> CollectSignals()
        {
            yield break;
        }

        public virtual IEnumerable<Bus> CollectBuses()
        {
            yield break;
        }

        public bool Equals(ICategory other)
        {
            CategoryBase cb = other as CategoryBase;
            if(cb == null)
                return false;
            else
                return Equals(cb);
        }
    }
}
