using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Events
{
    public class NodePropertyChangedEventArgs : EventArgs
    {
        private readonly object _oldValue;
        private readonly object _newValue;
        private readonly Node _node;
        private readonly NodeProperty _property;

        public NodePropertyChangedEventArgs(Node node, NodeProperty property, object oldValue, object newValue)
        {
            _node = node;
            _property = property;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public Node Node
        {
            get { return _node; }
        }

        public NodeProperty Property
        {
            get { return _property; }
        }

        public object OldValue
        {
            get { return _oldValue; }
        }

        public object NewValue
        {
            get { return _newValue; }
        }
    }
}
