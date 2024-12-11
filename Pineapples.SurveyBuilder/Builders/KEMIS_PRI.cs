using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Font;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Colors;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;

namespace surveybuilder
{
	public class KEMIS_PRI : PdfBuilder
	{
		public KEMIS_PRI()
		{
		}

		public override string Description
		{
			get
			{
				return "Kiribati Primary Survey for 2025 and after";
			}
		}

		public override Document Build()
		{
			lookups = new LookupManager(pdfDoc, options.EmisUrl);
			lookups.AddLookups("student");
			lookups.AddLookups("censuspdf");
			// add customised lookup lists for age range and class levels
			AddCustomLookupLists();

			CellMakers.SetLookups(lookups);    // make all the lookup strutures availabe to cell makers

			Document document = new Document(pdfDoc, PageSize.A4);
			SetFacingPages(true);
			SetPageHeader("left page", "right page");
			// add the javascript libraries
			LoadJs();

			// prepare the outline tree for the bookmarks pane
			PdfOutline rootoutline = pdfDoc.GetOutlines(false);
			PdfOutline parentOutline = AddOutline(document,rootoutline, "KEMIS Survey");

			// Set the title info
			pdfDoc.GetDocumentInfo().SetTitle("KEMIS Primary School Survey");
			pdfDoc.GetDocumentInfo().SetAuthor("Pacific EMIS Survey Builder");
			/**************************************************************************/
			/* Begin Pdf output															  */
			/**************************************************************************/

			CoverPage(document, "Primary", SurveyYear);
			NewPage(document);

			#region ***** School Information *****
			var schoolInfoOutline = this.AddOutline(document,parentOutline, "School Information");

			document.Add(Heading_1("School Information"));
			document = new SchoolInfo()
				.Build(this, document, schoolInfoOutline);

			NewPage(document);
			#endregion

			#region ***** Enrolment Grids *****

			// Prepare grid makers common to several sections in the document
			GenderedGridmaker grd = new GenderedGridmaker();
			grd.Rows = lookups["ages"];
			grd.Columns = lookups["classLevels"];

			var enrolOutline = this.AddOutline(document, parentOutline, "Enrolment Details");
			document.Add(Heading_1("Enrolment Details"));

			AddOutline(document, enrolOutline, "Enrolment");
			document.Add(Heading_2("Enrolment of Pupils by Age, Class Level and Gender"));
			
			document.Add(new Paragraph(@"Record the number of pupils enrolled at your school this year according to their age, class level and gender. "
+ @"Age is at 31 March this year."));

			grd.Tag = "Enrol";
			document.Add(grd.Make(this));
			NewPage(document);

			AddOutline(document, enrolOutline, "Repeaters");
			document.Add(Heading_2("Repeaters"));

			document.Add(new Paragraph(@"For each class, record the number of pupils who were enrolled in the same class in the previous school year. "
			+ @"Record the repeating students by their age as at 31 March this year."));

			grd.Tag = "Rep";
			document.Add(grd.Make(this));
			NewPage(document);

			AddOutline(document,enrolOutline, "Distance from School");
			document.Add(Heading_2("Distance from School"));
			document = new DistanceFromSchool()
				.Build(this, document, lookups["distanceCodes"]);
			NewPage(document);

			AddOutline(document, enrolOutline, "Disabilities");
			document.Add(Heading_2("Children with Disabilities Attending School"));
			document = new Disabilities()
				.Build(this, document, grd, lookups["disabilities"]);
			NewPage(document);

			AddOutline(document, enrolOutline, "Transfers In");
			document.Add(Heading_2("Transfers In"));
			document = new TransfersIn()
				.Build(this, document, grd, lookups["islands"]);
			NewPage(document);


			AddOutline(document, enrolOutline, "Pre-School Attendance");
			document.Add(Heading_2("Pupils Who Have Attended Pre-School"));
			document = new PSA()
				.Build(this, document);
			NewPage(document);

			AddOutline(document, enrolOutline, "Single Teacher Classes");
			document.Add(Heading_2("Single Teacher Classes"));
			document = new SingleTeacherClasses().Build(this, document);
			NewPage(document);

			AddOutline(document, enrolOutline, "Joint Teacher Classes");
			document.Add(Heading_2("Joint Teacher Classes"));
			document = new JointTeacherClasses().Build(this, document);
			NewPage(document);

			#endregion

			#region *********** Staff *************
			var staffOutline = this.AddOutline(document, parentOutline, "School Staff Information");
			document.Add(Heading_1("School Staff Information"));

			this.AddOutline(document, staffOutline, "Expected Staff List");
			document.Add(Heading_2("Expected Staff List"));
			document = new ExpectedStaff()
				.Build(this, document);
			NewPage(document);

			AddOutline(document, staffOutline, "New Staff");
			document.Add(Heading_2("New Staff"));
			document = new NewStaff()
				.Build(this, document);
			NewPage(document);
			#endregion

			#region ************** Buildings and Rooms *************************
			var infrastructureOutline = this.AddOutline(document, parentOutline, "Buildings and Rooms");
			document.Add(Heading_1("Buildings and Rooms"));

			AddOutline(document, infrastructureOutline, "Classrooms");
			document.Add(Heading_2("Classrooms"));
			document = new Classrooms()
				.Build(this, document);
			NewPage(document);

			AddOutline(document, infrastructureOutline, "School Site");
			document.Add(Heading_2("School Site"));
			document = new SchoolSite()
				.Build(this, document);
			NewPage(document);

			AddOutline(document, infrastructureOutline, "Teacher Housing");
			document.Add(Heading_2("Teacher Housing"));
			document = new TeacherHousing()
				.Build(this, document, lookups["metaResourceDefinitions"]);
			NewPage(document);

			#endregion

			#region *************** Wash ***************************
			var washOutline = this.AddOutline(document, parentOutline, "Water, Sanitation and Hygiene");
			document.Add(Heading_1("Water, Sanitation and Hygiene"));

			AddOutline(document, washOutline, "Water");
			document.Add(Heading_2("Water"));
			document = new WASHWater()
				.Build(this, document, lookups["waterSupplyTypes"]);
			NewPage(document);

			AddOutline(document, washOutline, "Sanitation");
			document.Add(Heading_2("Sanitation"));
			document = new WASHSanitation()
				.Build(this, document, lookups["toiletTypes"]);
			NewPage(document);

			AddOutline(document, washOutline, "Hygiene");
			document.Add(Heading_2("Hygiene"));
			document = new WASHHygiene()
				.Build(this, document);
			NewPage(document);
			#endregion

			#region ************ Resources ***************
			var resourcesOutline = this.AddOutline(document, parentOutline, "School Resources");
			document.Add(Heading_1("School Resources"));

			AddOutline(document, resourcesOutline, "School Resources");
			document.Add(Heading_2("School Resources"));
			document = new SchoolResources()
				.Build(this, document, lookups["metaResourceDefinitions"]);
			NewPage(document);

			AddOutline(document, resourcesOutline, "School Supplies");
			document.Add(Heading_2("School Supplies"));
			document = new SchoolSupplies()
				.Build(this, document);
			NewPage(document);
			#endregion

			#region ************ Comments ***************
			var generalOutline = this.AddOutline(document, parentOutline, "General Comments");
			document.Add(Heading_1("General Comments"));			

			document = new GeneralComments()
				.Build(this, document, generalOutline);
			NewPage(document);
			#endregion

			Finalise();
			return document;
		}

