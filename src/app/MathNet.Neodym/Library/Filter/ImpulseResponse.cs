using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.SignalProcessing.Filter
{
    /// <summary>
    /// Specifies how a filter will respond to an impulse input.
    /// </summary>
    public enum ImpulseResponse
    {
        /// <summary>
        /// Impulse response always has a finite length of time and are stable, but usually have a long delay.
        /// </summary>
        Finite,
        /// <summary>
        /// Impulse response may have an infinite length of time and may be unstable, but usually have only a short delay.
        /// </summary>
        Infinite
    }
}
