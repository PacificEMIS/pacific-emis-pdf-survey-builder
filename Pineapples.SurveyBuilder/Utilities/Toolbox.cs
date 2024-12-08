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

namespace surveybuilder.Utilities
{
	public class Toolbox
	{
		
		string tmpfile;
		Options opts;
		PdfDocument pdfDoc;
		Document document;
		public Toolbox() { }

		public void RunTools(Options opts) {
			Console.WriteLine("Running toolbox");
			this.opts = opts;
			pdfDoc = OpenDocument();
			document = new Document(pdfDoc);
			if (opts.PushJs)
			{
				PushJs();
			}
			document.Close();
			System.IO.File.Delete(opts.Toolbox);
			System.IO.File.Move(tmpfile, opts.Toolbox);
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

			PdfDocument pdfDoc = new PdfDocument(reader, writer);
			pdfDoc.SetCloseWriter(true);
			return pdfDoc;
		}

		public void PushJs()
		{
			Console.WriteLine("Pushing Javascript");
		
			Assembly assembly = Assembly.GetExecutingAssembly();
			IEnumerable<string> jsNames = assembly.GetManifestResourceNames()
							.Where(name => name.EndsWith(".js"));


			var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);


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

	}
}
