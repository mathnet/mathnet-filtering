using System;
using System.Collections.Generic;
using System.Text;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Shape factory for the internally defined shape types.
    /// </summary>
    public static class ShapeFactory
    {

        /// <summary>
        /// Gets the shape.
        /// </summary>
        /// <param name="shapeName">Name of the shape.</param>
        /// <returns></returns>
        public static IShape GetShape(string shapeName)
        {
            if(string.IsNullOrEmpty(shapeName))
                return null;

            foreach(string shapeType in Enum.GetNames(typeof(ShapeTypes)))
            {
                if(shapeType.ToString().ToLower() ==shapeName.ToLower())
                {
                      return GetShape((ShapeTypes) Enum.Parse(typeof(ShapeTypes), shapeType));                    
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the shape.
        /// </summary>
        /// <param name="shapeType">Type of the shape.</param>
        /// <returns></returns>
        public static IShape GetShape(ShapeTypes shapeType)
        {
            switch(shapeType)
            {
                case ShapeTypes.SimpleRectangle:
                    return new SimpleRectangle();
                case ShapeTypes.SimpleEllipse:
                    return new SimpleEllipse();
                case ShapeTypes.TextLabel:
                    return new TextLabel();
                case ShapeTypes.ClassShape:
                    return new ClassShape();
                case ShapeTypes.TextOnly:
                    return new TextOnly();                    
                case ShapeTypes.ImageShape:
                    return new ImageShape();                
                case ShapeTypes.ComplexRectangle:
                    return new ComplexRectangle();
                default:
                    return null;
            }

            return null;
        }
  
    }
}
