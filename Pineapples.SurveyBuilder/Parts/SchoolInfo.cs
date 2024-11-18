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

namespace surveybuilder
{

	public class SchoolInfo
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public SchoolInfo() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{


			// make a table 6 columns grouped 3 1 2

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1, 2 }))
						.UseAllAvailableWidth();

			var model = new Cell().SetHeight(20);
			var model2 = new Cell(1, 2).SetHeight(20);
			var model4 = new Cell(1, 4).SetHeight(20);

			builder.Heading_2("General Information", document);

			//AddRow is an extension method in PdfExtensions
			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "School Name")),
				InputCell(model2, "Survey.SchoolName", 50)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Registered Number")),
				InputCell(model2, "Survey.SchoolNo", 20)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Header Teacher's Name")),
				ts.TableHeaderStyle(TextCell(model, "First:")),
				InputCell(model, "Survey.HtGiven")
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "")),
				ts.TableHeaderStyle(TextCell(model, "Last:")),
				InputCell(model, "Survey.HtFamily", 20)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "")),
				ts.TableHeaderStyle(TextCell(model, "Mobile #:")),
				InputCell(model, "Survey.HtPh", 20)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "School Email")),
				InputCell(model2, "Survey.SchoolEmail", 50)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "School Phone")),
				InputCell(model2, "Survey.SchoolPhone", 20)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Survey Year")),
				InputCell(model2, "Survey.SurveyYear", 4, "2024", true)
			);

			document.Add(table);

			builder.Heading_2("Parents' Committee", document);
			document.Add(new Paragraph(@"Record the details of the school's Parents Committee."));

			table = new Table(UnitValue.CreatePercentArray(new float[] { 8, 1, 1, 1, 1 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, "PC.Exists")
				.CreateRadioGroup();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Does your school have a Parents' Committee")),
				ts.TableHeaderStyle(TextCell(model, "Yes")),
				YesCell(model, rgrp),
				ts.TableHeaderStyle(TextCell(model, "No")),
				NoCell(model, rgrp)
			);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "If Yes, how many times did it meet last year?")),
				NumberCell(model4, "PC.Meet")
			);
			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "No of Males and Females on your Parents' Committee")),
				ts.TableHeaderStyle(TextCell(model, "M")),
				NumberCell(model, "PC.Members.M"),
				ts.TableHeaderStyle(TextCell(model, "F")),
				NumberCell(model, "PC.Members.F")

			);
			document.Add(table);
			rgrp.SetFieldFlags(0);

			builder.Heading_2("Community Support", document);

			string prompt = @"On the scale below rate the level of support your school receives from the local community.";
			document.Add(new Paragraph(prompt));

			string[] Columns = { "Excellent", "Very Good", "Satisfactory", "Poor", "Very Bad" };
			int[] values = { 1, 2, 3, 4, 5 };

			var chk = new CheckBoxPickmaker();
			chk.Names = Columns;
			chk.Values = values;
			chk.Types = new CheckBoxType[] {CheckBoxType.CHECK, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.SQUARE };
			chk.Tag = "PC.Support";
			chk.Make(builder, document);

			builder.Heading_2("Overall School Structure", document);
			prompt = @"Record the following summary details about the overall structure of your school.";
			document.Add(new Paragraph(prompt));

			table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 }))
						.UseAllAvailableWidth();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, "")),
				ts.TableHeaderStyle(TextCell(model, "Number"))
			);
			table.AddRow(
				TextCell(model, "Total number of pupils"),
				NumberCell(model, "Survey.Pupils")
			);

			table.AddRow(
				TextCell(model, "Total number of classes"),
				NumberCell(model, "Survey.Classes")
			);

			table.AddRow(
				TextCell(model, "Total teachers (including Head Teacher)"),
				NumberCell(model, "Survey.Teachers")
			);

			document.Add(table);

			return document;
		}

	}
}
