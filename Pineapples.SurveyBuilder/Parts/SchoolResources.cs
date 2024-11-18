﻿using iText.Forms.Fields.Properties;
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
using System.ComponentModel;

namespace surveybuilder
{
	public class SchoolResources
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public Document Build(KEMIS_PRI_Builder builder, Document document, List<LookupEntry> resources)
		{

			// TODO Move to reusable Cell stylesheets once all tables in survey complete the the cell styling is clear
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
			var model17 = new Cell(1, 7).SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);

			document.Add(builder.Heading_3("School Resources"));

			document.Add(new Paragraph()
				.Add(@"Record whether each of the following resources are available at your school, the number available and "
				+ @"the condition they are in.")
			);

			// School resources types and conditions
			Table tableSRTConditions = new Table(UnitValue.CreatePercentArray(new float[] { 40, 10, 10, 10, 10, 10, 10 }))
						.UseAllAvailableWidth();

			// table headers
			tableSRTConditions.AddRow(
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Resources"))),
				ts.TableHeaderStyle(TextCell(model12, ts.TableHeaderStyle("Available?"))),
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Number"))),
				ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Overall Condition")))
			);

			tableSRTConditions.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Yes"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("No"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Good"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Fair"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Poor")))
			);

			// data rows
			// Categories
			// Currently the Resource Lookup C (keys) don't match the XML field so hard coded here.
			var resourcesCategories = new Dictionary<string, string>
			{
				{ "Communications", "Comm" },
				// When getting Equipment from metaResourceDefs they are all under Communications category (how to safely clean mismatch?)
				{ "Equipment", "Eqp" },
				{ "Power Supply", "Power" },
				{ "Library Resources", "Library" }
			};

			foreach (var kvp in resourcesCategories)
			{
				var cat = $"Resource.{kvp.Value}.Cat";

				// first the row subheader (TODO no field included yet here)
				tableSRTConditions.AddRow(
					ts.TableSubHeaderStyle(TextCell(model17, ts.TableHeaderStyle($"{kvp.Key}")))
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
					string fieldK = $"Resource.{lookupRes.C}.R.{i:00}.K";
					string fieldA = $"Resource.{lookupRes.C}.D.{i:00}.A";
					string fieldNum = $"Resource.{lookupRes.C}.D.{i:00}.Num";
					string fieldC = $"Resource.{lookupRes.C}.D.{i:00}.C";

					PdfButtonFormField rgrpAvail = new RadioFormFieldBuilder(builder.pdfDoc, fieldA).CreateRadioGroup();
					PdfButtonFormField rgrpC = new RadioFormFieldBuilder(builder.pdfDoc, fieldC).CreateRadioGroup();

					tableSRTConditions.AddRow(
						TextCell(model, $"{lookupRes.N}"),
						YesCell(model, rgrpAvail),
						NoCell(model, rgrpAvail),
						NumberCell(model, fieldNum),
						SelectCell(model, rgrpC, "G"),
						SelectCell(model, rgrpC, "F"),
						SelectCell(model, rgrpC, "P")
					);
					i++;
				}
			}

			document.Add(tableSRTConditions);

			document.Add(builder.Heading_3("Internet Resources"));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrp1 = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.InternetRachel")
				.CreateRadioGroup();

			table.AddRow(
				TextCell(model, ""),
				TextCell(model, "Yes"),
				TextCell(model, "No")
			);
			table.AddRow(
				TextCell(model, "Does your school have access to internet or to a device like RACHEL*"),
				YesCell(model, rgrp1),
				NoCell(model, rgrp1)
			);

			document.Add(table);

			document.Add(new Paragraph()
				.Add(@"* RACHEL (Remote Area Community Hotspot for Education and Learning) is a portable, battery-powered, device that "
				+ "contains education resources that can be used for teacher and learning even offline.")
			);

			return document;
		}
	}
}
