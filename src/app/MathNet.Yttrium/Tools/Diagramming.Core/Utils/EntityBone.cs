using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Bare bone structure of an entity related to a scaling transformation
    /// </summary>
    public struct EntityBone
    {
        /// <summary>
        /// the rectangle of the entity
        /// </summary>
        private Rectangle mRectangle;
        /// <summary>
        /// the connector points
        /// </summary>
        private Point[] mConnectorPoints;

        /// <summary>
        /// Gets or sets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public Rectangle Rectangle
        {
            get { return mRectangle; }
            set { mRectangle = value; }
        }


        /// <summary>
        /// Gets or sets the connector points.
        /// </summary>
        /// <value>The connector points.</value>
        public Point[] ConnectorPoints
        {
            get { return mConnectorPoints; }
            set { mConnectorPoints = value; }
        }
    }
}
