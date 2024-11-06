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

namespace surveybuilder
{
	class Program
	{		
		private static String DEST = System.IO.Path.Combine(ConfigurationManager.AppSettings["filesPath"], "LayoutFormFields.pdf");

		static void Main()
		{


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
				FontColor = ColorConstants.BLUE
			});

			// Define Heading 2 style
			stylesheet.Add("Heading 2", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 20,
				FontColor = ColorConstants.RED
			});

			// Define Heading 3 style
			stylesheet.Add("Heading 3", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 18,
				FontColor = ColorConstants.ORANGE
			});

			// Define Heading 4 style
			stylesheet.Add("Heading 4", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 16,
				FontColor = ColorConstants.CYAN
			});

			// Define Heading 5 style
			stylesheet.Add("Heading 5", new PdfStyle(stylesheet["headingbase"])
			{
				FontSize = 14,

				FontColor = ColorConstants.GREEN
			});

			// Create a new PDF document
			string dest = System.IO.Path.Combine(ConfigurationManager.AppSettings["filesPath"], "kiri2024.pdf");

			PdfWriter writer = new PdfWriter(dest);
			// helps for *very* low level debugging
			writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);

			PdfDocument pdfDoc = new PdfDocument(writer);

			//Document document = new Document(pdfDoc);

			Document document = new KEMIS_PRI_Builder(stylesheet, pdfDoc).Build();

			document.Close();
			Console.WriteLine("Completed");

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