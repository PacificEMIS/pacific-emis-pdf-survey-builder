using iText.Forms.Fields;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Forms;
using iText.Forms.Form.Element;
using static iText.IO.Codec.TiffWriter;
using iText.Forms.Fields.Properties;
using static surveybuilder.CellMakers;
using surveybuilder.Utilities;
using iText.Kernel.Pdf;

namespace surveybuilder
{

	public class SchoolInfo
	{
		public SchoolInfo() { }

		public Document Build(PdfBuilder builder, Document document, PdfOutline schoolInfoOutline)
		{
			Console.WriteLine("Part: SchoolInfo");

			const string conditionalMsg = "You need to supply details of your Parent Committee";
			ConditionalFields conditionalFields = new ConditionalFields("SchoolInfo", conditionalMsg);
			const string requiredMsg = "Some answers are missing from General Information.";
			RequiredFields requiredFields = new RequiredFields("SchoolInfo", requiredMsg);

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model12 = CellStyleFactory.TwoColumn;
			var model14 = CellStyleFactory.FourColumn;
			var model31 = CellStyleFactory.ThreeRowOneColumn;


			builder.AddOutline(document, schoolInfoOutline, "General Information");
			builder.Heading_2("General Information", document);

			Table table = CellStyleFactory.DefaultTable(35, 15, 50);
			//AddRow is an extension method in PdfExtensions
			table.AddRow(
				TextCell(model12, "School Name").Style(ts.TableRowHeaderStyle),
				InputCell(model, "Survey.SchoolName", 50)
			);

			table.AddRow(
				TextCell(model12, ts.TableRowHeaderStyle("Registered Number")),
				InputCell(model, "Survey.SchoolNo", 20)
			);

			table.AddRow(
				TextCell(model31, ts.TableRowHeaderStyle("Header Teacher's Name")),
				TextCell(model, ts.TableRowHeaderStyle("First:")),
				InputCell(model, "Survey.HtGiven")
			);

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Last:")),
				InputCell(model, "Survey.HtFamily", 20)
			);

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Mobile #:")),
				InputCell(model, "Survey.HtPh", 20)
			);

			table.AddRow(
				TextCell(model12, ts.TableRowHeaderStyle("School Email")),
				InputCell(model, "Survey.SchoolEmail", 50)
			);

			table.AddRow(
				TextCell(model12, ts.TableRowHeaderStyle("School Phone")),
				InputCell(model, "Survey.SchoolPhone", 20)
			);

			table.AddRow(
				TextCell(model12, ts.TableRowHeaderStyle("Survey Year")),
				InputCell(model, "Survey.SurveyYear", 4, builder.SurveyYear.ToString(), true)
			);

			document.Add(table);

			builder.AddOutline(document, schoolInfoOutline, "Parent's Committee");
			builder.Heading_2("Parents' Committee", document);

			string prompt = @"Record the details of the school's Parents Committee.";
			builder.AddParagraph(prompt, document);

			table = new Table(UnitValue.CreatePercentArray(new float[] { 8, 1, 1, 1, 1 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, "PC.Exists")
				.CreateRadioGroup();
			rgrp.SetAlternativeName("Parent committee exists");

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Does your school have a Parents' Committee")),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Yes"))),
				YesCell(model, rgrp),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("No"))),
				NoCell(model, rgrp)
			);

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("If Yes, how many times did it meet last year?")),
				NumberCell(model14, "PC.Meet")
			);
			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("No of Males and Females on your Parents' Committee")),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("M"))),
				NumberCell(model, "PC.Members.M"),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("F"))),
				NumberCell(model, "PC.Members.F")

			);
			document.Add(table);

			builder.AddOutline(document, schoolInfoOutline, "Community Support");
			builder.Heading_2("Community Support", document);

			prompt = @"On the scale below rate the level of support your school receives from the local community.";
			builder.AddParagraph(prompt, document);

			string[] Columns = { "Excellent", "Very Good", "Satisfactory", "Poor", "Very Bad" };
			object[] values = { "Excellent", "Very_Good", "Satisfactory", "Poor", "Very_Bad" };

			var chk = new CheckBoxPickmaker();
			chk.Names = Columns;
			chk.Values = values;
			chk.Types = new CheckBoxType[] {CheckBoxType.CHECK, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.SQUARE };
			chk.Tag = "PC.Support";
			chk.Description = "Local community support";
			chk.Make(builder, document);

			#region School Structure

			builder.AddOutline(document, schoolInfoOutline, "Overall School Structure");

			builder.Heading_2("Overall School Structure", document);
			prompt = @"Record the following summary details about the overall structure of your school.";
			builder.AddParagraph(prompt, document);

			table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 }))
						.UseAllAvailableWidth();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "")),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Number")))
			);
			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Total number of pupils")),
				NumberCell(model, "Survey.Pupils")
			);

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Total number of classes")),
				NumberCell(model, "Survey.Classes")
			);

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Total teachers (including Head Teacher)")),
				NumberCell(model, "Survey.Teachers")
			);
			#endregion
			document.Add(table);

			requiredFields.Add("Survey.HtFamily", "Survey.HtGiven",  "Survey.SchoolPhone",  
				"PC.Exists", "Survey.Classes", "Survey.Pupils", "Survey.Teachers", "PC.Support");

			ValidationManager.AddRequiredFields(document.GetPdfDocument(), requiredFields);
			conditionalFields.Add(ConditionalField.IfYes("PC.Exists", "PC.Members.M", "PC.Members.F", "PC.Meet"));
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), conditionalFields);

			return document;
		}

	}
}
