using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MathNet.Symbolics.Mediator
{
    public interface ICommand : ISerializable, IXmlSerializable
    {
        IMathSystem System { get; set;}
        void Execute();
        bool Done { get;}
        event EventHandler Executed;
        //void Unexecute();
    }
}
