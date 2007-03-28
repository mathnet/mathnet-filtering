using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Mediator
{
    public interface ISystemMediatorSource
    {
        bool HasSystemMediator { get; }
        ISystemMediator SystemMediator { get; set; }
    }
}
