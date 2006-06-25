using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Patterns
{
    public class GroupCollection : KeyedCollection<string, Group>
    {
        public GroupCollection()
            : base()
        {
        }

        protected override string GetKeyForItem(Group group)
        {
            return group.Label;
        }

        public bool TryGetValue(string label, out Group group)
        {
            if(Dictionary == null)
            {
                group = null;
                return false;
            }
            return Dictionary.TryGetValue(label, out group);
        }

        public void Append(string label, Tuple<Signal, Port> value)
        {
            Group group;
            if(!TryGetValue(label, out group))
            {
                group = new Group(label);
                Add(group);
            }
            group.Add(value);
        }
        public void Append(string label, IEnumerable<Tuple<Signal, Port>> values)
        {
            Group group;
            if(!TryGetValue(label, out group))
            {
                group = new Group(label);
                Add(group);
            }
            group.AddRange(values);
        }
        public void Append(Group group)
        {
            Group localGroup;
            if(!TryGetValue(group.Label, out localGroup))
                Add(group);
            else
                localGroup.AddRange(group);
        }
    }
}
