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
using System.Web.UI.WebControls.Expressions;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace surveybuilder
{
	public abstract class PdfBuilder : IBuilder
	{
		public PdfStylesheet stylesheet;
		public PdfDocument pdfDoc;
		//public Dictionary<string, List<KeyValuePair<string, string>>> lookups;
		public LookupManager lookups;
		public Options options;

		public Boolean facingPages = false;

		// IBuilder interface
		public virtual string Description
		{
			get
			{
				return "Provide a description for this builder by overriding IBuilder:Description";
			}
		}

		public virtual void Initialise(Options options, PdfDocument pdfDoc)
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

			this.stylesheet = stylesheet;
			this.options = options;
			this.pdfDoc = pdfDoc;
		}


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
		public Document NewPage(Document document, PageSize size)
		{
			pdfDoc.AddNewPage(size);
			document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
			return document;
		}
		public virtual Document Build()
		{
			Document document = new Document(pdfDoc, PageSize.A4);
			Paragraph p = new Paragraph("Override Build in PDF Builder to generate your PDF");
			document.Add(p);
			return document;
		}

		/// <summary>
		/// common cleanup routine for any form
		/// </summary>
		public virtual void Finalise()
		{
			// logic to clear up any duplicates found in Kids
			IDictionary<string, PdfFormField> flds = PdfAcroForm.GetAcroForm(pdfDoc, true).GetAllFormFields();

			var buttons = flds
				.Where(fx => (fx.Value is PdfButtonFormField && fx.Value.GetKids() != null))
				.ToDictionary(fx => fx.Key, fx => fx.Value);

			Console.WriteLine($"Final touches...");
			foreach (var btn in buttons.Values)
			{
				PdfButtonFormField rgrp = btn as PdfButtonFormField;
				PdfArray kidsArray = rgrp.GetKids();
				HashSet<PdfObject> uniqueKids = new HashSet<PdfObject>(kidsArray);
				kidsArray.Clear();
				foreach (var kid in uniqueKids)
				{
					kidsArray.Add(kid);
				}
			}


		}

		#region Bookmarks and Outline
		// Utility functions for Bookmark
		PdfDestination CurrentDestination(Document document)
		{
			// this bounding box is the remaining area on the current page
			var bbox = document.GetRenderer().GetCurrentArea().GetBBox();
			// y is at the botttom of the page
			//and height is ThenBy remain part of the page not yet used
			float y = bbox.GetY() + bbox.GetHeight();

			// Create a destination at this point
			return PdfExplicitDestination.CreateXYZ(
				pdfDoc.GetLastPage(), // Current page
				0,                 // X coordinate (left edge)
				y,                 // Y coordinate (top of the table)
				1                  // Zoom level
			);

		}
		public PdfOutline AddOutline(Document document, PdfOutline parent, string text)
		{
			var newOutline = parent.AddOutline(text);
			newOutline.AddDestination(CurrentDestination(document));
			return newOutline;
		}

		#endregion
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


		#region Javascript

		public virtual void LoadJs()
		{
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
		#endregion
	}
}
