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
    class PasteTool : AbstractTool
    {

        #region Fields
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:HoverTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public PasteTool(string name)
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
 

            try
            {
                IDataObject data = Clipboard.GetDataObject();
                string format = typeof(CopyTool).FullName;
                if (data.GetDataPresent(format))
                {
                    MemoryStream stream = data.GetData(format) as MemoryStream;
                    CollectionBase<IDiagramEntity> collection = null;
                    GenericFormatter<BinaryFormatter> f = new GenericFormatter<BinaryFormatter>();
                    //Anchors collection is a helper collection to re-connect connectors to their parent
                    Anchors.Clear();
                    //but is it actually a stream coming this application?
                    collection = f.Deserialize<CollectionBase<IDiagramEntity>>(stream);
                    
                  

                    if (collection != null)
                    {
                        #region Unwrap the bundle
                        this.Controller.Model.Unwrap(collection);
                        Rectangle rec = Utils.BoundingRectangle(collection);
                        rec.Inflate(30, 30);
                        this.Controller.View.Invalidate(rec);
                        #endregion
                    }

                }
                else if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap bmp = data.GetData(DataFormats.Bitmap) as Bitmap;
                    if (bmp != null)
                    {
                        #region Unwrap into an image shape
                        //TODO: Insert the image shape here
                        #endregion
                    }
                }

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
