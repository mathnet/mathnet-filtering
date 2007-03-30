using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout.Force
{

    ///<summary>
    /// Updates velocity and position data using the 4th-Order Runge-Kutta method.
    /// It is slower but more accurate than other techniques such as Euler's Method.
    /// The technique requires re-evaluating forces 4 times for a given timestep.
    /// </summary>
    public class RungeKuttaIntegrator : IIntegrator
    {


        /// <summary>
        /// Integrates the specified simulation.
        /// </summary>
        /// <param name="sim">The sim.</param>
        /// <param name="timestep">The timestep.</param>
        public void Integrate(ForceSimulator sim, long timestep)
        {
            float speedLimit = sim.SpeedLimit;
            float vx, vy, v, coeff;
            float[,] k, l;

            foreach (ForceItem item in sim.Items)
            {
                coeff = timestep / item.Mass;
                k = item.RungeKuttaTemp1;
                l = item.RungeKuttaTemp2;
                item.PreviousLocation[0] = item.Location[0];
                item.PreviousLocation[1] = item.Location[1];
                k[0, 0] = timestep * item.Velocity[0];
                k[0, 1] = timestep * item.Velocity[1];
                l[0, 0] = coeff * item.Force[0];
                l[0, 1] = coeff * item.Force[1];

                // Set the position to the new predicted position
                item.Location[0] += 0.5f * k[0, 0];
                item.Location[1] += 0.5f * k[0, 1];
            }

            // recalculate forces
            sim.Accumulate();

            foreach (ForceItem item in sim.Items)
            {
                coeff = timestep / item.Mass;
                k = item.RungeKuttaTemp1;
                l = item.RungeKuttaTemp2;
                vx = item.Velocity[0] + .5f * l[0, 0];
                vy = item.Velocity[1] + .5f * l[0, 1];
                v = (float)Math.Sqrt(vx * vx + vy * vy);
                if (v > speedLimit)
                {
                    vx = speedLimit * vx / v;
                    vy = speedLimit * vy / v;
                }
                k[1, 0] = timestep * vx;
                k[1, 1] = timestep * vy;
                l[1, 0] = coeff * item.Force[0];
                l[1, 1] = coeff * item.Force[1];

                // Set the position to the new predicted position
                item.Location[0] = item.PreviousLocation[0] + 0.5f * k[1, 0];
                item.Location[1] = item.PreviousLocation[1] + 0.5f * k[1, 1];
            }

            // recalculate forces
            sim.Accumulate();

            foreach (ForceItem item in sim.Items)
            {
                coeff = timestep / item.Mass;
                k = item.RungeKuttaTemp1;
                l = item.RungeKuttaTemp2;
                vx = item.Velocity[0] + .5f * l[1, 0];
                vy = item.Velocity[1] + .5f * l[1, 1];
                v = (float)Math.Sqrt(vx * vx + vy * vy);
                if (v > speedLimit)
                {
                    vx = speedLimit * vx / v;
                    vy = speedLimit * vy / v;
                }
                k[2, 0] = timestep * vx;
                k[2, 1] = timestep * vy;
                l[2, 0] = coeff * item.Force[0];
                l[2, 1] = coeff * item.Force[1];

                // Set the position to the new predicted position
                item.Location[0] = item.PreviousLocation[0] + 0.5f * k[2, 0];
                item.Location[1] = item.PreviousLocation[1] + 0.5f * k[2, 1];
            }

            // recalculate forces
            sim.Accumulate();

            foreach (ForceItem item in sim.Items)
            {
                coeff = timestep / item.Mass;
                k = item.RungeKuttaTemp1;
                l = item.RungeKuttaTemp2;
                float[] p = item.PreviousLocation;
                vx = item.Velocity[0] + l[2, 0];
                vy = item.Velocity[1] + l[2, 1];
                v = (float)Math.Sqrt(vx * vx + vy * vy);
                if (v > speedLimit)
                {
                    vx = speedLimit * vx / v;
                    vy = speedLimit * vy / v;
                }
                k[3, 0] = timestep * vx;
                k[3, 1] = timestep * vy;
                l[3, 0] = coeff * item.Force[0];
                l[3, 1] = coeff * item.Force[1];
                item.Location[0] = p[0] + (k[0, 0] + k[3, 0]) / 6.0f + (k[1, 0] + k[2, 0]) / 3.0f;
                item.Location[1] = p[1] + (k[0, 1] + k[3, 1]) / 6.0f + (k[1, 1] + k[2, 1]) / 3.0f;

                vx = (l[0, 0] + l[3, 0]) / 6.0f + (l[1, 0] + l[2, 0]) / 3.0f;
                vy = (l[0, 1] + l[3, 1]) / 6.0f + (l[1, 1] + l[2, 1]) / 3.0f;
                v = (float)Math.Sqrt(vx * vx + vy * vy);
                if (v > speedLimit)
                {
                    vx = speedLimit * vx / v;
                    vy = speedLimit * vy / v;
                }
                item.Velocity[0] += vx;
                item.Velocity[1] += vy;
            }
        }

    }
}
