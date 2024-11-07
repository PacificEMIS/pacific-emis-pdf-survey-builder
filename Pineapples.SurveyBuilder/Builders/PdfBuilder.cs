using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using iText.Kernel.Pdf.Action;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Events;
using System.Numerics;
using System.Data.Common;
using System.Xml.Linq;
using iText.Kernel.Pdf.Navigation;


namespace surveybuilder
{

	public class PdfBuilder
	{
		public PdfStylesheet stylesheet;
		public PdfDocument pdfDoc;
		public Dictionary<string, List<KeyValuePair<string, string>>> lookups;


		public Boolean facingPages = false;

		// also add the data factory in here
		public string dataHost;

		// header and footer



		public PdfBuilder(PdfStylesheet stylesheet, PdfDocument pdfDoc)
		{
			this.stylesheet = stylesheet;
			this.pdfDoc = pdfDoc;
		}

		public PdfBuilder(PdfDocument pdfDoc) { this.pdfDoc = pdfDoc; }
		public PdfBuilder(PdfStylesheet stylesheet) { this.stylesheet = stylesheet; }



		private string pageHeaderLeft;
		private string pageHeaderRight;
		public PdfBuilder SetPageHeader(string pageHeader)
		{
			pageHeaderLeft = pageHeaderRight = pageHeader;
			return this;
		}
		public PdfBuilder SetPageHeader(string left, string right)
		{
			pageHeaderLeft = left;
			pageHeaderRight = right;
			return this;
		}
		public PdfBuilder SetPageHeader(string[] headers)
		{
			pageHeaderLeft = headers[0];
			pageHeaderRight = headers[1];
			return this;
		}

		public PdfBuilder SetFacingPages(Boolean facingPages)
		{
			this.facingPages = facingPages;
			BuildPageHandlers();
			return this;
		}
		public Boolean GetFacingPages()
		{
			return this.facingPages;
		}

		private IEventHandler currentPageHandler = null;
		private IEventHandler currentHeaderHandler = null;
		private IEventHandler currentFooterHandler = null;
		// common setup nt to be overriden
		protected IEventHandler BuildPageHandlers()
		{
			// Create the event handlers
			// Example function for dynamic header text
			Func<int, bool, string> dynamicHeaderText = (pageNumber, facing) =>
			{
				return facing && pageNumber % 2 == 0 ? pageHeaderLeft : pageHeaderRight;
			};

			if (currentPageHandler != null)
			{
				pdfDoc.RemoveEventHandler(PdfDocumentEvent.END_PAGE, currentPageHandler);
			}
			if (currentHeaderHandler != null)
			{
				pdfDoc.RemoveEventHandler(PdfDocumentEvent.END_PAGE, currentHeaderHandler);
			}
			if (currentFooterHandler != null)
			{
				pdfDoc.RemoveEventHandler(PdfDocumentEvent.END_PAGE, currentFooterHandler);
			}

			IEventHandler headerHandler = new HeaderEventHandler(dynamicHeaderText, facingPages);
			IEventHandler footerHandler = new FooterEventHandler(facingPages);
			IEventHandler compositeHandler = new CompositeEventHandler(headerHandler, footerHandler);
			currentPageHandler = compositeHandler;
			currentHeaderHandler = headerHandler;
			currentFooterHandler = footerHandler;


			// Add the composite event handler to the PDF document

			pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, compositeHandler);
			return compositeHandler;
		}

		public Document NewPage(Document document)
		{
			pdfDoc.AddNewPage();
			document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
			return document;
		}

		public Document Build()
		{
			Document document = new Document(pdfDoc, PageSize.A4);
			Paragraph p = new Paragraph("Override Build in PDF Builder to generate your PDF");
			document.Add(p);
			return document;
		}

		public void InitLookups()
		{
			lookups = new Dictionary<string, List<KeyValuePair<string, string>>>();
			AddLookups("core");
		}
		public void AddLookups(string lookupCollection)
		{
			string endpoint = $"{dataHost}/api/lookups/collection/{lookupCollection}";

			Console.WriteLine($"Reading lookups from {endpoint}");
			try
			{
				RestApi api = new RestApi();
				var tmp = api.GetFromRestService(endpoint);
				foreach (var kvp in tmp)
				{
					if (!lookups.ContainsKey(kvp.Key))
					{
						lookups.Add(kvp.Key, kvp.Value);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error retrieving XML: {ex.Message}");

			}
		}
		// Utility functions for Bookmark
		public PdfOutline AddOutline(PdfOutline parent, string text, int pageNo = 0)
		{
			if (pageNo == 0)
			{
				pageNo = this.pdfDoc.GetNumberOfPages();
			}
			var po = new PdfString(pageNo.ToString());
			var newOutline = parent.AddOutline(text);
			newOutline.AddDestination(PdfDestination.MakeDestination(po));
			return newOutline;
		}

		// some wrappers around the style sheet

		public Paragraph Heading_1(string text)
		{
			return stylesheet.ApplyStyle("Heading 1", text);
		}
		public Paragraph Heading_2(string text)
		{
			return stylesheet.ApplyStyle("Heading 2", text);
		}
		public Paragraph Heading_2(string text, Document document = null)
		{
			var p = stylesheet.ApplyStyle("Heading 2", text);
			if (document != null)
			{
				document.Add(p);
			}
			return p;
		}
		public Paragraph Heading_3(string text, Document document = null)
		{
			var p = stylesheet.ApplyStyle("Heading 3", text);
			if (document != null)
			{
				document.Add(p);
			}
			return p;
		}

		public Paragraph Heading_4(string text)
		{
			return stylesheet.ApplyStyle("Heading 4", text);
		}

		public Paragraph Heading_5(string text)
		{
			return stylesheet.ApplyStyle("Heading 5", text);
		}



	}

}
