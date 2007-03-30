using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Interface to be returned by <see cref="IDiagramEntity"/>'s if additional actions have to be executed
    /// after being added to the <see cref="Model"/>. The object implementing this interface should be returned by the <see cref="IServiceProvider.GetService"/>method.
    /// 
    /// </summary>
    interface IAdditionCallback
    {
        void OnAddition();
    }
}
