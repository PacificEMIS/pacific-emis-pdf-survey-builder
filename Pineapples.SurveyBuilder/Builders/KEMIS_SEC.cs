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
	public class KEMIS_SEC : KEMIS_PRI				// allow some sharing with KEMIS_PRI
	{
		public KEMIS_SEC()
		{

		}
		public override string Description
		{
			get
			{
				return "Kiribati Secondary Survey for 2025 and after";
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
			PdfOutline parentOutline = AddOutline(document, rootoutline, "KEMIS Survey");

			/**************************************************************************/
			/* Begin Pdf output														  */
			/**************************************************************************/

			CoverPage(document, "Secondary");

			#region ***** School Information *****
			var schoolInfoOutline = this.AddOutline(document, parentOutline, "School Information");

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

			AddOutline(document, enrolOutline, "Distance from School");
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
			

			#endregion

			#region *********** Staff *************
			NewPage(document, PageSize.A4.Rotate());
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

			AddOutline(document, infrastructureOutline, "Dormitories");
			document.Add(Heading_2("Dormitories"));
			document = new Dormitories()
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
			AddOutline(document, generalOutline, "Final Comments");
			AddOutline(document, generalOutline, "Certification");

			document.Add(Heading_1("General Comments"));
			document = new GeneralComments()
				.Build(this, document);
			NewPage(document);
			#endregion

			Finalise();
			return document;
		}

		public override void AddCustomLookupLists()
		{


			// Moved to KeyValuePair to a custom LookupEntry class which can also hold metadata.
			var ages = new string[] { "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22" }
							.Select(n => new LookupEntry
							{
								C = n, // Set the primary code (C)
								N = n  // Set the primary name (N)
							})
							.ToLookupList();
			lookups.Add("ages", ages);

			var classLevels = new LookupList()
			{
				new LookupEntry() {C = "JS1", N = "Form 1" },
				new LookupEntry() { C = "JS2", N = "Form 2" },
				new LookupEntry() { C = "JS3", N = "Form 3" },
				new LookupEntry() { C = "SS1", N = "Form 4" },
				new LookupEntry() { C = "SS2", N = "Form 5" },
				new LookupEntry() { C = "SS3", N = "Form 6" },
				new LookupEntry() { C = "SS4", N = "Form 7" }
			};

			lookups.Add("classLevels", classLevels);

			// teacher roles vary across school types

			LookupList tmp = lookups["schoolTypeRoles"]
							.FilterByMetadata("T", "CS");       // use values for combined sec school
			lookups.Add("filteredRoles", tmp);
			tmp = lookups["teacherQualGroups"]
							.FilterByMetadata("E", true);       // use values for combined sec school
			lookups.Add("qualEd", tmp);
			tmp = lookups["teacherQualGroups"]
				.FilterByMetadata("E", false);       // use values for combined sec school
			lookups.Add("qualN", tmp);
		}


	}
	

}
