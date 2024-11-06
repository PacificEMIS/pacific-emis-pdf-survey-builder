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
			float headerX = facingPages && pageNumber % 2 == 1 ? pageSize.GetRight() - 50 : pageSize.GetLeft() + 50;
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
			Rectangle pageSize = page.GetPageSize();

			// Determine footer position
			float footerX = facingPages && pageNumber % 2 == 0 ? pageSize.GetLeft() + 50 : pageSize.GetRight() - 50;
			float footerY = pageSize.GetBottom() + 20;
			TextAlignment alignment = facingPages && pageNumber % 2 == 0 ? TextAlignment.LEFT : TextAlignment.RIGHT;

			// Add footer
			Canvas canvas = new Canvas(page, pageSize);
			canvas.ShowTextAligned(new Paragraph("Page " + pageNumber),
								   footerX, footerY, alignment);
			canvas.Close();
		}
	}


}
