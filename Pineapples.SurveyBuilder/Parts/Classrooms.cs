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
using iText.Kernel.Colors;
using static System.Net.Mime.MediaTypeNames;
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class Classrooms
	{
		
		public Classrooms()
		{

		}
		public Document Build(PdfBuilder builder, Document document)
		{
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model13 = CellStyleFactory.ThreeColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;

			// Classrooms
			document.Add(new Paragraph()
				.Add(@"How many classrooms are there at your school? Count all rooms in which tuition takes place.")
			);


			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 }))
						.UseAllAvailableWidth();

			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Total number of classrooms")),
				NumberCell(model, "Survey.ClassroomCount")
			);

			document.Add(table);

			// Construction material

			document.Add(new Paragraph()
				.Add(@"Show the number of rooms for each type of Construction Material: i.e. Permanent, Semi-permanent, or Traditional. "
				+ @"For each Material, indicate whether classrooms are in 'Good', 'Fair' or 'Poor' condition by selecting the appropriate box.")
			);

			PdfButtonFormField rgrpP = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.ClassroomCountP.C")
				.CreateRadioGroup();
			PdfButtonFormField rgrpSP = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.ClassroomCountSP.C")
				.CreateRadioGroup();
			PdfButtonFormField rgrpT = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.ClassroomCountT.C")
				.CreateRadioGroup();

			Table tableMaterial = new Table(UnitValue.CreatePercentArray(new float[] { 40, 30, 10, 10, 10 }))
						.UseAllAvailableWidth();

			tableMaterial.AddRow(
					ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Material"))),
					ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Number of Rooms"))),
					ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Overall Condition")))
				);

			tableMaterial.AddRow(
					ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Good"))),
					ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Fair"))),
					ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Poor")))
				);

			// Could dynamically generate types of material...

			tableMaterial.AddRow(
					TextCell(model, ts.TableRowHeaderStyle("Permanent")),
					NumberCell(model, "Survey.ClassroomCountP.Num"),
					SelectCell(model, rgrpP, "G"),
					SelectCell(model, rgrpP, "F"),
					SelectCell(model, rgrpP, "P")
				);

			tableMaterial.AddRow(
					TextCell(model, ts.TableRowHeaderStyle("Semi-Permanent")),
					NumberCell(model, "Survey.ClassroomCountSP.Num"),
					SelectCell(model, rgrpSP, "G"),
					SelectCell(model, rgrpSP, "F"),
					SelectCell(model, rgrpSP, "P")
				);

			tableMaterial.AddRow(
					TextCell(model, ts.TableRowHeaderStyle("Traditional")),
					NumberCell(model, "Survey.ClassroomCountT.Num"),
					SelectCell(model, rgrpT, "G"),
					SelectCell(model, rgrpT, "F"),
					SelectCell(model, rgrpT, "P")
				);

			document.Add(tableMaterial);

			RequiredFields req = new RequiredFields("Classroom Count",
					"You have not recorded the total number of classrooms.");
					req.Add("Survey.ClassroomCount");
			ValidationManager.AddRequiredFields(builder.pdfDoc, req);

			ConditionalFields conditionalFields = new ConditionalFields("Classrooms",
				"Classrooms are missing Material information."
			);
			var cf = ConditionalField.IfAnyAlternatives("Survey.ClassroomCount",
				"Survey.ClassroomCountP.Num", "Survey.ClassroomCountSP.Num", "Survey.ClassroomCountT.Num");
	
			conditionalFields.Add(cf);
			ValidationManager.AddConditionalFields(builder.pdfDoc, conditionalFields);

			conditionalFields = new ConditionalFields("ClassroomCondition",
				"Classrooms are missing Condition information."
				);	
			conditionalFields.Add(ConditionalField.IfAny("Survey.ClassroomCountT.Num",
				"Survey.ClassroomCountT.C"));
			conditionalFields.Add(ConditionalField.IfAny("Survey.ClassroomCountP.Num",
				"Survey.ClassroomCountP.C"));
			conditionalFields.Add(ConditionalField.IfAny("Survey.ClassroomCountSP.Num",
				"Survey.ClassroomCountSP.C"));
			ValidationManager.AddConditionalFields(builder.pdfDoc,conditionalFields);
			return document;
		}
	}
}
