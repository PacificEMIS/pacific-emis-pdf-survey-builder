using iText.Forms.Fields.Properties;
using iText.Forms.Fields;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Forms;
using iText.Forms.Form.Element;
using static iText.IO.Codec.TiffWriter;
using static surveybuilder.CellMakers;
using Org.BouncyCastle.Asn1.Cmp;
using System.Collections;

namespace surveybuilder
{
	public class TeacherHousing
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();

		public TeacherHousing()
		{
		}

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			var model = new Cell()
				.SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);
			var model12 = new Cell(1, 2)
				.SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);
			var model21 = new Cell(2, 1).SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);
			var model13 = new Cell(1, 3).SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);
			var model15 = new Cell(1, 5).SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);

			document.Add(new Paragraph()
				.Add(@"Record the number of teachers with officially provided housing at your school.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			table.AddRow(
				TextCell(model, ""),
				TextCell(model, "On Site"),
				TextCell(model, "Off Site")
			);
			table.AddRow(
				TextCell(model, "No. of teachers that are provided with housing."),
				NumberCell(model, "Housing.OnSite"),
				NumberCell(model, "Housing.OffSite")
			);
			table.AddRow(
				TextCell(model, "No. of teachers that are not provided with housing"),
				NumberCell(model12, "Housing.N")
			);
			table.AddRow(
				TextCell(model, "No. of teachers whose houses need significant maintenance."),
				NumberCell(model12, "Housing.M")
			);

			document.Add(table);


			document.Add(new Paragraph()
				.Add(@"Record the number and condition of each of the teacher housing types at your school.")
			);

			// Teacher Housing types and conditions
			Table tableTHTConditions = new Table(UnitValue.CreatePercentArray(new float[] { 40, 30, 10, 10, 10 }))
						.UseAllAvailableWidth();

			// table headers
			tableTHTConditions.AddRow(
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Teacher Housing Type "))),
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Number of"))),
				ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Overall Condition")))
			);

			tableTHTConditions.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Good"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Fair"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Poor")))
			);

			// data rows
			// Categories
			var teacherHousingCategories = new Dictionary<string, string>
			{
				{ "Permanent", "TchHP" },
				{ "Traditional", "TchHT" }
			};

			var teacherHousingTypes = new Dictionary<string, string>
			{
				{ "Permanent", "Kitchen, Living, Bath/Toilet" },
				{ "Traditional", "Kitchen, Kiakia, Bath/Toilet" }
			};

			var teacherHousingConditions = new Dictionary<string, string>
			{
				{ "G", "Good" },
				{ "F", "Fair" },
				{ "P", "Poor" }
			};

			foreach (var kvp in teacherHousingCategories)
			{
				var cat = $"Resource.{kvp.Value}.Cat";

				// first the row subheader (TODO no field included yet here)
				tableTHTConditions.AddRow(
					ts.TableSubHeaderStyle(TextCell(model15, ts.TableHeaderStyle($"{kvp.Key}")))
				);

				// Data fields for each row
				for (int i = 0; i < 3; i++)
				{
					string fieldK = $"Resource.{kvp.Key}.R.{i:00}.K";
					string fieldNum = $"Resource.{kvp.Key}.D.{i:00}.Num";
					string fieldC = $"Resource.{kvp.Key}.D.{i:00}.C";

					PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, fieldC).CreateRadioGroup();

					tableTHTConditions.AddRow(
						TextCell(model, teacherHousingTypes[$"{kvp.Key}"] + $", {i + 1} Bedroom"),
						NumberCell(model, fieldNum),
						SelectCell(model, rgrp, "G"),
						SelectCell(model, rgrp, "F"),
						SelectCell(model, rgrp, "P")
					);
				}
			}

			document.Add(tableTHTConditions);


			return document;
		}
	}
}
