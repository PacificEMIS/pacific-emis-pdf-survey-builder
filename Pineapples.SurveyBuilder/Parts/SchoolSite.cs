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
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class SchoolSite
	{
		// Import common table styles
		
		public Document Build(PdfBuilder builder, Document document)
		{
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			Console.WriteLine("Part: School site");
			// Cell layout/styling models
			var model = CellStyleFactory.Default;

			document.Add(new Paragraph()
				.Add(@"Record the size of your school site, playground and farm or garden. The unit of measure should be square metres. "
				+ @"Write ""0"" if no space is allocated to a playground, farm or garden.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 20 }))
						.UseAllAvailableWidth();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle(""))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Size (m²)")))
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Size of whole school site (m²)")),
				NumberCell(model, "Site.Site.Size")
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Size of pupil playground (m²)")),
				NumberCell(model, "Site.Playground.Size")
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Size of school farm or garden (m²)")),
				NumberCell(model, "Site.Garden.Size")
			);

			document.Add(table);

			document.Add(new Paragraph()
				.Add(@"Record the following details about your school site.")
			);

			Table table2 = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrp1 = new RadioFormFieldBuilder(builder.pdfDoc, "Site.Secure")
				.CreateRadioGroup();
				rgrp1.SetAlternativeName("Secure fence");
			PdfButtonFormField rgrp2 = new RadioFormFieldBuilder(builder.pdfDoc, "Site.Services")
				.CreateRadioGroup();
				rgrp2.SetAlternativeName("Town services available"); ;

			table2.AddRow(ts.TableHeaderStyle,
				TextCell(model, ""),
				TextCell(model, "Yes"),
				TextCell(model, "No")
			);

			table2.AddRow(ts.TableBaseStyle,
				TextCell(model, "School Site is securely fenced"),
				YesCell(model, rgrp1),
				NoCell(model, rgrp1)
			);
			table2.AddRow(
				TextCell(model.SetMaxHeight(40), ts.TableBaseStyle("In the area in which your school is located, is there access to piped water, "
				+ "town power and waste disposal services (town sewer, septic pump out and garbage collection)?")),
				YesCell(model, rgrp2),
				NoCell(model, rgrp2)
			);

			document.Add(table2);
			new RequiredFields("School Site",
				"School site is missing information. Review now?"
				)
			.AddFields("Site.Secure", "Site.Services", "Site.Size", "Site.Playground.Size","Site.Garden.Size")
			.GenerateJavaScript(document.GetPdfDocument());
			return document;
		}
	}
}
