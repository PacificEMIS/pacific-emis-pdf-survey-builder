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
using System.ComponentModel;
using surveybuilder.Utilities;
using System.Net;

namespace surveybuilder
{
	public class SchoolResources
	{
		// Cell layout/styling models
		Cell model = CellStyleFactory.Default;
		Cell model12 = CellStyleFactory.TwoColumn;
		Cell model13 = CellStyleFactory.ThreeColumn;
		Cell model17 = CellStyleFactory.SevenColumn;
		Cell model21 = CellStyleFactory.TwoRowOneColumn;
		// table styles used
		PdfStylesheet ss;       // for brevity

		const string TableHeaderStyle = "colheader";
		const string TableSubHeaderStyle = "tablesubheader";
		const string TableBaseStyle = "tablebase";


		public Document Build(PdfBuilder builder, Document document, LookupList resourcesCategories)
		{
			Console.WriteLine("Part: School Resources");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			ss = builder.stylesheet;

			document.Add(builder.Heading_3("School Resources"));

			document.Add(new Paragraph()
				.Add(@"Record whether each of the following resources are available at your school, the number available and "
				+ @"the condition they are in.")
			);

			// School resources types and conditions

			WriteResourcesHeader(document);

			// data rows

			LookupList resources = builder.lookups["metaResourceDefinitions"];
			foreach (var categoryDef in resourcesCategories)
			{
				// make the hidden field with this category definition
				CellMakers.ExportValue(builder.pdfDoc
					, $"Resource.{categoryDef.C}.Cat", categoryDef.N);

				// Get the category resources
				var catResFilter = new Dictionary<string, object>
					{
						{ "Cat", $"{categoryDef.N}" },  // note it is the category name
						{ "Surveyed", "True" }
					};
				LookupList catResources = resources.FilterByMetadata(catResFilter);
				if (catResources.Count() == 0)
				{
					continue;
				}
				// now we are going to determine whether the is enough space on the page for this group
				float spaceneeded =
					CellStyleFactory.DefaultTable(1).HeightEstimate(model, catResources.Count() + 1);
				if (builder.NewPageIf(document, spaceneeded))
				{
					WriteResourcesHeader(document);
					
				}
				WriteCategoryGroup(document, categoryDef, catResources);
			}


			document.Add(builder.Heading_3("Internet Resources"));

			Table table = CellStyleFactory.DefaultTable(80, 10, 10);

			PdfButtonFormField rgrp1 = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.InternetRachel")
				.CreateRadioGroup();

			table.AddRow(ss[TableHeaderStyle],
				TextCell(model, ""),
				TextCell(model, "Yes"),
				TextCell(model, "No")
			);
			table.AddRow(
				TextCell(model,"Does your school have access to internet or to a device like RACHEL*")
					.Style(ss[TableBaseStyle]),
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

		private Document WriteResourcesHeader(Document document)
		{
			//Table tableSRTConditions = new Table(UnitValue.CreatePercentArray(new float[] { 40, 10, 10, 10, 10, 10, 10 }))
			//		.UseAllAvailableWidth();
			Table hdr = CellStyleFactory.DefaultTable(40, 10, 10, 10, 10, 10, 10)
				.SetMarginBottom(0);            // so it joins up with the following group
												// table headers
			hdr.AddRow(ss[TableHeaderStyle],
				TextCell(model21, "Resources"),
				TextCell(model12, "Available?"),
				TextCell(model21, "Number"),
				TextCell(model13, "Overall Condition")
			);

			hdr.AddRow(ss[TableHeaderStyle],
				TextCell(model, "Yes"),
				TextCell(model, "No"),
				TextCell(model, "Good"),
				TextCell(model, "Fair"),
				TextCell(model, "Poor")
			);
			document.Add(hdr);
			return document;
		}

		private Document WriteCategoryGroup(Document document, LookupEntry category, LookupList catResources)
		{
			catResources.AsFields(document.GetPdfDocument(),
				(j) => $"Resource.{category.C}.R.{j:00}.K", (j) => $"Resource.{category.C}.R.{j:00}.V");

			Table cattable = CellStyleFactory.DefaultTable(40, 10, 10, 10, 10, 10, 10);
			// first the row subheader (TODO no field included yet here)
			cattable.AddRow(ss[TableSubHeaderStyle],
				TextCell(model17, category.N)
			);

			// Data fields for each row
			var i = 0;
			foreach (var lookupRes in catResources)
			{
				// may resourcce definitions have spaces in the names
				// , which generates problematic field names
				//string clean = lookupRes.C.Clean();
				string clean = category.C;			// ie this part will now be Comm, Eqp, Library, Lab 

				string fieldK = $"Resource.{clean}.R.{i:00}.K";
				string fieldA = $"Resource.{clean}.D.{i:00}.A";
				string fieldNum = $"Resource.{clean}.D.{i:00}.Num";
				string fieldC = $"Resource.{clean}.D.{i:00}.C";

				PdfButtonFormField rgrpAvail = new RadioFormFieldBuilder(document.GetPdfDocument(), fieldA).CreateRadioGroup();
				PdfButtonFormField rgrpC = new RadioFormFieldBuilder(document.GetPdfDocument(), fieldC).CreateRadioGroup();

				cattable.AddRow(
					TextCell(model, $"{lookupRes.N}").Style(ss[TableBaseStyle]),
					YesCell(model, rgrpAvail),
					NoCell(model, rgrpAvail),
					NumberCell(model, fieldNum),
					SelectCell(model, rgrpC, "G"),
					SelectCell(model, rgrpC, "F"),
					SelectCell(model, rgrpC, "P")
				);
				i++;
			}
			document.Add(cattable);
			return document;
		}
	}
}
