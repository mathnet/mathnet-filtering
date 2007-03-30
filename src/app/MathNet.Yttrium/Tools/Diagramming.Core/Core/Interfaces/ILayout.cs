using System;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// This interface describes the elements of a layout module.
	/// The single call to the inherited Run() method will organize the diagram in 
	/// a certain way.
	/// </summary>
	public interface ILayout : IAction
	{
		#region Events
		
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the bounds of the layout algorithm.
        /// </summary>
        /// <value>The bounds.</value>
        Rectangle Bounds { get;set;}
        PointF Center { get;set;}
		#endregion

		#region Methods
		
		#endregion
	}
}
