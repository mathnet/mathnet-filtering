using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The interface of the MVC view.
    /// </summary>
    public interface IView
    {
        #region Events
        /// <summary>
        /// Occurs when the cursor is changed and the surface is supposed to set the cursor accordingly.
        /// </summary>
        event EventHandler<CursorEventArgs> OnCursorChange;

        event EventHandler<ColorEventArgs> OnBackColorChange;
        #endregion

        #region Properties

        Rectangle HorizontalRulerBounds { get;}
        Rectangle VerticalRulerBounds { get;}
        MeasurementsUnit RulerUnits { get;}
        /// <summary>
        /// Get the transformation matrix of the view
        /// </summary>
        Matrix ViewMatrix { get;}
        /// <summary>
        /// Gets the current cursor.
        /// </summary>
        /// <value>The current cursor.</value>
        System.Windows.Forms.Cursor CurrentCursor { get; set;}

        /// <summary>
        /// Gets the ghost.
        /// </summary>
        /// <value>The ghost.</value>
        IGhost Ghost { get;}

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        IModel Model { get; set; }

        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        ITracker Tracker { get; set;}
        /// <summary>
        /// Scales the diagram.
        /// </summary>
        /// <value>The scale.</value>
        SizeF Magnification { get; set;}
        /// <summary>
        /// Pans the view with the given shift.
        /// </summary>
        /// <value>The pan.</value>
        Point Origin {get;set;}
        /// <summary>
        /// Gets or sets whether the rulers are visible.
        /// </summary>
        bool ShowRulers { get;set;}
        /// <summary>
        /// Gets or sets whether the page is shown in the background.
        /// </summary>
        bool ShowPage { get;set;}
        #endregion

        #region Methods

        #region View to device
        Point ViewToDevice(PointF point);
        Size ViewToDevice(SizeF szView);
        PointF ViewToDeviceF(PointF ptView);
        Rectangle ViewToDevice(RectangleF rcView);
        void ViewToDevice(PointF[] viewPts, out Point[] devicePts);
        SizeF ViewToDeviceF(SizeF szView);
        RectangleF ViewToDeviceF(RectangleF rcView);
        #endregion

        #region World to view
        PointF WorldToView(PointF ptWorld);
        RectangleF WorldToView(RectangleF rectangle);
        void WorldToView(PointF[] worldPts, out PointF[] viewPts);
        SizeF WorldToView(SizeF szWorld);
        #endregion

        #region Device to view
        PointF DeviceToView(Point ptDevice);
        void DeviceToView(Point[] devicePts, out PointF[] viewPts);
        RectangleF DeviceToView(Rectangle rcDevice);
        SizeF DeviceToView(Size szDevice);
        #endregion

        #region View to world
        PointF ViewToWorld(PointF ptView);
        RectangleF ViewToWorld(RectangleF rcView);
        Rectangle ViewToWorld(Rectangle rcView);
        SizeF ViewToWorld(SizeF szView);
        void ViewToWorld(PointF[] viewPts, out PointF[] worldPts);
        #endregion

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        void Invalidate();
        /// <summary>
        /// Invalidates the specified rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        void Invalidate(System.Drawing.Rectangle rectangle);
        /// <summary>
        /// Paints the specified g.
        /// </summary>
        /// <param name="g">The g.</param>
        void Paint(System.Drawing.Graphics g);
        /// <summary>
        /// Paints the background.
        /// </summary>
        /// <param name="g">The g.</param>
        void PaintBackground(System.Drawing.Graphics g);
        /// <summary>
        /// Sets the type of the background.
        /// </summary>
        /// <param name="type">The type.</param>
        void SetBackgroundType(CanvasBackgroundTypes type);
        /// <summary>
        /// Resets the ghost.
        /// </summary>
        void ResetGhost();
        /// <summary>
        /// Resets the tracker.
        /// </summary>
        void ResetTracker();

        void HideTracker();
        /// <summary>
        /// Paints the ghost rectangle.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        void PaintGhostRectangle(Point start, Point end);
        /// <summary>
        /// Paints a ghost line.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        void PaintGhostLine(Point start, Point end);
        /// <summary>
        /// Paints a ghost multi-line
        /// </summary>
        /// <param name="cureveType">Type of the cureve.</param>
        /// <param name="points">The points.</param>
        void PaintGhostLine(MultiPointType cureveType, Point[] points);
        /// <summary>
        /// Paints the ghost ellipse.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        void PaintGhostEllipse(Point start, Point end);
        /// <summary>
        /// Paints the ants rectangle.
        /// </summary>
        /// <param name="ltPoint">The lt point.</param>
        /// <param name="rbPoint">The rb point.</param>
        void PaintAntsRectangle(Point ltPoint, Point rbPoint);
        /// <summary>
        /// Paints the tracker.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="showHandles">if set to <c>true</c> shows the handles.</param>
        void PaintTracker(Rectangle rectangle, bool showHandles);
        /// <summary>
        /// Attaches to model.
        /// </summary>
        /// <param name="model">The model.</param>
        void AttachToModel(IModel model);
        /// <summary>
        /// Shows the tracker.
        /// </summary>
        void ShowTracker();
        /// <summary>
        /// Suspends the invalidation of the view, which means that the 
        /// Invalidate() method calls from any entity will be discarded until Resume() has been called;
        /// </summary>
        void Suspend();
        /// <summary>
        /// Resumes the invalidation of the view.
        /// </summary>
        void Resume();
        #endregion

    }
}
