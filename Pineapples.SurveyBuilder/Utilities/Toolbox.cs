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
using System.Xml.Linq;

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
			if (!String.IsNullOrEmpty(opts.Toolbox) &&
				(opts.ClearJs || opts.PushJs || (opts.LoadXfdf != null) || (opts.OpenActionJs != null))
			)				
			{
				pdfDoc = OpenDocument();
				document = new Document(pdfDoc);
			}
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
				Console.WriteLine($"Generating survey for {opts.Populate} year {opts.Year}");
				opts.Toolbox = Populate();
			}

			if (!String.IsNullOrEmpty(opts.LoadXfdf))
			{
				LoadXfdf(opts.LoadXfdf);
			}
			if (document != null)
			{
				document.Close();
				System.IO.File.Delete(opts.Toolbox);
				System.IO.File.Move(tmpfile, opts.Toolbox);
			}
			if (opts.Xfdf)
			{
				Xfdf(opts.Toolbox);
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
			else if (opts.Wait)
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
			//in toolbox mode, we only remove those javascripts that come from Embedded resources
			// these have the .js extension
			foreach (var jsName in javaScriptNameTree.GetKeys()
				.Where(name => name.ToString().EndsWith(".js"))
			)
			{
				Console.WriteLine($"Removing javascript: {jsName}");
				javaScriptNameTree.RemoveEntry(jsName);
			}

			Assembly assembly = Assembly.GetExecutingAssembly();
			IEnumerable<string> jsNames = assembly.GetManifestResourceNames()
							.Where(name => name.EndsWith(".js"));

			//load JS_Init.js first
			foreach (string jsName in jsNames)
			{
				string[] pp = jsName.Split('.');
				string name = $"{pp[pp.Length - 2]}.{pp[pp.Length - 1]}";
				string jscriptText = LoadEmbeddedResource(assembly, jsName);
				PdfDictionary jscript = iText.Kernel.Pdf.Action.PdfAction
					.CreateJavaScript(jscriptText).GetPdfObject();
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

			//removeall - preserve those that are 'dynamically' generated
			foreach (var jsName in javaScriptNameTree.GetKeys()
				.Where(jsname => jsname.ToString().EndsWith(".js")))
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
		public void Xfdf(string dest)
		{
			Console.WriteLine("writing Xfdf file");
			PdfForm frm = new PdfForm(dest);
			string xfdfName = System.IO.Path.ChangeExtension(dest, "xfdf");
			frm.Xfdf().Save(xfdfName);
			Console.WriteLine($"Xfdf saved to {xfdfName}");


		}

		public void LoadXfdf(string filename)
		{
			
			XfdfObject xfdfData;
			XDocument xdoc= null;
			// push the xfdf into the document


			if (System.IO.Path.GetExtension(filename) == "xfdf" ||
				System.IO.Path.GetExtension(filename) == "xml")
			{
				Console.WriteLine($"loading Xfdf file {filename}");
				try
				{
					xdoc = XDocument.Load(filename);
					
				}

				catch (Exception ex)
				{
					Console.WriteLine($"ERROR Loading Xfdf file: {ex}");
				}
			}
			else
			{
				// assume its a Pdf - use the custom routine to get Xfdf and deal
				// with both herarchical and flat field names structures
				Console.WriteLine($"loading Xfdf from pdf file {filename}");
				xdoc = new PdfForm(filename).Xfdf();
			}
			MemoryStream mem = new MemoryStream();
			xdoc.Save(mem);
			mem.Position= 0;
			xfdfData = new XfdfObjectFactory().CreateXfdfObject(mem);
			// push the xfdf into the document
			xfdfData.MergeToPdf(pdfDoc, opts.Toolbox);
			PdfForm.StandardizeCheckboxes(pdfDoc);


		}
		#endregion

		#region Scripting
		public void Script()
		{
			// must have year, emisurl, and access to the lookup listof schools
			if (opts.Year == 0)
			{
				Console.WriteLine($"Year must be supplied when scripting");
				return;	
			}
			if (String.IsNullOrEmpty(opts.EmisUrl))
			{
				Console.WriteLine($"EmisUrl must be supplied when scripting");
				return;
			}

			string app = Assembly.GetExecutingAssembly().Location;
			LookupManager lm = new LookupManager(null, opts.EmisUrl);
			LookupList schools = lm["schoolNames"]
				.OrderBy(entry => entry.C).ToLookupList();
			
			// open a text file for output
			string script = System.IO.Path.Combine(opts.OutputPath, opts.Script);
			// change extension to .bat
			script = System.IO.Path.ChangeExtension(script, "bat");
			FileStream fs = new FileStream(script, FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs);
			sw.WriteLine($"SET EmisUrl={opts.EmisUrl}");
			sw.WriteLine($"SET AppPath=\"{app}\"");

			foreach (LookupEntry school in schools)
			{
				sw.WriteLine($"REM {school.C} {school.N}");
				sw.WriteLine($"\"%appPath%\" -u %EmisUrl% -y {opts.Year} --toolbox \" \" --populate {school.C}");
			}
			
			sw.Close();
		}
		#endregion
	}
	
}
