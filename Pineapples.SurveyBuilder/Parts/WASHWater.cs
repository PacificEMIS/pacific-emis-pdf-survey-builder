using iText.Forms.Fields;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Forms;
using iText.Forms.Fields.Properties;
using static surveybuilder.CellMakers;
using System.Net;

namespace surveybuilder
{
	public class WASHWater
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public WASHWater() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document, List<KeyValuePair<string, string>> waterSupplyTypes)
		{
			// TODO Move to reusable Cell stylesheets
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

			document.Add(builder.Heading_3("Water Supply"));

			document.Add(new Paragraph()
				.Add(@"Record the details of your school water supply.")
			);

			// IMPORTANT: this is now populated from the table [dbo].[lkpWaterSupplyTypes] (or FROM metaResourceDefs WHERE mresCat = 'Water Supply'?!
			// Argh...rather annoying to have these defined differently in two different places. Let's take the one from censuswork (i.e. metaResourceDefs)

			int wstCount = waterSupplyTypes.Count;
			int totalColumns = wstCount + 1;

			// Define the percentage for the first column
			float firstColumnWidth = 40f; // First column gets 30% of the width

			// Calculate the remaining width and distribute it equally among the other columns
			float remainingWidth = 100f - firstColumnWidth;
			float otherColumnsWidth = remainingWidth / (totalColumns - 1);

			// Create an array of column widths
			float[] columnWidths = new float[totalColumns];
			columnWidths[0] = firstColumnWidth; // Assign the first column width
			for (int i = 1; i < totalColumns; i++)
			{
				columnWidths[i] = otherColumnsWidth; // Assign the remaining columns
			}

			// Create a Water supply types table with dynamic column widths
			Table tableWST = new Table(UnitValue.CreatePercentArray(columnWidths))
				.UseAllAvailableWidth();

			// Headers headers
			tableWST.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Water Supply"))));

			// Add water supply type values dynamically to the table
			int wstI = 0;
			foreach (var waterSupplyType in waterSupplyTypes)
			{
				// Headers
				// TODO need to include the field key
				string fieldK = $"Resource.Water.R.{wstI:00}.K";
				tableWST.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle(waterSupplyType.Value))));
				wstI++;
			}

			// Number Row
			tableWST.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Number"))));
			for (int i = 0; i < wstCount; i++)
			{
				string fieldNum = $"Resource.Water.D.{i:00}.Num";
				tableWST.AddCell(NumberCell(model, fieldNum));
			}

			// Total Capacity in Litres Row
			tableWST.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Total Capacity in Litres"))));
			for (int i = 0; i < wstCount; i++)
			{
				// TODO does not handle the water source types where it makes no sense to record the capacity
				// hard code blank cells?!
				string fieldNum = $"Resource.Water.D.{i:00}.Qty";
				tableWST.AddCell(NumberCell(model, fieldNum));
			}

			// Tick if properly covered/protected Row
			tableWST.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Tick if properly covered/protected"))));
			for (int i = 0; i < wstCount; i++)
			{
				string fieldNum = $"Resource.Water.D.{i:00}.Protected";
				PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, fieldNum).CreateRadioGroup();
				tableWST.AddCell(YesCell(model, rgrp));
			}

			document.Add(tableWST);

			document.Add(new Paragraph()
				.Add(@"On the following scale, rate the adequacy of your water supply for pupils and staff in relationship "
				+ @"to the standard you would like to be able to provide. Tick the category which describes it best.")
			);

			string[] Columns = { "Excellent", "Very Good", "Satisfactory", "Poor", "Very Bad" };
			int[] values = { 4, 3, 2, 1, 0 };

			var chk = new CheckBoxPickmaker();
			chk.Names = Columns;
			chk.Values = values;
			chk.Types = new CheckBoxType[] {CheckBoxType.CHECK, CheckBoxType.SQUARE
					,CheckBoxType.CIRCLE,CheckBoxType.DIAMOND,CheckBoxType.CROSS };
			chk.Tag = "Wash.Water.Rating";
			chk.Make(builder, document);

			document.Add(builder.Heading_3("Drinking Water"));

			document.Add(new Paragraph()
				.Add(@"What is the primary source of drinking water in the school?.")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Rainwater", "Groundwater", "Desalinated water", "Bottled water" };
			chk.Values = new int[] { 1, 2, 3, 4 };
			chk.Tag = "Wash.Water.Source";
			chk.Make(builder, document);

			document.Add(new Paragraph()
				.Add(@"For drinking water, what type of water treatment is practiced at the school? (select one only)")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Boiling", "Chlorination", "SODIS", "No water treatment" };
			chk.Values = new int[] { 1, 2, 3, 4 };
			chk.Tag = "Wash.Water.Treatment";
			chk.Make(builder, document);

			document.Add(new Paragraph()
				.Add(@"For drinking water does the school practice water treatment?")
			);
			document.Add(new Paragraph()
				.Add(@"If YES, provide details of the most recent test:")
			);
			Table tableWaterTreatment = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }))
						.UseAllAvailableWidth();

			tableWaterTreatment.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Date tested:")),
				DateCell(model, "Wash.Water.Test.Date")
			);
			tableWaterTreatment.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Tested by")),
				InputCell(model, "Wash.Water.Test.By")
			);
			tableWaterTreatment.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Total Coliform found")),
				NumberCell(model, "Wash.Water.Test.Result")
			);

			document.Add(tableWaterTreatment);

			return document;
		}
	}
}
