using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Forms.Fields;
using iText.Forms;
using System.Drawing.Printing;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Action;
using System.Diagnostics;
using iText.Kernel.Pdf.Annot;



namespace surveybuilder
{

	public class CompositeEventHandler : IEventHandler
	{
		private readonly IEventHandler[] eventHandlers;

		public CompositeEventHandler(params IEventHandler[] handlers)
		{
			this.eventHandlers = handlers;
		}

		public void HandleEvent(Event @event)
		{
			foreach (var handler in eventHandlers)
			{
				handler.HandleEvent(@event);
			}
		}
	}

	public class HeaderEventHandler : IEventHandler
	{
		private string headerText;
		private Func<int, bool, string> headerTextFunc;
		private bool facingPages;

		// Constructor with static header text
		public HeaderEventHandler(string headerText, bool facingPages)
		{
			this.headerText = headerText;
			this.facingPages = facingPages;
			this.headerTextFunc = null;
		}

		// Constructor with dynamic header text function
		public HeaderEventHandler(Func<int, bool, string> headerTextFunc, bool facingPages)
		{
			this.headerTextFunc = headerTextFunc;
			this.facingPages = facingPages;
			this.headerText = null;
		}

		public void HandleEvent(Event @event)
		{
			return;
			PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
			PdfDocument pdfDoc = docEvent.GetDocument();
			PdfPage page = docEvent.GetPage();
			int pageNumber = pdfDoc.GetPageNumber(page);
			Rectangle pageSize = page.GetPageSize();

			// Determine the header text
			string text = headerTextFunc != null ? headerTextFunc(pageNumber, facingPages) : headerText;

			// Determine header position - right hand pages are odd numbers
			float headerX = facingPages && pageNumber % 2 == 1 ? pageSize.GetRight() - 75 : pageSize.GetLeft() + 75;
			float headerY = pageSize.GetTop() - 20;
			TextAlignment alignment = facingPages && pageNumber % 2 == 1 ? TextAlignment.RIGHT : TextAlignment.LEFT;

			// Add header
			Canvas canvas = new Canvas(page, pageSize);
			canvas.ShowTextAligned(new Paragraph(text),
								   headerX, headerY, alignment);
			//canvas.Close();
		}
	}



	public class FooterEventHandler : IEventHandler
	{
		private bool facingPages;

		public FooterEventHandler(bool facingPages)
		{
			this.facingPages = facingPages;
		}

		public void HandleEvent(Event @event)
		{
			PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
			PdfDocument pdfDoc = docEvent.GetDocument();
			PdfPage page = docEvent.GetPage();
			int pageNumber = pdfDoc.GetPageNumber(page);

			// Skip footer for the cover page (assumed to be page 1)
			if (pageNumber == 1) return;
			// Offset the page number by subtracting 1 for display (page numbering starts after cover page)
			int displayedPageNumber = pageNumber - 1;

			Rectangle pageSize = page.GetPageSize();

			// Determine footer position
			float footerX = facingPages && pageNumber % 2 == 0 ? pageSize.GetRight() - 75 : pageSize.GetLeft() + 75;
			float footerY = pageSize.GetBottom() + 20;
			TextAlignment alignment = facingPages && pageNumber % 2 == 0 ? TextAlignment.LEFT : TextAlignment.RIGHT;

			// Add footer
			Canvas canvas = new Canvas(page, pageSize);
			canvas.ShowTextAligned(new Paragraph("Page " + displayedPageNumber),
								   footerX, footerY, alignment);
			canvas.Close();
		}
	}

	public class FieldFooterEventHandler : IEventHandler
	{

		private bool facingPages;

		public FieldFooterEventHandler(bool facingPages)
		{
			this.facingPages = facingPages;
		}
		public void HandleEvent(Event @event)
		{
			// Get the PDF document and current page from the event
			PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
			PdfDocument pdfDoc = docEvent.GetDocument();
			PdfPage page = docEvent.GetPage();
			int pageNumber = pdfDoc.GetPageNumber(page);
			Rectangle pageSize = page.GetPageSize();

			pdfDoc.VerboseDebug($"Field Page Handler on page {pageNumber}");

			// Skip footer for the cover page (assumed to be page 1)
			if (pageNumber == 1) return;

			// Offset the page number by subtracting 1 for display (page numbering starts after cover page)
			int displayedPageNumber = pageNumber - 1;

			// if facingPages, put the field inside and the page numer outside
			// otherwise put the field centre, and page number right
			Rectangle footerFieldRect = new Rectangle(50, 30, 500, 20); // (x, y, width, height)
			TextAlignment fieldalignment = TextAlignment.CENTER;
			if (facingPages)
			{
				var x = pageNumber % 2 == 0 ? pageSize.GetLeft() + 10: (pageSize.GetRight()-10 -300);
				footerFieldRect = new Rectangle(x, 20, 300, 20); // (x, y, width, height)
				fieldalignment = pageNumber % 2 == 0 ? TextAlignment.LEFT : TextAlignment.RIGHT;
			}
			else
			{
				var x = (pageSize.GetLeft() + pageSize.GetRight() )/ 2 - 150;
				footerFieldRect = new Rectangle(x, 20, 300, 20); // (x, y, width, height)
			}
			
			float footerX = facingPages && pageNumber % 2 == 0 ? pageSize.GetRight() -10: pageSize.GetLeft() + 10;
			float footerY = pageSize.GetBottom() + 20;
			TextAlignment alignment = facingPages && pageNumber % 2 == 0 ? TextAlignment.RIGHT : TextAlignment.LEFT;

			// Add footer
			Canvas canvas = new Canvas(page, pageSize);
			canvas
				.SetFontSize(10)
				.ShowTextAligned(new Paragraph("Page " + displayedPageNumber),
								   footerX, footerY, alignment);
			canvas.Close();

			var form = PdfAcroForm.GetAcroForm(pdfDoc, true);

			PdfTextFormField newField = new TextFormFieldBuilder(pdfDoc, $"Footer.{pageNumber:00}")
				.SetWidgetRectangle(footerFieldRect)
				.SetPage(pageNumber)
				.CreateText();
		
			newField.SetFieldFlag(PdfFormField.FF_READ_ONLY, true);
			newField.SetValue(pdfDoc.GetDocumentInfo().GetTitle());
			newField.SetJustification(fieldalignment);

			

			var w = newField.GetFirstFormAnnotation();
			//w.SetBackgroundColor(NamedColors.BurlyWood);

			// Add the field to the AcroForm
			form.AddField(newField,page);

			pdfDoc.VerboseDebug($"Field count @ page {pageNumber}: {form.GetAllFormFields().Count()}");
			//bool exists = form.GetAllFormFields().TryGetValue("Survey.SchoolNo", out ff);
			bool exists = (form.GetField("Survey.SchoolNo") !=null);
			pdfDoc.VerboseDebug($"Field count @ page {pageNumber}: Survey.SchoolNo Exists?: {exists.ToString()}");

		}
	}
}
