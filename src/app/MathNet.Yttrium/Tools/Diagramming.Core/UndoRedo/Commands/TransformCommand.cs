using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// ICommand implementation of the transform action
    /// </summary>
    class TransformCommand : Command
    {
        #region Fields
        Hashtable transformers;
       
        double scalex, scaley;
        Point origin;
        #endregion

        #region Properties



        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TransformCommand"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="scalex">The scalex.</param>
        /// <param name="scaley">The scaley.</param>
        /// <param name="transformers">The transformers.</param>
        public TransformCommand(IController controller, Point origin, double scalex, double scaley, Hashtable transformers)
            : base(controller)
        {
            this.Text = "Transform selection";
            this.transformers = transformers;
            this.origin = origin;
            this.scalex = scalex;
            this.scaley = scaley;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Perform redo of this command.
        /// </summary>
        public override void Redo()
        {
            //necessary to invalidate
            Rectangle recBefore = CalculateRectangle(this.transformers);
            recBefore.Inflate(20, 20);
            Transform(this.origin, scalex, scaley, transformers);
            Rectangle recAfter = CalculateRectangle(this.transformers);
            recAfter.Inflate(20, 20);
            this.Controller.View.Invalidate(recBefore);
            this.Controller.View.Invalidate(recAfter);

        }
        /// <summary>
        /// Calculates the bounding rectangle of the transformed items.
        /// </summary>
        /// <param name="transformers">The transformers.</param>
        /// <returns></returns>
        public static Rectangle CalculateRectangle(Hashtable transformers)
        {
            Rectangle rec = Rectangle.Empty;
            if(transformers == null || transformers.Count == 0)
                return Rectangle.Empty;
            bool first = true;
            foreach(object val in transformers.Values)
            {
                EntityBone bone = (EntityBone) val;
                if(bone.Rectangle.Equals(Rectangle.Empty))
                    continue;
                if(first)
                {
                    rec = bone.Rectangle;
                    first = false;
                }
                else
                    rec = Rectangle.Union(rec, bone.Rectangle);

            }
            return rec;
        }

        /// <summary>
        /// Perform undo of this command.
        /// </summary>
        public override void Undo()
        {
            //necessary to invalidate
            Rectangle recBefore = CalculateRectangle(this.transformers);
            recBefore.Inflate(20, 20);
            Transform(this.origin, 1, 1, transformers);
            Rectangle recAfter = CalculateRectangle(this.transformers);
            recAfter.Inflate(20, 20);
            this.Controller.View.Invalidate(recBefore);
            this.Controller.View.Invalidate(recAfter);
        }

        public static void Transform(Point origin, double scalex, double scaley, Hashtable transformers)
        {
            Rectangle r;
            int x, y, w, h;
           
            //the new location of the connector
        
            EntityBone bone;
            
            IConnection conn;
            //Scale the entities; this could be done via matrix transform but it seems some rounding is necessary since I got terrible 
            //decimal accumulation mistakes without rounding off the double data type.
            //However, if one rotation of shapes will be added the matrix tranfromation will be unavoidable.
            foreach(object key in transformers.Keys)
            {
                bone = (EntityBone) transformers[key];
                r = bone.Rectangle;
                //Scale the rectangle if not empty. If the rectangle is empty it is a connection.
                if(r.Equals(Rectangle.Empty)) //the bone represents a connection
                {
                    conn = key as IConnection;
                    //scaling of the From connector
                    if(!bone.ConnectorPoints[0].Equals(Point.Empty))
                    {

                        x = Convert.ToInt32(Math.Round(((double) bone.ConnectorPoints[0].X - (double) origin.X) * scalex + origin.X - conn.From.Point.X, 1));
                        y = Convert.ToInt32(Math.Round(((double) bone.ConnectorPoints[0].Y - (double) origin.Y) * scaley + origin.Y - conn.From.Point.Y, 1));
                        //it's important to use the Move method because shifting the Point could entail additional moves on the attached connectors
                        conn.From.Move(new Point(x, y));
                    }
                    //scaling of the To connector
                    if(!bone.ConnectorPoints[1].Equals(Point.Empty))
                    {
                        x = Convert.ToInt32(Math.Round(((double) bone.ConnectorPoints[1].X - (double) origin.X) * scalex + origin.X - conn.To.Point.X, 1));
                        y = Convert.ToInt32(Math.Round(((double) bone.ConnectorPoints[1].Y - (double) origin.Y) * scaley + origin.Y - conn.To.Point.Y, 1));
                        conn.To.Move(new Point(x, y));
                    }
                }
                else    //it's a shape
                {
                    //TransformShape(ref origin, scalex, scaley, ref r, ref bone, key, out x, out y, out w, out h, out a, out b, out p, out shape);
                    x = Convert.ToInt32(Math.Round((r.X - origin.X) * scalex + origin.X, 1));
                    y = Convert.ToInt32(Math.Round((r.Y - origin.Y) * scaley + origin.Y, 1));
                    w = Convert.ToInt32(r.Width * scalex);
                    h = Convert.ToInt32(r.Height * scaley);
                    (key as IShape).Transform(x, y, w, h);

                }

            }

        }

        //private static void TransformShape(ref Point origin, double scalex, double scaley, ref Rectangle r, ref EntityBone bone, object key, out int x, out int y, out int w, out int h, out double a, out double b, out Point p, out IShape shape)
        //{


        //    //(key as IDiagramEntity).Rectangle = new Rectangle(x, y, w, h);

        //    if(bone.ConnectorPoints != null)
        //    {
        //        shape = key as IShape;
        //        for(int m = 0; m < bone.ConnectorPoints.Length; m++)
        //        {
        //            a = Math.Round(((double) bone.ConnectorPoints[m].X - (double) r.X) / (double) r.Width, 1) * w + x - shape.Connectors[m].Point.X;
        //            b = Math.Round(((double) bone.ConnectorPoints[m].Y - (double) r.Y) / (double) r.Height, 1) * h + y - shape.Connectors[m].Point.Y;
        //            p = new Point(Convert.ToInt32(a), Convert.ToInt32(b));
        //            shape.Connectors[m].Move(p);
        //        }
        //    }
        //}



        #endregion
    }

}
