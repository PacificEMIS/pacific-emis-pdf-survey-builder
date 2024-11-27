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
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class TeacherHousing
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();

		public TeacherHousing()
		{
		}

		public Document Build(PdfBuilder builder, Document document, List<LookupEntry> resources)
		{
			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model12 = CellStyleFactory.TwoColumn;
			var model13 = CellStyleFactory.ThreeColumn;
			var model15 = CellStyleFactory.FiveColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;

			document.Add(new Paragraph()
				.Add(@"Record the number of teachers with officially provided housing at your school.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle(""))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("On Site"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Off Site")))
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("No. of teachers that are provided with housing.")),
				NumberCell(model, "Housing.OnSite"),
				NumberCell(model, "Housing.OffSite")
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("No. of teachers that are not provided with housing")),
				NumberCell(model12, "Housing.N")
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("No. of teachers whose houses need significant maintenance.")),
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
			// Currently the Resource Lookup C (keys) don't match the XML field so hard coded here.
			var teacherHousingCategories = new Dictionary<string, string>
			{
				{ "Staff Housing Permanent", "TchHP" },
				{ "Staff Housing Traditional", "TchHT" }
			};

			foreach (var kvp in teacherHousingCategories)
			{
				var cat = $"Resource.{kvp.Value}.Cat";

				// first the row subheader (TODO no field included yet here)
				tableTHTConditions.AddRow(
					ts.TableSubHeaderStyle(TextCell(model15, ts.TableHeaderStyle($"{kvp.Key}")))
				);

				// Get the category resources
				var catResFilter = new Dictionary<string, object>
					{
						{ "Cat", $"{kvp.Key}" },
						{ "Surveyed", "True" }
					};
				List<LookupEntry> catResources = LookupEntry.FilterByMetadata(resources, catResFilter);

				// Data fields for each row
				var i = 0;
				foreach (var lookupRes in catResources)
				{
					string fieldK = $"Resource.{kvp.Key}.R.{i:00}.K";
					string fieldA = $"Resource.{lookupRes.C}.D.{i:00}.A"; // Not used
					string fieldNum = $"Resource.{kvp.Key}.D.{i:00}.Num";
					string fieldC = $"Resource.{kvp.Key}.D.{i:00}.C";

					PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, fieldC).CreateRadioGroup();

					tableTHTConditions.AddRow(
						TextCell(model, ts.TableBaseStyle($"{lookupRes.N}")),
						NumberCell(model, fieldNum),
						SelectCell(model, rgrp, "G"),
						SelectCell(model, rgrp, "F"),
						SelectCell(model, rgrp, "P")
					);
					i++;
				}
			}

			document.Add(tableTHTConditions);


			return document;
		}
	}
}
