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
using Colors = iText.Kernel.Colors;

namespace surveybuilder
{
	public class TeacherHousing
	{
		public PdfStylesheet styles = new PdfStylesheet();

		public TeacherHousing()
		{
			string color1 = "eaeaea";
			string color2 = "cccccc";
			string color3 = "a8a8a8";

			// TODO all styles here are likely to quickly become used in many places (all/most tables?)
			// this would be better pulled out into PDFStylesheets.cs somehow.
			styles.Add("tableheader",
				new PdfStyle(styles["base"])
				{
					FontSize = 12,
					TextAlignment = TextAlignment.CENTER
				});
			styles.Add("tableheadercell",
				new PdfStyle(styles["base"])
				{
					BackgroundColor = Colors.ColorConstants.LIGHT_GRAY
				});
			styles.Add("tablesubheadercell",
				new PdfStyle(styles["base"])
				{
					BackgroundColor = Colors.WebColors.GetRGBColor(color3)
				});
		}
		Paragraph tableheader(string text)
		{
			return styles.ApplyStyle("tableheader", text);
		}

		Cell tableheader(Cell cell)
		{
			return styles.ApplyCell("tableheadercell", cell);
		}
		Cell tablesubheader(Cell cell)
		{
			return styles.ApplyCell("tablesubheadercell", cell);
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
					tableheader(TextCell(model21, tableheader("Teacher Housing Type "))),
						tableheader(TextCell(model21, tableheader("Number of"))),
						tableheader(TextCell(model13, tableheader("Overall Condition")))
					);

			tableTHTConditions.AddRow(
					tableheader(TextCell(model, tableheader("Good"))),
					tableheader(TextCell(model, tableheader("Fair"))),
					tableheader(TextCell(model, tableheader("Poor")))
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
					tablesubheader(TextCell(model15, tableheader($"{kvp.Key}")))
				);

				// Data fields for each row
				for (int i = 0; i < 3; i++)
				{
					string fieldK = $"Resource.{kvp.Key}.R.{i:00}.K";
					string fieldNum = $"Resource.{kvp.Key}.D.{i:00}.Num";
					string fieldC = $"Resource.{kvp.Key}.D.{i:00}.C";

					PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, fieldC).CreateRadioGroup();

					tableTHTConditions.AddRow(
						TextCell(model, teacherHousingTypes[$"{kvp.Key}"]+$", {i+1} Bedroom"),
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
