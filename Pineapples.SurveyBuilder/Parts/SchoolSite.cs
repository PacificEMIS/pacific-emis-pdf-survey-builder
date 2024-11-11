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

namespace surveybuilder
{
	public class SchoolSite
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			var model = new Cell()
				.SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);

			document.Add(new Paragraph()
				.Add(@"Record the size of your school site, playground and farm or garden. The unit of measure should be square metres. "
				+ @"Write ""0"" if no space is allocated to a playground, farm or garden.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 20 }))
						.UseAllAvailableWidth();

			table.AddRow(
				TextCell(model, "Size of pupil playground (m²)"),
				NumberCell(model, "Site.Playground.Size")
			);
			table.AddRow(
				TextCell(model, "Size of school farm or garden (m²)"),
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
			PdfButtonFormField rgrp2 = new RadioFormFieldBuilder(builder.pdfDoc, "Site.Services")
				.CreateRadioGroup();

			table2.AddRow(
				TextCell(model, ""),
				TextCell(model, "Yes"),
				TextCell(model, "No")
			);
			table2.AddRow(
				TextCell(model, "School Site is securely fenced (Y / N)"),
				YesCell(model, rgrp1),
				NoCell(model, rgrp1)
			);
			table2.AddRow(
				TextCell(model, "In the area in which your school is located, is there access to piped water, "
				+ "town power and waste disposal services (town sever, septic pump out and garbage collection)?"),
				YesCell(model, rgrp2),
				NoCell(model, rgrp2)
			);

			document.Add(table2);

			return document;
		}
	}
}