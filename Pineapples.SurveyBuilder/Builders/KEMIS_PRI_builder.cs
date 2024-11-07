using System;
using System.IO;
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
using iText.Forms.Form.Element;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.Kernel.Pdf.Navigation;
using iText.Commons.Actions.Data;
using static surveybuilder.CellMakers;
using System.Runtime.CompilerServices;
using System.Configuration;
using iText.Kernel.XMP.Impl;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.Design;
using System.Security.Principal;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Org.BouncyCastle.Utilities;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Net.NetworkInformation;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Font;


namespace surveybuilder
{
	public class KEMIS_PRI_Builder : PdfBuilder
	{
		public KEMIS_PRI_Builder(PdfStylesheet stylesheet, PdfDocument pdfDoc) : base(stylesheet, pdfDoc)
		{

		}

		public new Document Build()
		{
			dataHost = ConfigurationManager.AppSettings["emisUrl"]; // dataHost = $"https://kemis-test.pacific-emis.org";
			InitLookups();
			AddLookups("student");

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
				  .ShowText("2024")
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

			GenderedGridmaker grd = new GenderedGridmaker();

			// TODO temporarily not include javascript (I don't have it)
			// after setting all the fields, add the javascript libraries
			//var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);

			//string jsPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["pineapplesPath"], @"Pineapples.Client\assets\pdfSurvey\js");
			//foreach (string jsfile in System.IO.Directory.GetFiles(jsPath))
			//{
			//	string jscriptText = System.IO.File.ReadAllText(jsfile);
			//	iText.Kernel.Pdf.PdfDictionary jscript = iText.Kernel.Pdf.Action.PdfAction
			//		.CreateJavaScript(jscriptText).GetPdfObject();
			//	javaScriptNameTree.AddEntry(System.IO.Path.GetFileName(jsfile), jscript);
			//}

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
			AddOutline(enrolOutline, "Distance from School");


			document.Add(Heading_1("Enrolment Details"));

			var rows = new string[] { "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" }
							.Select(n => new KeyValuePair<string, string>(n, n))
							.ToList();
			grd.Rows = rows;

			var classLevels = new List<KeyValuePair<string, string>>();

			classLevels.Add(new KeyValuePair<string, string>("P1", "Class 1"));
			classLevels.Add(new KeyValuePair<string, string>("P2", "Class 2"));
			classLevels.Add(new KeyValuePair<string, string>("P3", "Class 3"));
			classLevels.Add(new KeyValuePair<string, string>("P4", "Class 4"));
			classLevels.Add(new KeyValuePair<string, string>("P5", "Class 5"));
			classLevels.Add(new KeyValuePair<string, string>("P6", "Class 6"));
			grd.Columns = classLevels;

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

			grd.Tag = "TRIN";
			grd.Rows = lookups["islands"];
			document.Add(grd.Make(this));
			NewPage(document);

			// Using different row/columns for GenderedGridMaker here
			rows = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }
							.Select(n => new KeyValuePair<string, string>(n, n))
							.ToList();
			grd.Rows = rows;

			var ages = new List<KeyValuePair<string, string>>();

			ages.Add(new KeyValuePair<string, string>("5", "5"));
			ages.Add(new KeyValuePair<string, string>("6", "6"));
			ages.Add(new KeyValuePair<string, string>("7", "7"));
			ages.Add(new KeyValuePair<string, string>("8", "8"));
			grd.Columns = ages;

			AddOutline(enrolOutline, "Pre-School Attendance");
			document.Add(Heading_2("Pupils Who Have Attended Pre-School"));
			document = new PSA()
				.Build(this, document, grd);
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

			NewPage(document);
			document.Add(Heading_1("Water, Sanitation and Hygiene"));
			AddOutline(parentOutline, "Water, Sanitation and Hygiene");
			document = new KEMIS_Wash().Build(this, document);




			return document;
		}

	}


}
