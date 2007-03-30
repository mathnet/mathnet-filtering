using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
namespace Netron.Diagramming.Core
{
	/// <summary>
	/// Utility class to binary (de)serialize a diagram (from) to file
	/// </summary>
	public static class BinarySerializer
	{
		

		#region Methods






        /// <summary>
        /// Binary saves the diagram
        /// </summary>
        /// <param name="fileName">the file-path</param>
        /// <param name="control">The control.</param>
        /// <returns></returns>
		public static  bool SaveAs(string fileName, DiagramControlBase control)
		{

			FileStream fs = new FileStream(fileName, FileMode.Create);

			GenericFormatter<BinaryFormatter> f = new GenericFormatter<BinaryFormatter>();			
			
			try
			{
				Document document = control.Document;
				

				//Warning!: cleaning up, you need to unhook all events since unserializable classes hooked to events will give problems				
				f.Serialize<Document>(fs, document );
				return true;
			}			
			catch(Exception exc)			
			{
				//site.OutputInfo("The graph was not saved, because some graph events were attached to non-serializable classes.\r\n This is a known issue and will be resolved in a later stadium.",OutputInfoLevels.Exception);
				Trace.WriteLine(exc.Message, "BinarySerializer.SaveAs");
				
				//DumpInfo();
			}
			catch
			{
				Trace.WriteLine("Non-CLS exception caught.","BinarySerializer.SaveAs");
			}
			finally
			{
				fs.Close();
			}
			return false;
		}
        /// <summary>
        /// Opens the binary-saved diagram
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="control">The control.</param>
		public static  void Open (string fileName, DiagramControlBase control)
		{
			FileStream fs=null;
			
			try
			{
				fs= File.OpenRead(fileName);
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
			catch(Exception exc)
			{				
				throw new Exception("Non-CLS exception caught.", exc);
			}
			//donnot open anything if filestream is not there
			if (fs==null) return;
			try
			{

                GenericFormatter<BinaryFormatter> f = new GenericFormatter<BinaryFormatter>();			
                //one-line deserialization, no bits 'nd bytes necessary....C# is amazing...
				Document document =  f.Deserialize<Document>(fs);

                if(document == null)
                    throw new InconsistencyException("The deserialization return 'null'.");
                //call the standard method at the control level to attach a new document
                //In principle you could create a document programmatically and attach it this way as well.
                control.AttachToDocument(document);
			}
			catch(SerializationException exc)			
			{
				MessageBox.Show(exc.Message);
			}
			catch(System.Reflection.TargetInvocationException exc)
			{
				MessageBox.Show(exc.Message, "BinarySerializer.Open");
			}
			catch(Exception exc)
			{
                MessageBox.Show(exc.Message, "BinarySerializer.Open");
			}
			catch
			{
                MessageBox.Show("Non-CLS exception caught.", "BinarySerializer.Open");
				
			}
			finally
			{
				if(fs!=null)
					fs.Close();				
			}
		}

		
		#endregion
	}
}
