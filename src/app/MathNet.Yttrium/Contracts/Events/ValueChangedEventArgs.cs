using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Events
{
    public class ValueChangedEventArgs<TValue, THost>
        : EventArgs
    {
        private readonly TValue _oldValue;
        private readonly TValue _newValue;
        private readonly THost _host;

        public ValueChangedEventArgs(THost host, TValue oldValue, TValue newValue)
        {
            _host = host;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public THost Host
        {
            get { return _host; }
        }

        public TValue OldValue
        {
            get { return _oldValue; }
        }

        public TValue NewValue
        {
            get { return _newValue; }
        }
    }
}
