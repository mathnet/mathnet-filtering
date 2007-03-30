using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Xml.Schema;
using Netron.GraphLib.Attributes;
using Netron.GraphLib.UI;
using System.Runtime.Remoting;
using System.Windows.Forms;
using System.Drawing;

namespace Netron.GraphLib.IO.NML
{
	/// <summary>
	/// NMLSerializer serializes a graph to NML
	/// Thanks to Martin Cully for his work on this.
	/// </summary>
	public class NMLSerializer
	{
		#region Fields
		private GraphControl site = null;
		private string dtdPath = "http://nml.graphdrawing.org/dtds/1.0rc/nml.dtd";


		//private Hashtable keyList = new Hashtable();

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		private NMLSerializer()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="site"></param>
		public NMLSerializer(GraphControl site)
		{
			this.site = site;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dtdPath"></param>
		public NMLSerializer(string dtdPath)
		{
			this.dtdPath = dtdPath;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Opens a NML serialized file
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="site"></param>
		public static void Open(string filename, GraphControl site)
		{
			try
			{
			XmlTextReader reader = new XmlTextReader(filename);
			IO.NML.NMLSerializer ser = new IO.NML.NMLSerializer(site);
			
			site.Abstract = ser.Deserialize(reader) as GraphAbstract;
			reader.Close();
			}
			catch (System.IO.DirectoryNotFoundException exc)
			{
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			catch(System.IO.FileLoadException exc)
			{				
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			catch (System.IO.FileNotFoundException exc)
			{
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			catch
			{				
				site.OutputInfo("Non-CLS exception caught.","BinarySerializer.SaveAs", OutputInfoLevels.Exception);
			}
		}

		/// <summary>
		/// Saves the diagram to NML format
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
				IO.NML.NMLSerializer g = new IO.NML.NMLSerializer();
				g.Serialize(tw,site.Abstract);
				
				return true;
			}
			catch(Exception exc)			
			{				
				//TODO: more specific exception handling here
				Trace.WriteLine(exc.Message, "NMLSerializer.SaveAs");
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
		
		
		#region Serialization
		/// <summary>
		/// Starts the serialization process. Takes the abstract of the graph and
		/// constructs a NMLType proxy-like object which will be serialized via the 
		/// standard .Net XmlSerializer process.
		/// </summary>
		/// <param name="writer">An XmlWriter</param>
		/// <param name="g">The GraphAbstract object to be serialized</param>
		public void Serialize(XmlWriter writer, GraphAbstract g	)
		{
			try
			{
				//the root of the NML
				NMLType nml = new NMLType(); 
				//add the version node
				nml.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				//the graph node 
				GraphType graph = new GraphType();	
				nml.Graph = graph;				
				//add the graph information				
				graph.GraphInformation = new GraphInformationType(g.GraphInformation);
				//serialize the shapes
				foreach ( Shape s in g.Shapes )				
					graph.Items.Add(SerializeNode(s));				
				//serialize the connections
				foreach(Connection c in g.Connections)
					graph.Items.Add(SerializeEdge(c));
				
				//			foreach(DictionaryEntry de in keyList)
				//			{
				//				nml.Key.Add(BuildKeyType((String)de.Key));
				//			}

			
				// serialize
				XmlSerializer ser = new XmlSerializer(typeof(NMLType));
				
				ser.Serialize(writer,nml);
	
			}

			
			catch(Exception exc)
			{
				site.OutputInfo(exc.Message, "NMLSerializer.Serialize", OutputInfoLevels.Exception);
			}
			catch
			{
				site.OutputInfo("Non-CLS exception caught.", "BinarySerializer.Serialize", OutputInfoLevels.Exception);
			}
			finally
			{
						
			}
		}

		/// <summary>
		/// Returns the NML representation of the given GraphAbstract
		/// </summary>	
		/// <returns></returns>
		public string Serialize()
		{
			try
			{
				GraphAbstract g = this.site.Abstract;
				System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();

				MemoryStream stream = new MemoryStream();
				XmlTextWriter writer = new XmlTextWriter(stream, System.Text.ASCIIEncoding.ASCII);
				writer.Formatting = System.Xml.Formatting.Indented;
				this.Serialize(writer,g);

				
				//StringReader reader = new StringReader();
				int count = 0;
				stream.Seek(0, SeekOrigin.Begin);

				byte[] byteArray = new byte[stream.Length];

				while(count < stream.Length)
				{
					byteArray[count++] = Convert.ToByte(stream.ReadByte());
				}

				// Decode the byte array into a char array 
				// and write it to the console.
				char[] charArray = new char[asciiEncoding.GetCharCount(byteArray, 0, count)];
				asciiEncoding.GetDecoder().GetChars(byteArray, 0, count, charArray, 0);

				string s = new string(charArray);
				
				
				writer.Close();

				stream.Close();
				return s;
			}
			catch(Exception exc)
			{
				Trace.WriteLine(exc.Message,"NMLSerializer.Serialize");
				return string.Empty;
			}
		}

		/// <summary>
		/// Serializes a node
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private ShapeType SerializeNode(Shape s)
		{
			PropertiesHashtable properties = GraphMLDataAttribute.GetValuesOfTaggedFields(s);

			ShapeType node = new ShapeType();
			node.UID = FormatID(s);			
			node.InstanceKey = s.Summary.Key;
			foreach(Connector c in s.Connectors)
			{
				ConnectorType ct = new ConnectorType();
				ct.Name = c.Name;
				ct.UID = c.UID.ToString();
				node.Data.Add(ct);
			}


			//--------------------------------------
			foreach(DataType data in DataTypesFromAttributes(properties))
			{
				node.Data.Add(data);
			}

			return node;
		}
		/// <summary>
		/// Serializes an edge
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private ConnectionType SerializeEdge(Connection c)
		{
			PropertiesHashtable properties = GraphMLDataAttribute.GetValuesOfTaggedFields(c);

			ConnectionType edge = new ConnectionType();
			edge.ID = FormatID(c);
			
			/* Save the connectors the Connection is connected to */
			edge.Source = FormatID(c.From.BelongsTo);
			edge.Target = FormatID(c.To.BelongsTo);
			/* Save the connectors the Connection is connected to */
			edge.Sourceport = FormatID(c.From);
			edge.Targetport = FormatID(c.To);			
			edge.InstanceKey = c.Summary.Key;
			
			foreach(DataType dt in DataTypesFromAttributes(properties))
			{
				edge.Data.Add(dt);
			}

			return edge;
		}
		#endregion
		
		#region Deserialize
		/// <summary>
		/// Deserializes the graph's xml
		/// </summary>
		/// <returns></returns>
		public GraphAbstract Deserialize(XmlReader reader)
		{
			//very important; always use the same instantiation parameter of the XmlSerializer as
			//you used for serialization!
			XmlSerializer ser = new XmlSerializer(typeof(NMLType));
			if(ser.CanDeserialize(reader))
			{
				NMLType gtype = ser.Deserialize(reader) as NMLType;
				
				return Deserialize(gtype);
			}
			else
				throw new Exception("The supplied file cannot be deserialized to a graph (check the XML)");
		}

		/// <summary>
		/// Deserializes the presumed NML-string to a GraphAbstract object
		/// </summary>
		/// <param name="xml">NML compliant string</param>
		/// <returns></returns>
		public GraphAbstract Deserialize(string xml)
		{
			StringReader sr = new StringReader(xml);
			XmlTextReader reader = new XmlTextReader(sr);
			return Deserialize(reader);
		}

		/// <summary>
		/// Deserializes the graphtype, here's where all the smart stuff happens
		/// </summary>
		/// <param name="gml">the graphtype which acts as an intermediate storage between XML and the GraphAbstract
		/// </param>
		/// <returns></returns>
		private GraphAbstract Deserialize(NMLType gml)
		{
			GraphAbstract abs = new GraphAbstract();
			
			#region Load the graph information
			GraphType g = gml.Graph;
			
			abs.GraphInformation = g.GraphInformation.ToGraphInformation();		

			#endregion
			Shape shape = null;
			ShapeType node;
			DataType dt;				
			ConnectorType ct;
			Connection con = null;
			ConnectionType et;
			string linePath = string.Empty; //see the split deserialization of the connection
			
			FromToCollection ftc = new FromToCollection(); //temporary store for from-to relations of connections
			Hashtable connectors = new Hashtable(); //temporary collection of connector
			#region Loop over all items

			

			for(int k =0; k<g.Items.Count;k++) //loop over all serialized items
			{
				try
				{
					if(g.Items[k] is ShapeType)
					{
						Trace.WriteLine("Node: " + (g.Items[k] as ShapeType).UID,"NMLSerializer.Deserialize");
						node = g.Items[k] as ShapeType;

						#region find out which type of shape needs to be instantiated
						if(node!=null && node.InstanceKey!=string.Empty)
							shape = GetShape(node.InstanceKey);	
						if(shape==null)
						{
							Trace.WriteLine("...but failed to instantiate the appropriate shape (missing or not loaded library?", "NMLSerializer.Deserialize");
							continue;
						}

						#endregion
						#region use the attribs again to reconstruct the props				
						for(int m=0; m<node.Data.Count;m++) //loop over the serialized data
						{
							if(node.Data[m] is DataType)
							{
								#region Handle data node
								dt = node.Data[m] as DataType;												
								if(dt==null) continue;

								foreach (PropertyInfo pi in shape.GetType().GetProperties()) 
								{	
									if (Attribute.IsDefined(pi, typeof(GraphMLDataAttribute))) 
									{
										try
										{
											if(pi.Name==dt.Name)
											{
															
												if(pi.GetIndexParameters().Length==0)
												{
													if(pi.PropertyType.Equals(typeof(int)))
														pi.SetValue(shape,Convert.ToInt32(dt.Value[0]),null);
													else if(pi.PropertyType.Equals(typeof(Color))) //Color is stored as an integer
														pi.SetValue(shape,Color.FromArgb(int.Parse(dt.Value[0].ToString())),null);
													else if(pi.PropertyType.Equals(typeof(string)))
														pi.SetValue(shape,(string)(dt.Value[0]),null);
													else if(pi.PropertyType.Equals(typeof(bool)))
														pi.SetValue(shape,Convert.ToBoolean(dt.Value[0]),null);	
													else if(pi.PropertyType.Equals(typeof(Guid)))
														pi.SetValue(shape,new Guid((string) dt.Value[0]),null);	
													else if(pi.PropertyType.Equals(typeof(float)))
														pi.SetValue(shape, Convert.ToSingle(dt.Value[0]),null);
													else if(pi.PropertyType.BaseType.Equals(typeof(Enum)))//yesyes, you can imp/exp enum types too
														pi.SetValue(shape, Enum.Parse(pi.PropertyType,dt.Value[0].ToString()),null);													
													else if(dt.IsCollection)
													{
														#region Some explanations
														/* OK, here's the deal. 
														 * This part is related to the possibility to imp/exp property-collections from/to NML.
														 * However, since (see more specifically the ClassShape) the collections will usually be defined
														 * where the shapes is defined, i.e. in an external assembly, the collection-type is unknown.
														 * Importing/exporting collections to NML is itsel a problem and in this part of the code the data is reflected again.
														 * To bypass the problem that the collection-type is unknown I hereby assume that the collection can be built up again via
														 * a public constructor of the collection with argument 'ArrayList'. In the ArrayList the collection-elements are of type string[]
														 * whereby the order of the strings reflect the order of the GraphMLData-tagged properties of the collection elements.
														 * For example, the ClassPropertiesCollection has the required constructor and the ClassProperty object is instantiated via the string[] elements in the
														 * ArrayList.
														 * Of course, this brings some restriction but it's for the moment the most flexible way I have found.
														 * If the ClassProperty would itself have a property inheriting from CollectionBase this will not work...one has to draw a line somewhere.
														 * It's the price to pay for using reflection and external assemblies. The whole story can be forgotten if you link the shape-classes at compile-time.
														 * Remember; the Netron graphlib is more a toolkit than a all-in solution to all situations. 
														 * However, the current implementation will cover, I beleive, 90% of the needs.
														 */
														#endregion
														
														ArrayList list = new ArrayList();
														for(int p =0; p<dt.Value.Count; p++) //loop over the collection elements
														{
															DataType dat = dt.Value[p] as DataType; //take a collection element
															if(dat.IsCollection)	//is it itself a collection?
															{
																string[] str = new string[dat.Value.Count];
																for(int l=0;l<dat.Value.Count;l++)
																{
																	if((dat.Value[l] as DataType).Value.Count>0)
																		str[l] = (string) (dat.Value[l] as DataType).Value[0];
																	else
																		str[l] = string.Empty;
																}
																list.Add(str); 
															}
															else
															{
																list.Add(new string[]{(string) dat.Value[0]}); 
															}
															
														}
														object o;
														o = pi.PropertyType.GetConstructor(new Type[]{typeof(ArrayList)}).Invoke(new Object[]{list});
														pi.SetValue(shape,o,null);
														Trace.WriteLine("'" + dt.Name + "' is an array type","NMLSeriliazer.Deserialize");
													}

												}
												else
													pi.SetValue(shape,dt.Value,null);
												Trace.WriteLine("'" + dt.Name + "' deserialized.","NMLSeriliazer.Deserialize");
												break;
											}
										}
										catch(Exception exc)
										{
											Trace.WriteLine("Failed '" + dt.Name +"': " + exc.Message,"NMLSeriliazer.Deserialize");
											continue;//just try to make the best out of it
										}
														
									}
								
								}
								#endregion
							}
							else if (node.Data[m] is ConnectorType)
							{
								#region Handle connector data
								ct = node.Data[m] as ConnectorType;
								foreach(Connector c in shape.Connectors)
									if(c.Name==ct.Name)
									{
										c.UID = new Guid(ct.UID);
										break;
									}
								#endregion
							}
						
						}
						#endregion
						//at this point the shape is fully deserialized
						//but we still need to assign the ambient properties,
						//i.e. the properties associated to the current hosting of the control
						if(shape !=null)
						{
							shape.Site = site;
							shape.PostDeserialization();
							shape.Font = site.Font;
							//shape.FitSize(false);
							abs.Shapes.Add(shape);
							//keep the references to the connectors, to be used when creating the connections
							foreach(Connector cor in shape.Connectors)
								connectors.Add(cor.UID.ToString(),cor);
						}
					}
					else if(g.Items[k] is ConnectionType)
					{
						#region  handle the edge
						//we cannot create the connection here since not all shapes have been instantiated yet
						//keep the edges in temp collection, treated in next loop
						et = g.Items[k] as ConnectionType;
						con = new Connection(site);
						con.Font = site.Font;
						con.UID = new Guid(et.ID);
						Trace.WriteLine("Connection: " + et.ID,"NMLSeriliazer.Deserialize");
						#region use the attribs to reconstruct the props


						for(int m=0; m<et.Data.Count;m++) //loop over the serialized data
						{
							if(et.Data[m] is DataType)
							{
								#region Handle data node, same as the shape
								dt = et.Data[m] as DataType;												
								if(dt==null) continue;

								foreach (PropertyInfo pi in con.GetType().GetProperties()) 
								{	
									if (Attribute.IsDefined(pi, typeof(GraphMLDataAttribute))) 
									{
										try
										{
											if(pi.Name==dt.Name)
											{
												if(dt.Name=="LinePath")
												{
													//the LinePath will not work without non-null From and To, so set it afterwards
													linePath = dt.Value[0].ToString();
												}
												else if(pi.GetIndexParameters().Length==0)
												{
													if(pi.PropertyType.Equals(typeof(int)))
														pi.SetValue(con,Convert.ToInt32(dt.Value[0]),null);
													else if(pi.PropertyType.Equals(typeof(Color))) //Color is stored as an integer
														pi.SetValue(con,Color.FromArgb(int.Parse(dt.Value[0].ToString())),null);
													else if(pi.PropertyType.Equals(typeof(string)))
														pi.SetValue(con,(string)(dt.Value[0]),null);
													else if(pi.PropertyType.Equals(typeof(bool)))
														pi.SetValue(con,Convert.ToBoolean(dt.Value[0]),null);	
													else if(pi.PropertyType.Equals(typeof(Guid)))
														pi.SetValue(con,new Guid((string) dt.Value[0]),null);	
													else if(pi.PropertyType.Equals(typeof(float)))
														pi.SetValue(con, Convert.ToSingle(dt.Value[0]),null);
													else if (pi.PropertyType.Equals(typeof(ConnectionWeight)))
														pi.SetValue(con, Enum.Parse(typeof(ConnectionWeight),dt.Value[0].ToString()),null);
													else if (pi.PropertyType.Equals(typeof(System.Drawing.Drawing2D.DashStyle)))
														pi.SetValue(con, Enum.Parse(typeof(System.Drawing.Drawing2D.DashStyle),dt.Value[0].ToString()),null);

												}
												else
													pi.SetValue(con,dt.Value,null);
												Trace.WriteLine("'" + dt.Name + "' deserialized.","NMLSeriliazer.Deserialize");
												break;
											}
										}
										catch(Exception exc)
										{
											Trace.WriteLine("Failed '" + dt.Name +"': " + exc.Message,"NMLSeriliazer.Deserialize");
											continue;//just try to make the best out of it
										}
														
									}
								
								}
								#endregion
							}
						}

						#endregion
						ftc.Add(new FromTo(et.Sourceport,et.Targetport, con));
						#endregion
					}
				}
				catch(Exception exc)
				{
					Trace.WriteLine(exc.Message,"NMLSeriliazer.Deserialize");
					continue;
				}

			}//loop over items in the graph-XML
			#endregion

			#region now for the edges;
			//loop over the FromTo collections and pick up the corresponding connectors
			for(int k=0; k<ftc.Count; k++)
			{
				try
				{
					con = ftc[k].Connection;
					con.From = connectors[ftc[k].From] as Connector;
					con.To = connectors[ftc[k].To] as Connector;					
					con.From.Connections.Add(con);//if From is null we'll fail in the catch and continue
					con.LinePath = linePath; //only setable after the From and To are found
					con.To.Connections.Add(con);				
					abs.Insert(con);
					Trace.WriteLine("Connection '" + con.UID + "' added.","NMLSeriliazer.Deserialize");
				}
				catch(Exception exc)
				{
					Trace.WriteLine("Connection failed: " + exc.Message,"NMLSeriliazer.Deserialize");
					continue; //make the best of it
				}
			}
			#endregion
			

			//			for(int n=0; n<pcs.Count; n++)
			//			{	
			//				from = pcs[n].ChildShape;
			//				to = abs.Shapes[pcs[n].Parent];
			//				con = new Connection(from, to );
			//				abs.Connections.Add(con);
			//				con.site = site;
			//				if(pcs[n].ChildShape.visible)
			//					con.visible = true;
			//				from.connection = con;	//a lot of crossing...to make life easy really
			//				from.parentNode =to;
			//				to.childNodes.Add(from);
			//				
			//				
			//			}

			return abs;

		}

		#endregion

		
	
		#endregion

		#region Helper Functions
		/// <summary>
		/// Validation of the XML
		/// </summary>
		/// <param name="reader"></param>
		public static void Validate(XmlReader reader)
		{
			//TODO: looks a little odd this one
			XmlValidatingReader vr = new XmlValidatingReader(reader);

			vr.ValidationType = ValidationType.Auto;
			vr.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);

			while (vr.Read()){};
		}
		/// <summary>
		/// Outputs the validation of the XML
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void ValidationHandler(object sender, ValidationEventArgs args)
		{
			Trace.WriteLine(args.ToString(),"NMLSeriliazer.ValidationHandler");
		}

		/// <summary>
		/// Returns a shape on the basis of the unique instantiation key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private Shape GetShape(string key)
		{

			ObjectHandle handle;
			Shape shape;
			for(int k=0; k<site.Libraries.Count;k++)
			{
				for(int m=0; m<site.Libraries[k].ShapeSummaries.Count; m++)
				{
					if(site.Libraries[k].ShapeSummaries[m].Key == key)
					{
						//Type shapeType = Type.GetType(site.Libraries[k].ShapeSummaries[m].ReflectionName);
						
						//object[] args = {this.site};
						Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
						handle = Activator.CreateInstanceFrom(site.Libraries[k].Path,site.Libraries[k].ShapeSummaries[m].ReflectionName);
						shape = handle.Unwrap() as Shape;
						return shape;
						
					}
				}
			}

			return null;

			
		}
		
		/// <summary>
		/// Returns the UID of the entity in string format
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		private static string FormatID(Entity e)
		{
			return String.Format("{0}",e.UID.ToString());
		}

		private static KeyType BuildKeyType(String s)
		{
			KeyType kt = new KeyType();
			kt.ID = s;
			kt.For = KeyForType.All;
			
			return kt;
		}

		


		/// <summary>
		/// Converts the hashtable of GraphML-marked properties to types
		/// </summary>
		/// <param name="properties"></param>
		/// <returns></returns>
		private DataType[] DataTypesFromAttributes(PropertiesHashtable properties)
		{
			DataType[] dts = new DataType[properties.Count];
			for(int k=0; k<properties.Count;k++)
			{
				
				dts[k] = new DataType();
				dts[k].Name = properties.Keys[k];
				object val = null;
				if ((val=properties[k]) != null)
				{
					//the color is a bit different
					if(typeof(Color).IsInstanceOfType(val))
					{
						int num = ((Color) val).ToArgb();
						dts[k].Value.Add(num.ToString());
					}
					else if(typeof(Shape).IsInstanceOfType(val))
					{
						dts[k].Value.Add((val as Shape).UID.ToString());
					}
					else if(typeof(Guid).IsInstanceOfType(val))
					{
						dts[k].Value.Add(((Guid) val ).ToString());
					}
					else if(typeof(CollectionBase).IsInstanceOfType(val))
					{
						CollectionBase col = val as CollectionBase;
						IEnumerator enumer  = col.GetEnumerator();
						while(enumer.MoveNext())
						{
							object obj = enumer.Current;
							PropertiesHashtable props= GraphMLDataAttribute.GetValuesOfTaggedFields(obj);
							DataType[] tps = DataTypesFromAttributes(props);
							DataType dt = new DataType();
							dt.Name=obj.GetType().Name;//the name of the collection element
							dt.IsCollection = true;
							for(int m =0; m<tps.Length; m++)
								dt.Value.Add	(tps[m]);

							dts[k].Value.Add(dt);
							dts[k].IsCollection = true;
							
						}						
					}
					else
						dts[k].Value.Add(val.ToString());
				}
				/*
				 * makes the union of all properties in the different shapes
				if ( !keyList.Contains(properties.Keys[k]))
					keyList.Add(properties.Keys[k],val);
				*/
			
			}
			return dts;
		}

		/// <summary>
		/// Returns qualified type name of o
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		private static string GetTypeQualifiedName(object o)
		{
			if (o==null)
				throw new ArgumentNullException("GetTypeQualifiedName(object) was called with null parameter");
			return GetTypeQualifiedName(o.GetType());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private static string GetTypeQualifiedName(Type t)
		{
			return Assembly.CreateQualifiedName(
				t.Assembly.FullName,
				t.FullName
				);
		}

		private static Type ToType(string text)
		{
			return Type.GetType(text,true);
		}

		private static bool ToBool(string text)
		{
			return bool.Parse(text);
		}
		#endregion
	}
}

