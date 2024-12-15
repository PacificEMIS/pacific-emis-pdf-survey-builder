using iText.Forms.Xfdf;
using iText.Forms;
using iText.Kernel.Pdf;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Forms.Form.Element;
using System.Diagnostics;
using System.Reflection;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Utils.Annotationsflattening;
using iText.Forms.Fields;
using System.CodeDom.Compiler;

namespace surveybuilder.Utilities
{
	public class Toolbox
	{

		string tmpfile;
		Options opts;
		PdfDocument pdfDoc;
		Document document;
		public Toolbox(Options opts) 
		{
			this.opts = opts;
		}

		public void RunTools()
		{
			Console.WriteLine("Running toolbox");
			pdfDoc = OpenDocument();
			document = new Document(pdfDoc);
			if (opts.ClearJs)
			{
				ClearJs();
			}

			if (opts.PushJs)
			{
				PushJs();
			}
			if (opts.OpenActionJs != null)
			{
				SetOpenAction();
			}
			if (opts.CheckActionJs != null)
			{
				SetCheckAction();
			}

			if (opts.Dump)
			{
				Dump();
			}
			if (!String.IsNullOrEmpty(opts.Populate))
			{
				Populate();
			}
			
			document.Close();
			System.IO.File.Delete(opts.Toolbox);
			System.IO.File.Move(tmpfile, opts.Toolbox);
			if (opts.Xfdf)
			{
				Xfdf();
			}
			Console.WriteLine("COMPLETED: Running toolbox");

			if (opts.AutoOpen)
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = opts.Toolbox,
					UseShellExecute = true // Use the OS shell to open the file with its associated app
				});
			}
			else
			{
				Console.ReadKey();
			}
		}

		public PdfDocument OpenDocument()
		{
			tmpfile = @"D:\files\tmp.pdf";

			string retry = "";
			PdfDocument pdfDoc = null;
			do
			{
				try
				{
					retry = "";
					ReaderProperties rprops = new ReaderProperties();
					//.SetPassword(System.Text.Encoding.ASCII.GetBytes("kiri"));
					// Verbose mode helps for low level debugging
					WriterProperties wprops = new WriterProperties()
						.SetCompressionLevel(opts.Verbose ? CompressionConstants.NO_COMPRESSION : CompressionConstants.BEST_COMPRESSION)
						.SetFullCompressionMode(!opts.Verbose);

					PdfReader reader = new PdfReader(new FileStream
							(opts.Toolbox, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite), rprops);
					PdfWriter writer = new PdfWriter(tmpfile, wprops);
					reader.SetCloseStream(true);
					writer.SetCloseStream(true);

					pdfDoc = new PdfDocument(reader, writer);
					pdfDoc.SetCloseWriter(true);
				}
				catch (Exception e)
				{
					Console.WriteLine($"Unable to open {opts.Toolbox} for writing. Press Y to retry...");
					retry = Console.ReadLine();
					if (retry.ToLower() != "y")
					{
						throw e;
					}
				}

			} while (retry.ToLower() == "y");

			return pdfDoc;

		}

		#region javascript
		public void PushJs()
		{
			Console.WriteLine("Pushing Javascript");
			var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);

			//removeall
			foreach (var jsName in javaScriptNameTree.GetKeys())
			{
				Console.WriteLine($"Removing javascript: {jsName}");
				javaScriptNameTree.RemoveEntry(jsName);
			}

			Assembly assembly = Assembly.GetExecutingAssembly();
			IEnumerable<string> jsNames = assembly.GetManifestResourceNames()
							.Where(name => name.EndsWith(".js"));



			foreach (string jsName in jsNames)
			{
				string jscriptText = LoadEmbeddedResource(assembly, jsName);
				PdfDictionary jscript = iText.Kernel.Pdf.Action.PdfAction
					.CreateJavaScript(jscriptText).GetPdfObject();

				string[] pp = jsName.Split('.');
				string name = $"{pp[pp.Length - 2]}.{pp[pp.Length - 1]}";
				Console.WriteLine($"Installing javascript: {name}");
				javaScriptNameTree.AddEntry(name, jscript);
			}

		}
		private string LoadEmbeddedResource(Assembly assembly, string resourceName)
		{
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null) throw new Exception($"Resource {resourceName} not found.");
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public void ClearJs()
		{
			Console.WriteLine("Removing Javascript");
			var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);

			//removeall
			foreach (var jsName in javaScriptNameTree.GetKeys())
			{
				Console.WriteLine($"Removing javascript: {jsName}");
				javaScriptNameTree.RemoveEntry(jsName);
			}
		}

		public void SetCheckAction()
		{
			Console.WriteLine($"Set Check action:");
			Console.WriteLine($"{opts.CheckActionJs}");

			// Create a JavaScript action
			string js = opts.CheckActionJs;
			PdfAction jsAction = PdfAction.CreateJavaScript(js);

			PdfFormField chk = PdfAcroForm.GetAcroForm(pdfDoc, true).GetField("CheckBtn") as PdfButtonFormField;
			chk.SetAdditionalAction(PdfName.U, jsAction);
		}

		public void SetOpenAction()
		{
			Console.WriteLine($"Set open action:");
			Console.WriteLine($"{opts.OpenActionJs}");
			// Create a JavaScript action
			string js = opts.OpenActionJs;
			PdfAction jsAction = PdfAction.CreateJavaScript(js);

			pdfDoc.GetCatalog().SetOpenAction(jsAction);
		}
		#endregion

		#region Pacific Emis end points
		public string Populate()
		{
			return new RestApi().Generate(opts.EmisUrl, opts.Populate, opts.Year).Result;
		}
		#endregion
		#region Auditing and Document examination

		public void Dump()
		{
			Console.WriteLine("Dumping fields to Output window");

			var form = PdfAcroForm.GetAcroForm(pdfDoc, true);

			var all = form.GetAllFormFields();

			foreach (PdfFormField fld in all.Values)
			{
				Debug.WriteLine($"{fld.GetFieldName()}  {(fld.IsRequired() ? "Required" : "")} {(fld.IsReadOnly() ? "ReadOnly" : "")}");
			}
			Console.WriteLine("Press any key to continue");
			Console.ReadKey();

		}
		public void Xfdf()
		{
			Console.WriteLine("writing Xfdf file");
			PdfForm frm = new PdfForm(opts.Toolbox);
			string xfdfName = System.IO.Path.ChangeExtension(opts.Toolbox, "xfdf");
			frm.Xfdf().Save(xfdfName);
			

		}
		#endregion

	}
}
