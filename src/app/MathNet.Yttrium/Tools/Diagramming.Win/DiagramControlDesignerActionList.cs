using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Drawing;
using Netron.Diagramming.Core;
using System.Windows.Forms;
namespace Netron.Diagramming.Win
{
    class DiagramControlDesignerActionList : DesignerActionList
    {
        #region Fields
        private Document mDocument;
        #endregion

        #region Properties

        #region Properties
        /// <summary>
        /// Proxy property required by the SmartTag mechanism
        /// </summary>              
  //      [Browsable(true), Description("The background color of the canvas if the type is set to 'flat'"), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColor
        {
            get
            {
                
                return this.DiagramControl.BackColor;
                
            }
            set
            {
                SetProperty("BackColor", value);
                //Debug.Assert(this.DiagramControl != null);
                //this.DiagramControl.BackColor = value;
                //this.DiagramControl.Invalidate();
                //SetProperty("BackColor", value);
            }
        }        
        public CanvasBackgroundTypes BackgroundType
        {
            get {
                return this.DiagramControl.BackgroundType;
            }
            set { SetProperty("BackgroundType", value); }
        }
        #endregion
        public DiagramControl DiagramControl
        {
            get { return (this.Component as DiagramControl); }
        }

        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public DiagramControlDesignerActionList(IComponent component) : base(component)
        {
            AutoShow = true;
            mDocument = (component as DiagramControl).Document;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the (ordered) content of the smart-tag panel
        /// </summary>
        /// <returns></returns>
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            // Create list to store designer action items
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection();

            // Add Appearance category header text
            actionItems.Add(new DesignerActionHeaderItem("Appearance"));
            // Add Appearance category descriptive label
            actionItems.Add(new DesignerActionTextItem("Properties that affect how the diagram control looks.", "Appearance"));
            actionItems.Add(
                new DesignerActionPropertyItem(
                "BackgroundType",
                "BackgroundType",
                "Appearance",
                "The type of canvas background"));
            actionItems.Add(
              new DesignerActionPropertyItem(
                "BackColor",
                "BackColor",
                "Appearance",
                "The description..."));

            return actionItems;
        }
        
        // Helper method to acquire a ClockControlDesigner reference
        private DiagramControlDesigner Designer
        {
            get
            {
                IDesignerHost designerHost = (IDesignerHost)this.DiagramControl.Site.Container;
                return (DiagramControlDesigner) designerHost.GetDesigner(this.DiagramControl);
            }
        }

        // Helper method to safely set a component’s property
        private void SetProperty(string propertyName, object value)
        {
            // Get property
            PropertyDescriptor property = TypeDescriptor.GetProperties(this.DiagramControl)[propertyName];
            // Set property value
            property.SetValue(this.DiagramControl, value);
        }

        // Helper method to return the Category string from a
        // CategoryAttribute assigned to a property exposed by 
        //the document
        private string GetCategory( string propertyName)
        {
            
            PropertyInfo property = mDocument.GetType().GetProperty(propertyName);
            CategoryAttribute attribute = (CategoryAttribute)property.GetCustomAttributes(typeof(CategoryAttribute), false)[0];
            if (attribute == null) return null;
            return attribute.Category;
        }

        // Helper method to return the Description string from a
        // DescriptionAttribute assigned to a property exposed by 
        //the document
        private string GetDescription(string propertyName)
        {
            PropertyInfo property = mDocument.GetType().GetProperty(propertyName);
            DescriptionAttribute attribute = (DescriptionAttribute)property.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            if (attribute == null) return null;
            return attribute.Description;
        }
        #endregion

        

    }
}
