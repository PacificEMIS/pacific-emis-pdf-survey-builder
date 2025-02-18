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
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class SchoolSupplies
	{
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: School Supplies");
			;
			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			// Cell layout/styling models
			var model = CellStyleFactory.Default;

			document.Add(builder.Heading_3("School Supplies for Students"));

			document.Add(new Paragraph()
				.Add(@"Did you receive all of the school supplies for students that were sent by MoE head office at the start of "
				+ @"this year? You should have received enough for 1 set per student.Tick the box below that indicates the "
				+ @"amount you received.")
			);

			var chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "All Received", "Most Received","Half Received","Some Received", "None Received" };
			chk.Values = new object[] { "All", "Most", "Half", "Some", "None" };
			chk.Types = new CheckBoxType[] {CheckBoxType.SQUARE, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.CROSS };
			chk.Tag = "Supplies.Student";
			chk.Description = "Student supplies received";
			chk.Make(builder, document);

			document.Add(builder.Heading_3("School Supplies for Teachers"));

			document.Add(new Paragraph()
				.Add(@"Did you receive all of the school supplies for Teachers that were sent by MoE head office at the start of "
				+ @"this year? You should have received enough for 1 set per teacher.Tick the box below that indicates the "
				+ @"amount you received.")
			);

			chk.Names = new string[] { "All Received", "Most Received", "Half Received", "Some Received", "None Received" };
			chk.Values = new object[] { "All", "Most", "Half", "Some", "None" };
			chk.Types = new CheckBoxType[] {CheckBoxType.SQUARE, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.CROSS };
			chk.Tag = "Supplies.Teacher";
			chk.Description = "Teacher supplies received";
			chk.Make(builder, document);

			document.Add(builder.Heading_3("New Curriculum Materials"));

			document.Add(new Paragraph()
				.Add(@"Did you receive all of the school supplies for Teachers that were sent by MoE head office at the start of "
				+ @"this year? You should have received enough for 1 set per teacher.Tick the box below that indicates the "
				+ @"amount you received.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrp1 = new RadioFormFieldBuilder(builder.pdfDoc, "Supplies.Curriculum")
				.CreateRadioGroup();
			PdfButtonFormField rgrp2 = new RadioFormFieldBuilder(builder.pdfDoc, "Supplies.InUse")
				.CreateRadioGroup();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle(""))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Yes"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("No")))
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Have you received new curriculum materials for Years 1 & 2?")),
				YesCell(model, rgrp1),
				NoCell(model, rgrp1)
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("If YES, have you started using them in the classroom?")),
				YesCell(model, rgrp2),
				NoCell(model, rgrp2)
			);

			document.Add(table);

			string msg = "School Supplies information is not complete.";
			var requiredFields = new RequiredFields("School Supplies",
				msg
				)
			.Add("Supplies.Student", "Supplies.Teacher", "Supplies.Curriculum");
			ValidationManager.AddRequiredFields(document.GetPdfDocument(), requiredFields);

			var cf = new ConditionalFields("School Supplies",
				msg
				);
			cf.Add(ConditionalField.IfYes("Supplies.Curriculum", "Supplies.InUse"));
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), cf);

			return document;
		}
	}
}