		public virtual void CoverPage(Document document, string levelName, int surveyYear)
		{
			// Cover page
			string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\Images", "report-cover.png");
			Image coverImage = new Image(ImageDataFactory.Create(imagePath)); ;
			coverImage.ScaleToFit(PageSize.A4.GetWidth(), PageSize.A4.GetHeight());
			coverImage.SetFixedPosition(0, 0);
			document.Add(coverImage);

			// Dynamic details on cover page
			// Define your text and its exact positions on the page
			PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());

			// Example of adding small text at specific coordinates
			// Load the custom font
			string fontPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\Fonts", "MYRIADPRO-REGULAR.OTF");
			string fontPath2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\Fonts", "MYRIADPRO-BOLD.OTF");
			PdfFont customFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
			PdfFont customFont2 = PdfFontFactory.CreateFont(fontPath2, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

			var fieldPosition = new iText.Kernel.Geom.Rectangle(50, 146, 80, 30);
			PdfTextFormField textField = new TextFormFieldBuilder(pdfDoc, "Survey.SurveyYear")
				.SetWidgetRectangle(fieldPosition)
				.CreateText();

			textField.SetValue(surveyYear.ToString())
				.SetFontAndSize(customFont2, 32);


			textField.SetFieldFlag(PdfFormField.FF_READ_ONLY, true);

			GetPdfAcroForm().AddField(textField);

			canvas.BeginText()
				  .SetFontAndSize(customFont2, 32)
				  .MoveText(135, 150) // (x, y) position for the text
				  .ShowText($"{levelName}")
				  .EndText();                // do this to reset the co-ordinates

			// right align the version
			string version = "07112024";
			int fontSize = 12;

			// Define the right margin (or use page width)
			float rightMargin = 547; // E.g., 550 units from the left of the page

			// Measure the width of the text
			float textWidth = customFont.GetWidth(version, fontSize);

			// Calculate the starting X-coordinate for right alignment
			float startX = rightMargin - textWidth;
			canvas.BeginText()
				  .SetFontAndSize(customFont, 12)
				  .MoveText(startX, 60) // this is now the absolute position on the page
				  .ShowText(version) // Version
				  .EndText();

		}

		public virtual void AddCustomLookupLists()
		{

			// Moved to KeyValuePair to a custom LookupEntry class which can also hold metadata.
			LookupList ages = new string[] { "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" }
							.Select(n => new LookupEntry
							{
								C = n, // Set the primary code (C)
								N = n  // Set the primary name (N)
							})
							.ToLookupList();
			lookups.Add("ages", ages);


			var classLevels = new LookupList()
			{

			new LookupEntry { C = "P1", N = "Class 1" },
			new LookupEntry { C = "P2", N = "Class 2" },
			new LookupEntry { C = "P3", N = "Class 3" },
			new LookupEntry { C = "P4", N = "Class 4" },
			new LookupEntry { C = "P5", N = "Class 5" },
			new LookupEntry { C = "P6", N = "Class 6" }
			};
			lookups.Add("classLevels", classLevels);
		}

	}


}
