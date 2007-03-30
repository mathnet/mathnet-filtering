using System;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// The types of backgrounds the control can have
    /// </summary>
    public enum CanvasBackgroundTypes
    {

        /// <summary>
        /// Uniform flat colored
        /// </summary>
        FlatColor,
        /// <summary>
        /// Two-color gradient
        /// </summary>
        Gradient,
        /// <summary>
        /// A user defined image
        /// </summary>
        Image

    }
	/// <summary>
	/// An enuration of the shapes defined internally.
    /// <remarks>In earlier Netron libraries shapes were defined and loaded by means of attributes and reflection. This system is still possible and requires only a little bit of work (or you can copy the code from older versions).
    /// For simplicity and performance I decided however not to use reflection in this version; many people got confused and there are certain technicalities involved.
    /// The drawback of this is the fact that all shapes are defined in one big assembly and you need update this enum and to recompile when you add new shapes. Since the code is open source and most people recompile things anyway this is not really a problem.
    /// 
    /// </remarks>
	/// </summary>
    public enum ShapeTypes
	{
        /// <summary>
        /// The rectangular shape
        /// </summary>
		SimpleRectangle,
        /// <summary>
        /// The elliptic shape
        /// </summary>
		SimpleEllipse,
        /// <summary>
        /// The text label
        /// </summary>
		TextLabel,
        /// <summary>
        /// The class shape
        /// </summary>
		ClassShape,
        /// <summary>
        /// Text with no background or border
        /// </summary>
        TextOnly,
        /// <summary>
        /// A shape with an embedded bitmap
        /// </summary>
        ImageShape,       
        /// <summary>
        /// An example of a shape based on the <see cref="ComplexShapeBase"/>
        /// with a <see cref="FolderMaterial"/>.
        /// </summary>
        ComplexRectangle
	}
    /// <summary>
    /// Denotes the twofold way in which shapes can be selected. 
    /// </summary>
    public enum SelectionTypes
    {
        /// <summary>
        /// Entities have to be inside the selection.
        /// </summary>
        Inclusion,
        /// <summary>
        /// Entities are selected if the selection partially covers them.
        /// </summary>
        Partial
    }
    /// <summary>
    /// Enumerates the type of sortings
    /// </summary>
    public enum SortByType
    {
        /// <summary>
        /// By method
        /// </summary>
        Method = 0,
        /// <summary>
        /// By property
        /// </summary>
        Property = 1
    }

    /// <summary>
    /// Enumerates the sorting direction
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Ascending
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// Descending
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// The different types of transformations correspond
    /// to the eight grips of the tracker
    /// </summary>
    public enum TransformTypes
    { 
        /// <summary>
        /// North-west transformation
        /// </summary>
        NW,
        /// <summary>
        /// North transformation
        /// </summary>
        N,
        /// <summary>
        /// North-east transformation
        /// </summary>
        NE,
        /// <summary>
        /// East transformation
        /// </summary>
        E,
        /// <summary>
        /// South-east transformation
        /// </summary>
        SE,
        /// <summary>
        /// South transformation
        /// </summary>
        S,
        /// <summary>
        /// South-west transformation
        /// </summary>
        SW,
        /// <summary>
        /// West transformation
        /// </summary>
        W
    }
    /// <summary>
    /// The style of painting, see <see cref="IPaintStyle"/>
    /// </summary>
    public enum FillType
    {
        /// <summary>
        /// A solid color filling
        /// </summary>
        Solid = 0,
        /// <summary>
        /// A textured filling
        /// </summary>
        Texture = 1,
        /// <summary>
        /// A gradient filling
        /// </summary>
        LinearGradient = 2,

    }
    /// <summary>
    /// The two metric systems. 
    /// </summary>
    public enum MeasurementSystem
    {
        /// <summary>
        /// The British metric system.
        /// </summary>
        English = 0,
        /// <summary>
        /// The European metrix system.
        /// </summary>
        Metric = 1,

    }
    /// <summary>
    /// The units of measurement
    /// </summary>
    public enum MeasurementsUnit
    {
        /// <summary>
        /// One sixteenth of an inch.
        /// </summary>
        SixteenthInches = 0,
        /// <summary>
        /// One eigth of an inch.
        /// </summary>
        EighthInches = 1,
        /// <summary>
        /// A quarter of an inch.
        /// </summary>
        QuarterInches = 2,
        /// <summary>
        /// Half an inch.
        /// </summary>
        HalfInches = 3,
        /// <summary>
        /// An inch is an Imperial and U.S. customary unit of length.
        /// According to some sources, the inch was originally defined informally as the distance between the tip of the thumb and the first joint of the thumb. 
        /// Another source says that the inch was at one time defined in terms of the yard, supposedly defined as the distance between Henry I of England's 
        /// nose and his thumb. 
        /// However, this is unlikely because we have records of the unit being used circa 1000 CE (both Laws of Æthelbert and Laws of Ælfred), and Henry was born in 1068. In another version, the inch was defined as the length of three barleycorns. Because of the etymology of the word "inch" (see below), it is more likely that the inch is merely a unit derived from the foot.
        /// </summary>
        Inches = 4,
        /// <summary>
        /// A foot (plural: feet) is a non-SI unit of distance or length, measuring around a third of a meter. 
        /// There are twelve inches in one foot and three feet in one yard. 
        /// The international standard symbol for feet is ft .
        /// </summary>
        Feet = 5,
        /// <summary>
        /// A yard (abbreviation: yd) is an English unit of length, defined as three feet, thirty-six inches, 
        /// or 1/1760 of a mile, which is exactly 0.9144 metres in the modern, international definition. 
        /// </summary>
        Yards = 6,
        /// <summary>
        /// A mile is any of a number of units of distance, each in the magnitude of 1–10 km. In (contemporary) English contexts mile refers to the statute mile of 1760 yards, which is about 1609 m, or to the (international) nautical mile, being exactly 1852 m.
        /// </summary>
        Miles = 7,
        /// <summary>
        /// One tenth of a centimeter.
        /// </summary>
        Millimeters = 8,
        /// <summary>
        /// One hundreth of a meter
        /// </summary>
        Centimeters = 9,
        /// <summary>
        /// One meter.
        /// </summary>
        Meters = 10,
        /// <summary>
        /// A thousand meters.
        /// </summary>
        Kilometers = 11,
        /// <summary>
        /// One pixel.
        /// </summary>
        Points = 12,

    }

    public enum ShapeAlignment
    { 
        /// <summary>
        /// Shapes are aligned along their left edge.
        /// </summary>
        Left,
        /// <summary>
        /// Shapes are aligned along their right edge.
        /// </summary>
        Right,
        /// <summary>
        /// Shapes are aligned along their bottom edge.
        /// </summary>
        Bottom,
        /// <summary>
        /// Shapes are aligned along their top edge.
        /// </summary>
        Top,
        /// <summary>
        /// Shapes are aligned vertically along their center.
        /// </summary>
        Vertical,
        /// <summary>
        /// Shapes are aligned horizontally along their center.
        /// </summary>
        Horizontal
    }


}
