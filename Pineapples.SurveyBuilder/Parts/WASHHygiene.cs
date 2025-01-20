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
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class WASHHygiene
	{
		public WASHHygiene() { }

		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Wash - Hygiene");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			// Cell layout/styling models
			var model = CellStyleFactory.Default;

			// validations
			const string conditionalMsg = "Some answers are missing from Wash Hygiene.";
			ConditionalFields conditionalFields = new ConditionalFields("WashHygiene", conditionalMsg);
			const string requiredMsg = "Some answers are missing from Wash Hygiene.";
			RequiredFields requiredFields = new RequiredFields("WashHygiene", requiredMsg);

			document.Add(new Paragraph()
				.Add(@"What type of handwashing station is provided at the school.")
			);

			var chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Stand tap", "Tippy Tap", "Piped Water", "Basin With Tap", "None" };
			chk.Values = new object[] { "Stand_Tap","Tippy_Tap","Piped","Basin_Tap","None" };
			chk.Types = new CheckBoxType[] {CheckBoxType.SQUARE, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.CROSS };
			chk.Tag = "Wash.Handwashing.Type";
			chk.Description = "Type of handwashing station";

			chk.Make(builder, document);
			requiredFields.Add(chk.Tag);

			document.Add(new Paragraph()
				.Add(@"Are there both water and soap available at the handwashing facilities.")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Soap and water", "Water only", "Soap Only", "Neither" };
			chk.Values = new object[] { "Soap_Water", "Water", "Soap", "None" };
			chk.Tag = "Wash.Handwashing.SoapAndWater";
			chk.Description = "Soap and/or water available";

			chk.Make(builder, document);
			requiredFields.Add(chk.Tag);

			document.Add(new Paragraph()
				.Add(@"Answer the following Yes/No questions.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrp1 = new RadioFormFieldBuilder(builder.pdfDoc, "Wash.Handwashing.Practiced")
				.CreateRadioGroup();
			
			PdfButtonFormField rgrp2 = new RadioFormFieldBuilder(builder.pdfDoc, "Wash.Toothbrushing.Practiced")
				.CreateRadioGroup();
			rgrp1.SetAlternativeName("Handwashing practiced");
			rgrp2.SetAlternativeName("Toothbrushing practiced");

			table.AddRow(ts.TableHeaderStyle,
				TextCell(model, String.Empty),
				TextCell(model, "Yes"),
				TextCell(model, "No")
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Is hand washing practiced at the school?")),
				YesCell(model, rgrp1),
				NoCell(model, rgrp1)
			);
			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Is toothbrushing practiced at the school?")),
				YesCell(model, rgrp2),
				NoCell(model, rgrp2)
			);
			requiredFields.Add(rgrp1.GetFieldName().ToString());
			requiredFields.Add(rgrp2.GetFieldName().ToString());

			document.Add(table);


			document.Add(new Paragraph()
				.Add(@"What additional facilities are provide in the toilets at the school ?")
			);

			Table table2 = new Table(UnitValue.CreatePercentArray(new float[] { 80, 10, 10 }))
						.UseAllAvailableWidth();

			PdfButtonFormField rgrpF1 = new RadioFormFieldBuilder(builder.pdfDoc, "Toilets.HasShower")
				.CreateRadioGroup();
			PdfButtonFormField rgrpF2 = new RadioFormFieldBuilder(builder.pdfDoc, "Toilets.HasMenstrualHygiene")
				.CreateRadioGroup();
			PdfButtonFormField rgrpF3 = new RadioFormFieldBuilder(builder.pdfDoc, "Toilets.HasRubbishTin")
				.CreateRadioGroup();
			rgrpF1.SetAlternativeName("Shower room in student toilets");
			rgrpF2.SetAlternativeName("Space for menstrual hygiene");
			rgrpF3.SetAlternativeName("Rubbish tin with lid in toilets");

			table2.AddRow(ts.TableHeaderStyle,
				TextCell(model, ""),
				TextCell(model, "Yes"),
				TextCell(model, "No")
			);
			table2.AddRow(
				TextCell(model, ts.TableBaseStyle("Shower room in student toilets")),
				YesCell(model, rgrpF1),
				NoCell(model, rgrpF1)
			);
			table2.AddRow(
				TextCell(model, ts.TableBaseStyle("Space for menstrual hygiene in girls’ toilets")),
				YesCell(model, rgrpF2),
				NoCell(model, rgrpF2)
			);
			table2.AddRow(
				TextCell(model, ts.TableBaseStyle("Rubbish tin with lid in toilets ")),
				YesCell(model, rgrpF3),
				NoCell(model, rgrpF3)
			);

			requiredFields.Add(rgrpF1.GetFieldName().ToString());
			requiredFields.Add(rgrpF2.GetFieldName().ToString());
			requiredFields.Add(rgrpF3.GetFieldName().ToString());

			document.Add(table2);
			
			ValidationManager.AddRequiredFields(document.GetPdfDocument(),requiredFields);
			return document;
		}
	}
}
