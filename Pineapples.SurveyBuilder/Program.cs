using System;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Layout.Renderer;
using Colors = iText.Kernel.Colors;
using Borders = iText.Layout.Borders;

using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using iText.Forms.Form.Element;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Colors;
using iText.Forms.Xfdf;
using iText.Forms.Fields.Merging;
using iText.Forms.Form;
using iText.Layout.Borders;
using System.Security.Cryptography;
using CommandLine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using System.Dynamic;

namespace surveybuilder
{
	class Program
	{
		private static String DEST = System.IO.Path.Combine(ConfigurationManager.AppSettings["filesPath"], "LayoutFormFields.pdf");

		static void Main(string[] args)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			// Find all types implementing IBuilder
			var builderTypes = assembly.GetTypes()
									   .Where(t => typeof(IBuilder).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

			// Create the dictionary
			Dictionary<string, string> builderDescriptions = new Dictionary<string, string>();

			foreach (var type in builderTypes)
			{
				// Create an instance of the class
				if (Activator.CreateInstance(type) is IBuilder instance)
				{
					builderDescriptions[type.Name] = instance.Description;
				}
			}
			
			// Print the dictionary
			foreach (var kvp in builderDescriptions)
			{
				Console.WriteLine($"Class: {kvp.Key}, Description: {kvp.Value}");
			}

			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(RunWithOptions)
				.WithNotParsed(HandleParseError);
		}

		static void HandleParseError(IEnumerable<Error> errs)
		{
			Console.WriteLine("Error parsing arguments.Press <ENTER> to exit");
			Console.ReadLine();

		}

		static void RunWithOptions(Options opts)
		{
			Console.WriteLine($"Form: {opts.Form}");
			// Add your application logic here.


			PdfStylesheet stylesheet = new PdfStylesheet();

			// Define the base style for headings
			PdfStyle headingbase = new PdfStyle()
			{
				FontBold = true,
				KeepWithNext = true,  // Headings often keep with the next paragraph
				SpacingBefore = 10,   // Add some space before the heading
				SpacingAfter = 5      // Add some space after the heading
			};

			// Add the base style for headings to the stylesheet
			stylesheet.Add("headingbase", headingbase);

			// Define Heading 1 style
			stylesheet.Add("Heading 1", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 24,
				//FontColor = ColorConstants.BLUE
			});

			// Define Heading 2 style
			stylesheet.Add("Heading 2", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 20,
				//FontColor = ColorConstants.RED
			});

			// Define Heading 3 style
			stylesheet.Add("Heading 3", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 16,
				//FontColor = ColorConstants.ORANGE
			});

			// Define Heading 4 style
			stylesheet.Add("Heading 4", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 12,
				//FontColor = ColorConstants.CYAN
			});

			// Define Heading 5 style
			stylesheet.Add("Heading 5", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 12,
				//FontColor = ColorConstants.GREEN
			});

			// Create a new PDF document
			string dest = System.IO.Path.Combine(ConfigurationManager.AppSettings["filesPath"], "kiri2024.pdf");

			// Verbose mode helps for low level debugging
			WriterProperties wprops = new WriterProperties()
				.SetCompressionLevel(opts.Verbose?CompressionConstants.NO_COMPRESSION:CompressionConstants.BEST_COMPRESSION)
				.SetFullCompressionMode(!opts.Verbose);
				

			PdfWriter writer = new PdfWriter(dest, wprops);

			PdfDocument pdfDoc = new PdfDocument(writer);

			// now use form to create the class
			// Create an instance of the class
			IBuilder builder = CreateBuilderInstance(opts.Form);
			builder.Initialise(stylesheet, pdfDoc);
			Document document = builder.Build();

			document.Close();
			Console.WriteLine("Completed");

		}

		static IBuilder CreateBuilderInstance(string className)
		{
			// Get the current assembly
			var assembly = typeof(Program).Assembly;

			string namespacename = typeof(Program).Namespace;
			// Get the full type name
			var type = assembly.GetType($"{namespacename}.{className}", true, true);
			//var type = builderTypes.Where(type => (type.Name == "KEMIS_SEC"))
			//	.First();
			// Create an instance of the type
			return (IBuilder)Activator.CreateInstance(type);
		}

		static void HierarchyTest(PdfDocument pdfDoc)
		{

			var field = PdfBuilders.HP("BRD.D.00.00", pdfDoc);
			field = PdfBuilders.HP("BRD.D.00.01", pdfDoc);
			field = PdfBuilders.HP("BRD.D.01.01", pdfDoc);
			field = PdfBuilders.HP("BRD.D.01.01", pdfDoc);
			// examine this in IEnumerableViewer
			var fields = PdfFormCreator.GetAcroForm(pdfDoc, true).GetAllFormFields();
		}


		public void LostFieldsDemo()
		{
			// Initialize PDF writer and document
			PdfWriter writer = new PdfWriter(new FileStream(System.IO.Path.Combine(ConfigurationManager.AppSettings["filesPath"], "output.pdf"), FileMode.Create, FileAccess.Write));
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
			PdfDocument pdf = new PdfDocument(writer);
			Document document = new Document(pdf);
			PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, true);

			// Add content to the first page
			document.Add(new Paragraph("This is the first page."));
			PdfTextFormField field1 = new TextFormFieldBuilder(pdf, "field1")
					.SetWidgetRectangle(new iText.Kernel.Geom.Rectangle(100, 750, 200, 20))
					.CreateText();
			form.AddField(field1);

			// Create a page break (start a new page)
			pdf.AddNewPage();


			document.Add(new Paragraph("This is the 2 page."));
			PdfTextFormField field2 = new TextFormFieldBuilder(pdf, "field2")
					.SetWidgetRectangle(new iText.Kernel.Geom.Rectangle(100, 750, 200, 20))
					.CreateText();
			form.AddField(field2);
			pdf.AddNewPage();
			pdf.AddNewPage();

			// Close the document
			document.Close();
		}
	}
}