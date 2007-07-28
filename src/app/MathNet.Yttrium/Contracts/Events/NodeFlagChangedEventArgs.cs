using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Events
{
    public class NodeFlagChangedEventArgs : EventArgs
    {
        private readonly FlagState _oldState;
        private readonly FlagState _newState;
        private readonly Node _node;
        private readonly NodeFlag _flag;

        public NodeFlagChangedEventArgs(Node node, NodeFlag flag, FlagState oldState, FlagState newState)
        {
            _node = node;
            _flag = flag;
            _oldState = oldState;
            _newState = newState;
        }

        public Node Node
        {
            get { return _node; }
        }

        public NodeFlag Flag
        {
            get { return _flag; }
        }

        public object OldState
        {
            get { return _oldState; }
        }

        public object NewState
        {
            get { return _newState; }
        }
    }
}
