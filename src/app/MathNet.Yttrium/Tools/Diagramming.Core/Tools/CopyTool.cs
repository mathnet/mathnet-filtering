using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This tool copies the selected entities to the clipboard
    /// </summary>
    class CopyTool : AbstractTool
    {

        #region Fields
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:HoverTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public CopyTool(string name)
            : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            if(Selection.SelectedItems.Count == 0)
                return;

            try
            {
                //Clear the Anchors otherwise subsequent Copy operations will raise an exception due to the fact that the Anchors class is a static helper class
                Anchors.Clear();
                //this will create a volatile collection of entities, they need to be unwrapped!
                //I never managed to get things working by putting the serialized collection directly onto the Clipboad. Thanks to Leppie's suggestion it works by
                //putting the Stream onto the Clipboard, which is weird but it works.
                MemoryStream copy = Selection.SelectedItems.ToStream();
                DataFormats.Format format = DataFormats.GetFormat(typeof(CopyTool).FullName);
                IDataObject dataObject = new DataObject();
                dataObject.SetData(format.Name, false, copy);
                Clipboard.SetDataObject(dataObject, false);
            }
            catch (Exception exc)
            {
                throw new InconsistencyException("The Copy operation failed.", exc);
            }
            finally
            {
                DeactivateTool();
            }
        }
        
        #endregion
    }

}
