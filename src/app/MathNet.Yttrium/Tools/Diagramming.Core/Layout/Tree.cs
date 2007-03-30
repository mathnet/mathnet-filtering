using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace Netron.Diagramming.Core.Layout
{
    public static class Tree
    {

        public static Bundle CreateRandomTree<S>(Size bounds, int amount) where S : SimpleShapeBase, new()
        {
            if (new S().Connectors.Count == 0)
                throw new InconsistencyException("The shape type specified '" + typeof(S).Name + "' has no connectors and cannot be used to create a tree. Please choose another type based on the SimpleShapeBase class.");

            #region Preamble
            int counter = 0;
            Dictionary<int, S> labels = new Dictionary<int, S>();
            Random rnd = new Random();
            Bundle bundle = new Bundle();
            Connection cn;
            S shape;
            #endregion

            #region Create the root
            shape = new S();
            shape.Text = "Root";
            shape.Location = new Point(rnd.Next(10, bounds.Width - 30), rnd.Next(10, bounds.Height - 30));
            labels.Add(0, shape);
            bundle.Entities.Add(shape);
            //this.diagramControl1.SetLayoutRoot(shape);
            #endregion

            #region Add random nodes to the existing tree
            for (int i = 0; i < amount; i++)
            {
                counter++;
                shape = new S();
                shape.Text = "Shape " + counter;
                int s1;
                shape.Location = new Point(rnd.Next(10, bounds.Width - 30), rnd.Next(10, bounds.Height - 30));

                labels.Add(counter, shape);
                bundle.Entities.Add(shape);
                //let's try to find a parent for this new node
                s1 = rnd.Next(0, counter - 1);
                while (labels[s1].Connectors[0].AttachedConnectors.Count >= rnd.Next(1, 9) && s1 <= counter - 5)
                {
                    s1 = rnd.Next(0, counter - 1);
                }
                cn = new Connection(Point.Empty, Point.Empty);

                labels[s1].Connectors[0].AttachConnector(cn.From);
                shape.Connectors[0].AttachConnector(cn.To);
                bundle.Entities.Add(cn);

            }

            #endregion

            return bundle;
        }
     
        /// <summary>
        /// Only a utility function which returns a specific type of (handcrafted) tree.
        /// </summary>
        /// <returns></returns>
        public static Bundle CreateSpecificTree()
        {
            Bundle bundle = new Bundle();

            SimpleRectangle rec1 = new SimpleRectangle();
            rec1.Location = new Point(100, 30);

            SimpleRectangle rec2 = new SimpleRectangle();
            rec2.Location = new Point(50, 80);

            SimpleRectangle rec3 = new SimpleRectangle();
            rec3.Location = new Point(150, 80);


            Connection cn1 = new Connection();
            rec1.Connectors[2].AttachConnector(cn1.From);
            rec2.Connectors[0].AttachConnector(cn1.To);

            Connection cn2 = new Connection();
            rec1.Connectors[2].AttachConnector(cn2.From);
            rec3.Connectors[0].AttachConnector(cn2.To);

            bundle.Entities.Add(rec1);
            bundle.Entities.Add(rec2);
            bundle.Entities.Add(rec3);
            bundle.Entities.Add(cn1);
            bundle.Entities.Add(cn2);

            return bundle;


        }
    }
}
