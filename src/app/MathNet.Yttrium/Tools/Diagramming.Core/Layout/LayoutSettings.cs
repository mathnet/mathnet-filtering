using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core.Layout
{
    public static class LayoutSettings
    {
        public static class TreeLayout
        {
            /// <summary>
            /// Gets or sets the TreeOrientation of the standard/classic tree layout
            /// </summary>
            public static TreeOrientation TreeOrientation
            {
                get { return StandardTreeLayout.Orientation; }
                set { StandardTreeLayout.Orientation = value; }
            }

            /// <summary>
            /// Gets or sets the spacing between siblings.
            /// </summary>
            public static float BreadthSpacing
            {
                get { return StandardTreeLayout.BreadthSpacing;}
                set { StandardTreeLayout.BreadthSpacing = value; }
            }
            /// <summary>
            /// Gets or sets the spacing between parent-child.
            /// </summary>
            public static float DepthSpacing
            {
                get { return StandardTreeLayout.DepthSpacing;}
                set { StandardTreeLayout.DepthSpacing = value; }
            }

            public static float SubTreeSpacing
            {
                get { return StandardTreeLayout.SubTreeSpacing; }
                set { StandardTreeLayout.SubTreeSpacing = value; }
            }
        }
    }
}
