using System;
using System.Diagnostics;
using System.Xml;
using Netron.GraphLib.UI;
namespace Netron.GraphLib.IO.SVG
{
	/// <summary>
	/// Exports a diagram to SVG
	/// </summary>
	public class SVGSerializer
	{
		#region Fields
		private GraphControl site;
		#endregion

		#region Constructor
		/// <summary>
		/// Defautl constructor
		/// </summary>
		/// <param name="site"></param>
		public SVGSerializer(GraphControl site)
		{
			this.site = site;
		}

		#endregion

		#region Methods
		/// <summary>
		/// Serializes the given graph abstract to XML with the given XmlWriter
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="g"></param>
		public static void Serialize(XmlWriter writer, GraphAbstract g	)
		{
			writer.WriteStartElement("svg");
			writer.WriteAttributeString("xmlns","http://www.w3.org/2000/svg");
			WriteDefinitions(writer);
			WriteFilters(writer);
			WriteMetadata(writer, g);
			
			WriteConnections(writer, g);
			WriteShapes(writer, g);

			writer.WriteEndElement();
		}

		private static void WriteShapes(XmlWriter writer, GraphAbstract g)
		{
			writer.WriteStartElement("g");
			for(int k=0; k<g.Shapes.Count; k++) //<rect x="1140" y="30" width="100" height="20" style="fill: url(#two_hues); stroke: black;"/>
			{
				writer.WriteStartElement("rect");
				writer.WriteAttributeString("x", g.Shapes[k].X.ToString());
				writer.WriteAttributeString("y", g.Shapes[k].Y.ToString());
				writer.WriteAttributeString("width", g.Shapes[k].Width.ToString());
				writer.WriteAttributeString("height", g.Shapes[k].Height.ToString());
				writer.WriteAttributeString("rx", "2");//rounded rectangle				

				//writer.WriteAttributeString("style", "fill: url(#two_hues); stroke: black;");
				writer.WriteAttributeString("fill",  string.Concat("#", (g.Shapes[k].ShapeColor.ToArgb() & 0x00FFFFFF).ToString("X6")) );
				writer.WriteEndElement();
				
				//<text text-anchor="middle" x="{$x+50}" y="{$y+15}">
				writer.WriteStartElement("text");
				writer.WriteAttributeString("x", Convert.ToString(g.Shapes[k].X + 10));
				writer.WriteAttributeString("y", Convert.ToString(g.Shapes[k].Y + 15));
				writer.WriteAttributeString("text-anchor", "start");			
				writer.WriteAttributeString("font-size", "9");
				writer.WriteString(g.Shapes[k].Text);
				writer.WriteEndElement();
				
			}
			writer.WriteEndElement();
		}

		private static void WriteConnections(XmlWriter writer, GraphAbstract g)
		{
			/*
				<line class="edge" x1="{$x1}" y1="{$y1}" x2="{$x}" y2="{$y+50}">
				<xsl:attribute name="style">marker-end:url(#arrow)</xsl:attribute>
				</line>
			 */
			writer.WriteStartElement("g");
			for(int k=0; k<g.Connections.Count; k++) 
			{
				writer.WriteStartElement("line");
				writer.WriteAttributeString("x1", g.Connections[k].From.Location.X.ToString());
				writer.WriteAttributeString("y1", g.Connections[k].From.Location.Y.ToString());
				writer.WriteAttributeString("x2", g.Connections[k].To.Location.X.ToString());
				writer.WriteAttributeString("y2", g.Connections[k].To.Location.Y.ToString());				
				//writer.WriteAttributeString("style", "fill: url(#two_hues); stroke: black;");				
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private static void WriteMetadata(XmlWriter writer, GraphAbstract g)
		{
			writer.WriteStartElement("title");
			writer.WriteString(g.GraphInformation.Title);
			writer.WriteEndElement();
			writer.WriteStartElement("desc");
			writer.WriteString(g.GraphInformation.Description);
			writer.WriteEndElement();
		}

		private static void WriteDefinitions(XmlWriter writer)
		{
			writer.WriteRaw(@"<defs>
										<linearGradient id=""two_hues"">
												<stop offset=""0%"" style=""stop-color: #ffffff;""/>
												<stop offset=""100%"" style=""stop-color: #4682B4;""/>
										</linearGradient>
										</defs>"
			  );
//			writer.WriteStartElement("defs");
//			writer.WriteStartElement("linearGradient");
//			writer.WriteAttributeString("id", "two_hues");
//			writer.WriteStartElement("stop");
//			writer.WriteAttributeString("offset", "0%");
//			writer.WriteAttributeString("style","stop-color: #ffffff");
//			writer.WriteEndElement();//stop
//			writer.WriteStartElement("stop");
//			writer.WriteAttributeString("offset", "100%");
//			writer.WriteAttributeString("style","stop-color: #4682B4");
//			writer.WriteEndElement();//stop
//			writer.WriteEndElement();//linearGradient
//			writer.WriteEndElement();//defs


		}

		private static void WriteFilters(XmlWriter writer)
		{

			writer.WriteRaw(@"<filter id=""drop-shadow"">
											<feGaussianBlur in=""SourceAlpha"" stdDeviation=""2"" result=""blur""/>
											<feOffset in=""blur"" dx=""4"" dy=""4"" result=""offsetBlur""/>
											<feMerge>
												<feMergeNode in=""offsetBlur""/>
												<feMergeNode in=""SourceGraphic""/>
											</feMerge>
										</filter>"
									);

//			writer.WriteStartElement("filter");
//			writer.WriteAttributeString("id","dropShadow");
//			writer.WriteStartElement("feGaussianBlur");
//			writer.WriteAttributeString("in", "SourceAlpha");
//			writer.WriteAttributeString("stdDeviation","2");
//			writer.WriteAttributeString("result","blur");
//			writer.WriteEndElement();//feGaussianBlur
//			writer.WriteEndElement();//filter


		}
		/// <summary>
		/// Saves the diagram to SVG format
		/// </summary>
		/// <param name="fileName">the file-path</param>
		/// <param name="site">the graph-control instance to be serialized</param>
		/// <returns></returns>
		public static  bool SaveAs(string fileName, GraphControl site)
		{
			XmlTextWriter tw = null;
			try
			{

				tw = new XmlTextWriter(fileName,System.Text.Encoding.Unicode);
				tw.Formatting = System.Xml.Formatting.Indented;				
				Serialize(tw,site.Abstract);
				
				return true;
			}
			catch(Exception exc)			
			{				
				//TODO: more specific exception handling here
				Trace.WriteLine(exc.Message, "SVGSerializer.SaveAs");
			}
			catch
			{
				Trace.WriteLine("Non-CLS exception caught.","BinarySerializer.SaveAs");
			}
			finally
			{
				if(tw!=null) tw.Close();
			}
			return false;

		}
		#endregion
	}
}
