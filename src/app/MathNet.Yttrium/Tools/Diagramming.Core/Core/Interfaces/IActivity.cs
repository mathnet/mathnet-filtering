using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// This defines what an action or activity is on the diagram, which can be a data manipulation or layout or an asynchronuous dump of data.
	/// </summary>
	public interface IActivity 
	{
        //In the final analysis, your understanding of how to do construction determines how good a programmer you are

		#region Events
        /// <summary>
        /// 
        /// </summary>
        event EventHandler OnActivityRun;

		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IActivity"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set;}
        /// <summary>
        /// Gets or sets the step time.
        /// </summary>
        /// <value>The step time.</value>
        TimeSpan StepTime { get; set;}
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        DateTime StartTime { get; set;}
        /// <summary>
        /// Gets the duration of the activity.
        /// </summary>
        /// <value>The duration.</value>
        TimeSpan Duration { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get;}

		#endregion

		#region Methods
		/// <summary>
		/// Runs the activity
		/// </summary>
		void Run();

        /// <summary>
        /// Runs the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        void Run(int time);

        /// <summary>
        /// Runs the after.
        /// </summary>
        /// <param name="firstActivity">The first activity.</param>
        void RunAfter(IActivity firstActivity);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
        
		#endregion
	}
}
