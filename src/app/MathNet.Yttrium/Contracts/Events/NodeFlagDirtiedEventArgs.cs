using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Events
{
    public class NodeFlagDirtiedEventArgs : EventArgs
    {
        private readonly Node _node;
        private readonly NodeFlag _flag;

        public NodeFlagDirtiedEventArgs(Node node, NodeFlag flag)
        {
            _node = node;
            _flag = flag;
        }

        public Node Node
        {
            get { return _node; }
        }

        public NodeFlag Flag
        {
            get { return _flag; }
        }
    }
}
