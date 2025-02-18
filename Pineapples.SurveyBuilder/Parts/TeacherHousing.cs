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
using Org.BouncyCastle.Asn1.Cmp;
using System.Collections;
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class TeacherHousing
	{

		public TeacherHousing()
		{
		}

		public Document Build(PdfBuilder builder, Document document, LookupList resources)
		{
			Console.WriteLine("Part: Teacher Housing");

			var houseTypeConditionFields = new ConditionalFields("House condition",
					"House condition not specified");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

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
						.UseAllAvailableWidth()
						.SetMarginBottom(20);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, (""))),
				ts.TableHeaderStyle(TextCell(model, "On Site")),
				ts.TableHeaderStyle(TextCell(model, "Off Site"))
			);
			table.AddRow(
				TextCell(model, "No. of teachers that are provided with housing").Style(ts.TableRowHeaderStyle),
				NumberCell(model, "Housing.OnSite"),
				NumberCell(model, "Housing.OffSite")
			);
			table.AddRow(
				TextCell(model, "No. of teachers that are not provided with housing").Style(ts.TableRowHeaderStyle),
				NumberCell(model12, "Housing.N")
			);
			table.AddRow(
				TextCell(model, "No. of teachers whose houses need significant maintenance").Style(ts.TableRowHeaderStyle),
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
				{ "Staff Housing Permanent", "TchHousePerm" },
				{ "Staff Housing Traditional", "TchHouseTrad" }
			};

			// we'll push all the Num column into this array
			var alternatives = new List<string>();
			foreach (var kvp in teacherHousingCategories)
			{
				// export the full category name
				var cat = $"Resource.{kvp.Value}.Cat";
				ExportValue(builder.pdfDoc, cat, kvp.Key);
				
				tableTHTConditions.AddRow(
					ts.TableSubHeaderStyle(TextCell(model15, ts.TableHeaderStyle($"{kvp.Key}")))
				);

				// Get the category resources
				var catResFilter = new Dictionary<string, object>
					{
						{ "Cat", $"{kvp.Key}" },
						{ "Surveyed", "True" }
					};

				LookupList catResources = resources.FilterByMetadata(catResFilter);
				// create the row fields from the lookup list
				catResources.AsFields(document.GetPdfDocument(),
					(j) => $"Resource.{kvp.Value}.R.{j:00}.K", (j) => $"Resource.{kvp.Value}.R.{j:00}.V");

				// Data fields for each row
				var i = 0;
				foreach (var lookupRes in catResources)
				{
					string tag = kvp.Value; //.Clean();
					string fieldK = $"Resource.{tag}.R.{i:00}.K";
					string fieldA = $"Resource.{tag}.D.{i:00}.A"; // Not used
					string fieldNum = $"Resource.{tag}.D.{i:00}.Num";
					string fieldC = $"Resource.{tag}.D.{i:00}.C";
					// tracking validations as field names are generated
					alternatives.Add(fieldNum);
					houseTypeConditionFields.Add(ConditionalField.IfAny(fieldNum, fieldC));

					PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, fieldC).CreateRadioGroup();

					tableTHTConditions.AddRow(
						TextCell(model, lookupRes.N).Style(ts.TableBaseStyle),
						NumberCell(model, fieldNum),
						SelectCell(model, rgrp, "G"),
						SelectCell(model, rgrp, "F"),
						SelectCell(model, rgrp, "P")
					);
					
					i++;
				}
			}

			document.Add(tableTHTConditions);

			// Validation
			var requiredFields = new RequiredFields("Teacher Housing",
				"Teacher housing is missing information."
				)
				.AddAlternatives("Housing.OnSite", "Housing.OffSite", "Housing.N");
			ValidationManager.AddRequiredFields(document.GetPdfDocument(), requiredFields);
			var houseTypeFields = new ConditionalFields("Teacher Housing Types",
				"Teacher housing types not specified.");
			houseTypeFields.Add(ConditionalField.IfAnyAlternatives("Housing.OnSite",alternatives.ToArray()));
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), houseTypeFields);
			// condition dependencies added after house type dependencies
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), houseTypeConditionFields);

			return document;
		}
	}
}
