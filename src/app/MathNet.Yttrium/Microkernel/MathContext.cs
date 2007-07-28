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
using System.Threading;

using MathNet.Symbolics.Properties;

namespace MathNet.Symbolics
{
    // TODO: Consider a better design that would allow customization (by inheritance).

    /// <summary>
    /// Provides a context stack for each thread. You may use this to track an execution path
    /// e.g. for a mediator/obeserver pattern by means of the provided <c>InstanceId</c>.
    /// </summary>
    /// <example>
    /// To inject a new context into the current execution path use the following pattern:
    /// <code>
    /// using(MathContext ctx = MathContext.Create())
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </example>
    public class MathContext : IDisposable
    {
        [ThreadStatic]
        private static Stack<MathContext> _perThreadStack;

        private static MathContext _root = new MathContext(null, null);

        public static event EventHandler<Events.MathContextEventArgs> ContextExpired;

        public static MathContext Current
        {
            get
            {
                if(_perThreadStack == null)
                {
                    _perThreadStack = new Stack<MathContext>();
                    _perThreadStack.Push(_root);
                }
                return _perThreadStack.Peek();
            }
        }

        public static Guid CurrentInstanceId
        {
            get { return Current.InstanceId; }
        }

        public static MathContext Root
        {
            get { return _root; }
        }

        public static MathContext Create()
        {
            if(_perThreadStack == null)
            {
                _perThreadStack = new Stack<MathContext>();
                _perThreadStack.Push(_root);
            }
            MathContext ctx = new MathContext(_perThreadStack.Peek(), _perThreadStack);
            _perThreadStack.Push(ctx);
            return ctx;
        }

        private MathContext _parent;
        private Stack<MathContext> _localStack;
        private Guid _iid;

        private MathContext(MathContext parent, Stack<MathContext> stack)
        {
            _parent = parent;
            _localStack = stack;
            _iid = Guid.NewGuid();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing) // we only clean up managed resources
            {
                if(_root == null)
                    return; // root context
                if(!_localStack.Pop()._iid.Equals(_iid))
                    throw new Exceptions.MicrokernelException(Resources.ContextIllegalState);
                EventHandler<Events.MathContextEventArgs> handler = ContextExpired;
                if(handler != null)
                    handler(null, new Events.MathContextEventArgs(_iid));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Guid InstanceId
        {
            get { return _iid; }
        }

        public bool HasParent
        {
            get { return _parent != null; }
        }

        public MathContext ParentContext
        {
            get { return _parent; }
        }

        public bool IsRoot
        {
            get { return _iid.Equals(_root._iid); }
        }
    }
}
