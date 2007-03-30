using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{
    /// <summary>
    /// Interface for numerical integration routines. These routines are used
     /// to update the position and velocity of items in response to forces
     /// over a given time step.
    /// </summary>
    public interface IIntegrator
    {
        void Integrate(ForceSimulator sim, long timestep);
    }
}
