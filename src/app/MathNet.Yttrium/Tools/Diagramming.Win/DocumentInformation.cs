using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Netron.Diagramming.Win
{
    /// <summary>
    /// Custom control to display the document information.
    /// </summary>
    public partial class DocumentInformation : UserControl
    {
        #region Events
       
        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the Author
        /// </summary>
        public string Author
        {
            get
            {
                if(AuthorTextbox != null)
                    return AuthorTextbox.Text;
                else
                    return string.Empty;
            }
            set
            {
                if(AuthorTextbox != null)
                    AuthorTextbox.Text = value;
            }
        }


        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title
        {
            get
            {
                if(TitleTextbox != null)
                    return TitleTextbox.Text;
                else
                    return string.Empty;
            }
            set
            {
                if(TitleTextbox != null)
                    TitleTextbox.Text = value;
            }
        }


        /// <summary>
        /// Gets or sets the CreationDate
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(CreationDateLabel.Text);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                try
                {
                    if(CreationDateLabel!=null)
                        CreationDateLabel.Text = value.ToString();
                }
                catch
                {
                    if(CreationDateLabel != null)
                        CreationDateLabel.Text = string.Empty;
                }
            }
        }


        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description
        {
            get
            {
                if(DescriptionTextbox != null)
                    return DescriptionTextbox.Text;
                else
                    return string.Empty;
            }
            set
            {
                if(DescriptionTextbox != null)
                    DescriptionTextbox.Text = value;
            }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentInformation()
        {
            InitializeComponent();
        } 
        #endregion
    }
}
