using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Netron.Diagramming.Core;
namespace Netron.Diagramming.Win
{
    /// <summary>
    /// Utility class to edit the text of the <see cref="TextOnly"/> shape.
    /// </summary>
    static class TextEditor
    {
        #region Fields
        /// <summary>
        /// the unique text editor
        /// </summary>
        private static TextEditorControl editor = null;
        /// <summary>
        /// the shape of which the text is being edited
        /// </summary>
        private static IShape currentShape;
        /// <summary>
        /// the site 
        /// </summary>
        private static DiagramControl diagramControl;
        /// <summary>
        /// the event handler is defined here so it can be attached and detached from the even as needed
        /// </summary>
        private static EventHandler<MouseEventArgs> onescape = new EventHandler<MouseEventArgs>(Controller_OnMouseDown);
        #endregion

        #region Properties

        /// <summary>
        /// Gets the shape.
        /// </summary>
        /// <value>The shape.</value>
        private static IShape Shape
        {
            get
            {
                return currentShape;
            }
        }
        /// <summary>
        /// Gets the text editor.
        /// </summary>
        /// <value>The editor.</value>
        private static TextEditorControl Editor
        {
            get
            {
                if (editor == null)
                    editor = new TextEditorControl();
                return editor;
            }

        }
        #endregion

        #region Methods

        /// <summary>
        /// Inits with the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public static void Init(DiagramControl parent)
        {
            diagramControl = parent;
            parent.Controls.Add(Editor);
            Editor.Visible = false;
        }

        /// <summary>
        /// Gets the editor.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public static TextEditorControl GetEditor(IShape shape)
        {
            if (shape == null)
                throw new InconsistencyException("Cannot assign an editor to a 'null' shape.");
            currentShape = shape;
            Editor.Location = currentShape.Rectangle.Location;
            Editor.Width = currentShape.Rectangle.Width;
            Editor.Height = currentShape.Rectangle.Height;
            Editor.Font = ArtPallet.DefaultFont;
            Editor.BackColor = diagramControl.BackColor;//            currentShape.ShapeColor;
            Editor.Visible = false;
            return Editor;
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public static void Show()
        {
            if (currentShape == null)
                return;
            Selection.Clear();
            diagramControl.View.ResetTracker();
            diagramControl.Controller.SuspendAllTools();
            diagramControl.Controller.Enabled = false;
            diagramControl.Controller.OnMouseDown += onescape;
            Editor.Visible = true;
            Editor.Text = (currentShape as SimpleShapeBase).Text;
            Editor.SelectionLength = Editor.Text.Length;
            Editor.ScrollToCaret();
            Editor.Focus();
        }

        /// <summary>
        /// Handles the OnMouseDown event of the Controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        static void Controller_OnMouseDown(object sender, MouseEventArgs e)
        {
            Hide();
            diagramControl.Controller.OnMouseDown -= onescape;
        }
        /// <summary>
        /// Hides this instance.
        /// </summary>
        public static void Hide()
        {
            if (currentShape == null)
                return;
            diagramControl.Controller.Enabled = true;
            diagramControl.Focus();
            Editor.Visible = false;
            (currentShape as SimpleShapeBase).Text = Editor.Text;
            diagramControl.Controller.UnsuspendAllTools();
            currentShape = null;
        }
        #endregion

        #region The texteditor control
        /// <summary>
        /// Only a pre-defined textbox used as a singleton to edit text in the shapes.
        /// </summary>
        internal class TextEditorControl : TextBox
        {

            public TextEditorControl()
                : base()
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.Multiline = true;
                this.ScrollBars = ScrollBars.None;
                this.WordWrap = true;
                this.BackColor = Color.White;
            }

        }
        #endregion
    }

}
