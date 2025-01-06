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
using surveybuilder.Utilities;
using System.Security.Policy;

namespace surveybuilder
{
	public class WASHWater
	{
		const string conditionalMsg = "You need to specify the test results of last water test";
		ConditionalFields conditionalFields = new ConditionalFields("WashWater", conditionalMsg);
		const string requiredMsg = "Some answers are missing from Wash Water. Review now?";
		RequiredFields requiredFields = new RequiredFields("WashWater", requiredMsg);

		public WASHWater() { }

		public Document Build(PdfBuilder builder, Document document, LookupList waterSupplyTypes)
		{
			Console.WriteLine("Part: Wash - Water");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var modelb = CellStyleFactory.DefaultNoHeight;
			var model2 = CellStyleFactory.TwoColumn;



			document.Add(builder.Heading_3("Water Supply"));

			document.Add(new Paragraph()
				.Add(@"Record the details of your school water supply.")
			);




			// IMPORTANT: this is now populated from the table [dbo].[lkpWaterSupplyTypes] (or FROM metaResourceDefs WHERE mresCat = 'Water Supply'?!
			// Argh...rather annoying to have these defined differently in two different places. Let's take the one from censuswork (i.e. metaResourceDefs)

			int wstCount = waterSupplyTypes.Count;
			int totalColumns = wstCount * 2 + 1;

			// Define the percentage for the first column
			float firstColumnWidth = 40f; // First column gets 40% of the width

			// Calculate the remaining width and distribute it equally among the other columns
			float remainingWidth = 100f - firstColumnWidth;
			float otherColumnsWidth = remainingWidth / (wstCount * 2);

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
			tableWST.AddCell(TextCell(modelb, "Water Supply").Style(ts.TableRowHeaderStyle));

			// Add water supply type values dynamically to the table
			int wstI = 0;
			foreach (var waterSupplyType in waterSupplyTypes)
			{
				// Headers
				tableWST.AddCell(TextCell(model2, waterSupplyType.N).Style(ts.TableHeaderStyle));
				wstI++;
			}

			// Number Row
			tableWST.AddCell(TextCell(model, ts.TableBaseStyle("Number")));
			for (int i = 0; i < wstCount; i++)
			{
				string fieldNum = $"Resource.Water.D.{i:00}.Num";
				tableWST.AddCell(NumberCell(model2, fieldNum));
			}

			// Total Capacity in Litres Row
			tableWST.AddCell(TextCell(model, ts.TableBaseStyle("Total Capacity in Litres")));
			for (int i = 0; i < wstCount; i++)
			{
				// TODO does not handle the water source types where it makes no sense to record the capacity
				// hard code blank cells?!
				// No, use the available metadata
				string fieldNum = $"Resource.Water.D.{i:00}.Qty";
				bool promptQty = (bool)waterSupplyTypes[i].Metadata["PromptQty"];
				tableWST.AddCell(NumberCell(model2, fieldNum,configurer: ReadOnlyConfigurer(!promptQty)));
			}

			// Tick if properly covered/protected Row
			tableWST.AddCell(TextCell(model, ts.TableBaseStyle("Supply is properly covered/protected")));
			for (int i = 0; i < wstCount; i++)
			{
				// OK is a Y/N field supported on the server in xfdfResourceList - use that...
				string fieldNum = $"Resource.Water.D.{i:00}.OK";
				PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, fieldNum).CreateRadioGroup();
				tableWST.AddCell(YesCell(model, rgrp));
				tableWST.AddCell(NoCell(model, rgrp));
			}

			document.Add(tableWST);
			// export the water supply metadata
			waterSupplyTypes.AsFields(builder.pdfDoc,
				j => $"Resource.Water.R.{j:00}.K", j => $"Resource.Water.R.{j:00}.V");


			document.Add(new Paragraph()
				.Add(@"On the following scale, rate the adequacy of your water supply for pupils and staff in relationship "
				+ @"to the standard you would like to be able to provide. Tick the category which describes it best.")
			);

			string[] Columns = { "Excellent", "Very Good", "Satisfactory", "Poor", "Very Bad" };
			object[] values = { 4, 3, 2, 1, 0 };

			var chk = new CheckBoxPickmaker();
			chk.Names = Columns;
			chk.Values = values;

			chk.Types = new CheckBoxType[] {CheckBoxType.STAR, CheckBoxType.SQUARE
					,CheckBoxType.CIRCLE,CheckBoxType.DIAMOND,CheckBoxType.CROSS };
			chk.Colors = new Color[] { ColorConstants.ORANGE };
			chk.Tag = "Wash.Water.Rating";
			chk.Description = "Adequate water supply";

			chk.Make(builder, document);

			document.Add(builder.Heading_3("Drinking Water"));

			document.Add(new Paragraph()
				.Add(@"What is the primary source of drinking water in the school?.")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Rainwater", "Groundwater", "Desalinated water", "Bottled water" };
			chk.Values = new object[] { 1, 2, 3, 4 };
			chk.DefaultColor = ColorConstants.PINK;
			chk.Tag = "Wash.Water.Source";
			chk.Description = "Primary source of drinking water";

			chk.Make(builder, document);

			document.Add(new Paragraph()
				.Add(@"For drinking water, what type of water treatment is practiced at the school? (select one only)")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Boiling", "Chlorination", "SODIS", "No water treatment" };
			chk.Values = new object[] { 1, 2, 3, 4 };
			chk.Colors = new Color[] { ColorConstants.PINK, null, ColorConstants.ORANGE };
			chk.Tag = "Wash.Water.Treatment";
			chk.Description = "Water treatment used";

			chk.Make(builder, document);

			document.Add(new Paragraph()
				.Add(@"For drinking water does the school practice water treatment? If YES, provide details of the most recent test:")
			);

			Table tableWaterTreatment = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }))
						.UseAllAvailableWidth();

			tableWaterTreatment.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Water Treatment Details (if treated)"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Answer")))
			);
			tableWaterTreatment.AddRow(
				TextCell(model, ts.TableBaseStyle("Date tested:")),
				DateCell(model, "Wash.Water.Test.Date")
			);
			tableWaterTreatment.AddRow(
				TextCell(model, ts.TableBaseStyle("Tested by")),
				InputCell(model, "Wash.Water.Test.By")
			);
			tableWaterTreatment.AddRow(
				TextCell(model, ts.TableBaseStyle("Total Coliform found")),
				NumberCell(model, "Wash.Water.Test.Result")
			);

			document.Add(tableWaterTreatment);
			requiredFields.Add("Wash.Water.Treatment");
			requiredFields.GenerateJavaScript(document.GetPdfDocument());
			conditionalFields.Add(new ConditionalField()
			{
				Test = "Wash.Water.Treatment",
				Value = new string[] { "1", "2", "3" },
				Rq = new string[] { "Wash.Water.Test.Date", "Wash.Water.Test.By", "Wash.Water.Test.Result" }
			});
			conditionalFields.GenerateJavaScript(document.GetPdfDocument());
			return document;
		}
	}
}
