using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Events
{
    public class NodePropertyDirtiedEventArgs : EventArgs
    {
        private readonly Node _node;
        private readonly NodeProperty _property;

        public NodePropertyDirtiedEventArgs(Node node, NodeProperty property)
        {
            _node = node;
            _property = property;
        }

        public Node Node
        {
            get { return _node; }
        }

        public NodeProperty Property
        {
            get { return _property; }
        }
    }
}
