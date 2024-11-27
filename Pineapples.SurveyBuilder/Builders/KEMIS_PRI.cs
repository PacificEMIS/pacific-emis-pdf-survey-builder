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

namespace surveybuilder
{
	public class KEMIS_PRI_Builder : PdfBuilder
	{
		public KEMIS_PRI_Builder()
		{
		}

		public override void Initialise(PdfStylesheet stylesheet, PdfDocument pdfDoc)
		{
			base.Initialise(stylesheet, pdfDoc);
		}
		public override Document Build()
		{
			dataHost = ConfigurationManager.AppSettings["emisUrl"]; // dataHost = $"https://kemis-test.pacific-emis.org";
			
			lookups = new LookupManager(pdfDoc, dataHost);
			lookups.AddLookups("student");
			lookups.AddLookups("censuspdf");
			CellMakers.SetLookups(lookups);    // make all the lookup strutures availabe to cell makers

			Document document = new Document(pdfDoc, PageSize.A4);
			SetFacingPages(true);
			SetPageHeader("left page", "right page");

			// Cover page
			string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\Images", "report-cover.png");
			Image coverImage = new Image(ImageDataFactory.Create(imagePath)); ;
			coverImage.ScaleToFit(PageSize.A4.GetWidth(), PageSize.A4.GetHeight());
			coverImage.SetFixedPosition(0, 0);
			document.Add(coverImage);
			NewPage(document);

			// Dynamic details on cover page
			// Define your text and its exact positions on the page
			PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage());

			// Example of adding small text at specific coordinates
			// Load the custom font
			string fontPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\Fonts", "MYRIADPRO-REGULAR.OTF");
			string fontPath2 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\Fonts", "MYRIADPRO-BOLD.OTF");
			PdfFont customFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
			PdfFont customFont2 = PdfFontFactory.CreateFont(fontPath2, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
			canvas.BeginText()
				  .SetFontAndSize(customFont2, 32)
				  .MoveText(50, 150) // (x, y) position for the text
				  .ShowText("2024 (Primary)")
				  .SetFontAndSize(customFont, 12)
				  .MoveText(448, -90) // Adjust for the next line of text if needed
				  .ShowText("07112024") // Version
				  .EndText();

			//IList<PdfOutline> children = outlines.GetAllChildren();
			//outlines.AddOutline()
			//if (children.Count == 0)
			//{
			//	PdfOutline ol = new PdfOutline("KEMIS Survey",null,pdfDoc);
			//}
			//		pdfDoc.Add

			// Prepare grid makers common to several sections in the document
			GenderedGridmaker grd = new GenderedGridmaker();

			// Moved to KeyValuePair to a custom LookupEntry class which can also hold metadata.
			var rows = new string[] { "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" }
							.Select(n => new LookupEntry
							{
								C = n, // Set the primary code (C)
								N = n  // Set the primary name (N)
							})
							.ToList();


			var classLevels = new List<LookupEntry>();

			classLevels.Add(new LookupEntry { C = "P1", N = "Class 1" });
			classLevels.Add(new LookupEntry { C = "P2", N = "Class 2" });
			classLevels.Add(new LookupEntry { C = "P3", N = "Class 3" });
			classLevels.Add(new LookupEntry { C = "P4", N = "Class 4" });
			classLevels.Add(new LookupEntry { C = "P5", N = "Class 5" });
			classLevels.Add(new LookupEntry { C = "P6", N = "Class 6" });

			grd.Rows = rows;
			grd.Columns = classLevels;

			// after setting all the fields, add the javascript libraries
			var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);

			string jsPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["pineapplesPath"], @"Pineapples.Client\assets\pdfSurvey\js");
			foreach (string jsfile in System.IO.Directory.GetFiles(jsPath))
			{
				string jscriptText = System.IO.File.ReadAllText(jsfile);
				iText.Kernel.Pdf.PdfDictionary jscript = iText.Kernel.Pdf.Action.PdfAction
					.CreateJavaScript(jscriptText).GetPdfObject();
				javaScriptNameTree.AddEntry(System.IO.Path.GetFileName(jsfile), jscript);
			}

			PdfOutline rootoutline = pdfDoc.GetOutlines(false);
			var parentOutline = rootoutline.AddOutline("KEMIS Survey");

			var schoolInfoOutline = this.AddOutline(parentOutline, "School Information");
			AddOutline(schoolInfoOutline, "General Information");
			AddOutline(schoolInfoOutline, "Parent's Committee");
			AddOutline(schoolInfoOutline, "Community Support");
			AddOutline(schoolInfoOutline, "Overall School Structure");

			document.Add(Heading_1("School Information"));

			document = new SchoolInfo()
				.Build(this, document);

			NewPage(document);

			var enrolOutline = this.AddOutline(parentOutline, "Enrolment Details");
			document.Add(Heading_1("Enrolment Details"));

			/* in production we could do this, with access to the DbContext:
			var classLevels = Factory.DbContext.Levels
				.Where(lvl => lvl.lvlYear >= 1 && lvl.lvlYear <= 6)
				.OrderBy(lvl => p.lvlYear)
				.Select( lvl => new KeyValuePair<string,string>(lvl.Code, lvl.Description)
				.ToList();

			*/

