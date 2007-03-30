using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Pallet of cursors
    /// <remarks>
    /// You can define your own cursors or change the existing ones by dropping a cursor file in the Resources directory
    /// and set the compile flag to 'embedded resource'.
    /// </remarks>
    /// </summary>
    static class CursorPallet
    {
        #region Fields
        /// <summary>
        /// The root namespace
        /// </summary>
        private const string NameSpace = "Netron.Diagramming.Core";
        /// <summary>
        /// the grip cursor
        /// </summary>
        private static Cursor mGrip = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.Grip.cur"));
        /// <summary>
        /// the add cursor
        /// </summary>
        private static Cursor mAdd = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.Add.cur"));
        /// <summary>
        /// the cross cursor
        /// </summary>
        private static Cursor mCross = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.Cross.cur"));
        /// <summary>
        /// the move cursor
        /// </summary>
        private static Cursor mMove = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.Move.cur"));
        /// <summary>
        /// the selection cursor
        /// </summary>
        private static Cursor mSelection = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.Selection.cur"));
        /// <summary>
        /// the select cursor
        /// </summary>
        private static Cursor mSelect = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.Select.cur"));
        /// <summary>
        /// the drop text cursor
        /// </summary>
        private static Cursor mDropText = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.DropText.cur"));
        /// <summary>
        /// the drop shape cursor
        /// </summary>
        private static Cursor mDropShape = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.DropShape.cur"));
        /// <summary>
        /// the drop image cursor
        /// </summary>
        private static Cursor mDropImage = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(NameSpace + ".Resources.DropImage.cur"));
        

        #endregion 

        #region Properties

        /// <summary>
        /// Gets the drop-image cursor to reflect the creation of a new image-shape by dragdrop onto the canvas.
        /// </summary>
        /// <value>The drop-image cursor.</value>
        public static Cursor DropImage
        {
            get
            {
                return mDropImage;
            }
        }
        /// <summary>
        /// Gets the drop-text cursor to reflect the creation of a new text-shape by dragdrop onto the canvas.
        /// </summary>
        /// <value>The drop-text cursor.</value>
        public static Cursor DropText
        {
            get
            {
                return mDropText;
            }
        }
        /// <summary>
        /// Gets the drop-shape cursor to reflect the creation of a new shape by dragdrop onto the canvas.
        /// </summary>
        /// <value>The drop-shape cursor.</value>
        public static Cursor DropShape
        {
            get
            {
                return mDropShape;
            }
        }
        /// <summary>
        /// Gets the grip cursor to reflect the creation of a (new) connection .
        /// </summary>
        /// <value>The grip.</value>
        public static Cursor Grip
        {
            get
            {
                return mGrip;
            }
        }
        /// <summary>
        /// Gets the add to reflect the creation of a (new) drawing shape.
        /// </summary>
        /// <value>The add.</value>
        public static Cursor Add
        {
            get
            {
                return mAdd;
            }
        }

        /// <summary>
        /// Gets the cross cursor.
        /// </summary>
        /// <value>The cross.</value>
        public static Cursor Cross
        {
            get
            {
                return mCross;
            }
        }

        /// <summary>
        /// Gets the move cursor to reflect the motion of a selection over the canvas.
        /// </summary>
        /// <value>The move.</value>
        public static Cursor Move
        {
            get
            {
                return mMove;
            }
        }

        /// <summary>
        /// Gets the selection cursor to reflect the process of selection shapes by dragging over the canvas.
        /// </summary>
        /// <value>The selection.</value>
        public static Cursor Selection
        {
            get
            {
                return mSelection;
            }
        }

        /// <summary>
        /// Gets the select cursor to reflect to motion and selection of a single connector (<see cref="ConnectorMoverTool"/>).
        /// </summary>
        /// <value>The select.</value>
        public static Cursor Select
        {
            get
            {
                return mSelect;
            }
        }
        #endregion

    }
}
