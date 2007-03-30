//To localize the descriptions see http://groups.google.be/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/3bb6895b49d7cbe/e3241b7fa085ba90?lnk=st&q=csharp+attribute+resource+file&rnum=4#e3241b7fa085ba90
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The document class represents the root of the controls' data hierarchy. The document is the root of the serialization graph and contains both the data of the diagram(s)
    /// and the metadata (or user information).
    /// </summary>
    public partial class Document 
    {
        #region Fields
        /// <summary>
        /// pointer to the model
        /// </summary>       
        private IModel mModel;
        /// <summary>
        /// the Information field
        /// </summary>
        private DocumentInformation mInformation;
        #endregion

        #region Properties
    



        /// <summary>
        /// Gets or sets the Information
        /// </summary>
        public DocumentInformation Information
        {
            get
            {
                return mInformation;
            }
            set
            {
                mInformation = value;
            }
        }


        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public IModel Model
        {
            get
            {
                return mModel;
            }
        }

        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor. Creates a new document and, hence, a new model with one default page and one default layer.
        ///</summary>
        public Document()
        {
            mModel = new Model();
            mInformation = new DocumentInformation();
        }

    
        #endregion

        #region Methods

        #endregion

      

    
    }
}
