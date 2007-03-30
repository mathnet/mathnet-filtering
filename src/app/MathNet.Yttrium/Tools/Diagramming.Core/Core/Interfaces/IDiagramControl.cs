using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Interface of the surface or diagram control
    /// </summary>
    public interface IDiagramControl
    {
        #region Events
        /// <summary>
        /// Occurs when a drag-and-drop operation is completed.
        /// </summary>
        event DragEventHandler DragDrop;
        /// <summary>
        /// Occurs when an object is dragged into the control's bounds.
        /// </summary>
        event DragEventHandler DragEnter;
        /// <summary>
        /// Occurs when an object is dragged out of the control's bounds.
        /// </summary>
        event EventHandler DragLeave;
        /// <summary>
        /// Occurs when an object is dragged over the control's bounds.
        /// </summary>
        event DragEventHandler DragOver;

        event GiveFeedbackEventHandler GiveFeedback;
        /// <summary>
        /// Occurs when the size of the canvas has changed
        /// </summary>
        /// <remarks>
        /// This event is usually defined already in the Control or ScrollableControl class from which
        /// the canvas inherits.
        /// </remarks>
        event EventHandler SizeChanged;
        /// <summary>
        /// Occurs when the mouse is pressed on the canvas
        /// </summary>
        /// <remarks>
        /// This event is usually defined already in the Control or ScrollableControl class from which
        /// the canvas inherits.
        /// </remarks>
        event MouseEventHandler MouseDown;
        /// <summary>
        /// Occurs when the mouse is released above the canvas
        /// </summary>
        /// <remarks>
        /// This event is usually defined already in the Control or ScrollableControl class from which
        /// the canvas inherits.
        /// </remarks>
        event MouseEventHandler MouseUp;
        /// <summary>
        /// Occurs when the mouse is moved over the canvas
        /// </summary>
        /// <remarks>
        /// This event is usually defined already in the Control or ScrollableControl class from which
        /// the canvas inherits.
        /// </remarks>
        event MouseEventHandler MouseMove;
        /// <summary>
        /// Occurs when the mouse pointer rests on the control.
        /// </summary>
        event EventHandler MouseHover;

        event MouseEventHandler MouseWheel;
        /// <summary>
        /// Occurs when a key is down
        /// </summary>
        event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a key is released
        /// </summary>
        event KeyEventHandler KeyUp;
        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        event KeyPressEventHandler KeyPress;
        /// <summary>
        /// Occurs when a new diagram is started.
        /// </summary>
        event EventHandler OnNewDiagram;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the scroll position.
        /// </summary>
        Point AutoScrollPosition { get;set;}
        /// <summary>
        /// Gets the client rectangle.
        /// </summary>
        /// <value>The client rectangle.</value>
        Rectangle ClientRectangle { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Invalidates the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        void Invalidate(Rectangle rectangle);
        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        void Invalidate();
        /// <summary>
        /// Focuses this instance.
        /// </summary>
        /// <returns></returns>
        bool Focus();
        /// <summary>
        /// Opens the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void Open(string path);
        /// <summary>
        /// Saves as.
        /// </summary>
        /// <param name="path">The path.</param>
        void SaveAs(string path);
        /// <summary>
        /// News the document.
        /// </summary>
        void NewDocument();
        #endregion
        
    }
}