			AddOutline(enrolOutline, "Enrolment");
			document.Add(Heading_2("Enrolment of Pupils by Age, Class Level and Gender"));
			document.Add(new Paragraph(@"Record the number of pupils enrolled at your school this year according to their age, class level and gender. "
			+ @"Age is at 31 March this year."));

			grd.Tag = "Enrol";

			document.Add(grd.Make(this));

			NewPage(document);

			AddOutline(enrolOutline, "Repeaters");
			document.Add(Heading_2("Repeaters"));
			document.Add(new Paragraph(@"For each class, record the number of pupils who were enrolled in the same class in the previous school year. "
			+ @"Record the repeating students by their age as at 31 March this year."));

			grd.Tag = "Rep";

			document.Add(grd.Make(this));

			NewPage(document);

			AddOutline(enrolOutline, "Distance from School");
			document.Add(Heading_2("Distance from School"));
			document = new DistanceFromSchool()
				.Build(this, document, lookups["distanceCodes"]);
			NewPage(document);

			AddOutline(enrolOutline, "Disabilities");
			document.Add(Heading_2("Children with Disabilities Attending School"));
			document = new Disabilities()
				.Build(this, document, grd, lookups["disabilities"]);
			NewPage(document);

			AddOutline(enrolOutline, "Transfers In");
			document.Add(Heading_2("Transfers In"));
			document = new TransfersIn()
				.Build(this, document, grd, lookups["islands"]);
			NewPage(document);


			AddOutline(enrolOutline, "Pre-School Attendance");
			document.Add(Heading_2("Pupils Who Have Attended Pre-School"));
			document = new PSA()
				.Build(this, document);
			NewPage(document);

			AddOutline(enrolOutline, "Single Teacher Classes");
			document.Add(Heading_2("Single Teacher Classes"));
			document = new SingleTeacherClasses().Build(this, document);
			NewPage(document);

			AddOutline(enrolOutline, "Joint Teacher Classes");
			document.Add(Heading_2("Joint Teacher Classes"));
			document = new JointTeacherClasses().Build(this, document);
			NewPage(document);

			var staffOutline = this.AddOutline(parentOutline, "School Staff Information");
			document.Add(Heading_1("School Staff Information"));

			AddOutline(staffOutline, "Expected Staff List");
			document.Add(Heading_2("Expected Staff List"));
			document = new ExpectedStaff()
				.Build(this, document);
			NewPage(document);

			AddOutline(staffOutline, "New Staff");
			document.Add(Heading_2("New Staff"));
			document = new NewStaff()
				.Build(this, document);
			NewPage(document);

			var infrastructureOutline = this.AddOutline(parentOutline, "Buildings and Rooms");
			document.Add(Heading_1("Buildings and Rooms"));

			AddOutline(infrastructureOutline, "Classrooms");
			document.Add(Heading_2("Classrooms"));
			document = new Classrooms()
				.Build(this, document);
			NewPage(document);

			AddOutline(infrastructureOutline, "School Site");
			document.Add(Heading_2("School Sites"));
			document = new SchoolSite()
				.Build(this, document);
			NewPage(document);

			AddOutline(infrastructureOutline, "Teacher Housing");
			document.Add(Heading_2("Teacher Housing"));
			document = new TeacherHousing()
				.Build(this, document, lookups["metaResourceDefinitions"]);
			NewPage(document);

			var washOutline = this.AddOutline(parentOutline, "Water, Sanitation and Hygiene");
			document.Add(Heading_1("Water, Sanitation and Hygiene"));

			AddOutline(washOutline, "Water");
			document.Add(Heading_2("Water"));
			document = new WASHWater()
				.Build(this, document, lookups["waterSupplyTypes"]);
			NewPage(document);

			AddOutline(washOutline, "Sanitation");
			document.Add(Heading_2("Sanitation"));
			document = new WASHSanitation()
				.Build(this, document, lookups["toiletTypes"]);
			NewPage(document);

			AddOutline(washOutline, "Hygiene");
			document.Add(Heading_2("Hygiene"));
			document = new WASHHygiene()
				.Build(this, document);
			NewPage(document);

			var resourcesOutline = this.AddOutline(parentOutline, "School Resources");
			document.Add(Heading_1("School Resources"));

			AddOutline(resourcesOutline, "School Resources");
			document.Add(Heading_2("School Resources"));
			document = new SchoolResources()
				.Build(this, document, lookups["metaResourceDefinitions"]);
			NewPage(document);

			AddOutline(resourcesOutline, "School Supplies");
			document.Add(Heading_2("School Supplies"));
			document = new SchoolSupplies()
				.Build(this, document);
			NewPage(document);

			var generalOutline = this.AddOutline(parentOutline, "General Comments");
			AddOutline(generalOutline, "Final Comments");
			AddOutline(generalOutline, "Certification");

			document.Add(Heading_1("General Comments"));			
			document = new GeneralComments()
				.Build(this, document);
			NewPage(document);
			Finalise();
			return document;
		}

	}


}
