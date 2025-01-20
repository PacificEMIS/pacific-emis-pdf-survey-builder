﻿using iText.Forms.Fields;
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
	public class WASHSanitation
	{
		public WASHSanitation() { }

		public Document Build(PdfBuilder builder, Document document, LookupList toiletTypes)
		{
			Console.WriteLine("Part: Wash - Sanitation");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

			// validations
			const string conditionalMsg = "You need to specify toilet condition";
			ConditionalFields conditionalFields = new ConditionalFields("WashSanitation", conditionalMsg);
			const string requiredMsg = "Some answers are missing from Wash Sanitation.";
			RequiredFields requiredFields = new RequiredFields("WashSanitation", requiredMsg);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model12 = CellStyleFactory.CreateCell(rowSpan: 1, colSpan: 2, applyHeight: false);
			var model13 = CellStyleFactory.ThreeColumn;
			var model21 = CellStyleFactory.CreateCell(rowSpan: 2, colSpan: 1, applyHeight: false);

			// Toilets
			document.Add(builder.Heading_3("Toilets"));

			document.Add(new Paragraph()
				.Add(@"Record the details of your school’s toilet facilities for staff and pupils.")
			);

			document.Add(new Paragraph()
				.Add(@"Note: Under Condition columns 'G' = 'Good', 'F' = 'Fair', 'P' = 'Poor'")
			);

			Table tableToilets = new Table(UnitValue.CreatePercentArray(new float[] { 19, 6, 6, 5, 5, 5, 12, 5, 5, 5, 12, 5, 5, 5 }))
						.UseAllAvailableWidth();

			// Heading
			tableToilets.AddRow(ts.TableHeaderStyle,
				TextCell(model21, "Toilet Type"),
				TextCell(model12, "No of Pupil Toilets"),
				TextCell(model13, "Condition"),
				TextCell(model21, "No of Staff Toilets"),
				TextCell(model13, "Condition"),
				TextCell(model21, "No of Wheelchair Accessible Toilets"),
				TextCell(model13, "Condition")
			);

			tableToilets.AddRow(ts.TableHeaderStyle,
				TextCell(model, "M"),
				TextCell(model, "F"),
				TextCell(model, "G"),
				TextCell(model, "F"),
				TextCell(model, "P"),
				TextCell(model, "G"),
				TextCell(model, "F"),
				TextCell(model, "P"),
				TextCell(model, "G"),
				TextCell(model, "F"),
				TextCell(model, "P")
			);

			// Rows
			var ttI = 0;
			foreach (var toiletType in toiletTypes)
			{
				string fieldDPupilM = $"Toilets.D.{ttI:00}.Pupil.M";
				string fieldDPupilF = $"Toilets.D.{ttI:00}.Pupil.F";
				string fieldDPupilC = $"Toilets.D.{ttI:00}.Pupil.C";
				string fieldDStaff = $"Toilets.D.{ttI:00}.Staff.All";
				string fieldDStaffC = $"Toilets.D.{ttI:00}.Staff.C";
				string fieldDWheelchair = $"Toilets.D.{ttI:00}.Wheelchair.All";
				string fieldDWheelchairC = $"Toilets.D.{ttI:00}.Wheelchair.C";

				PdfButtonFormField rgrpPupilToiletC = new RadioFormFieldBuilder(builder.pdfDoc, fieldDPupilC).CreateRadioGroup();
				PdfButtonFormField rgrpStaffToiletC = new RadioFormFieldBuilder(builder.pdfDoc, fieldDStaffC).CreateRadioGroup();
				PdfButtonFormField rgrpWheelchairToiletC = new RadioFormFieldBuilder(builder.pdfDoc, fieldDWheelchairC).CreateRadioGroup();
				rgrpPupilToiletC.SetAlternativeName("Pupil toilets condition");
				rgrpStaffToiletC.SetAlternativeName("Staff toilets condition");
				rgrpWheelchairToiletC.SetAlternativeName("Wheelchair toilets condition");

				tableToilets.AddRow(
					TextCell(model, ts.TableBaseStyle(toiletType.N)),
					NumberCell(model, fieldDPupilM),
					NumberCell(model, fieldDPupilF),
					SelectCell(model, rgrpPupilToiletC, "G"),
					SelectCell(model, rgrpPupilToiletC, "F"),
					SelectCell(model, rgrpPupilToiletC, "P"),
					NumberCell(model, fieldDStaff),
					SelectCell(model, rgrpStaffToiletC, "G"),
					SelectCell(model, rgrpStaffToiletC, "F"),
					SelectCell(model, rgrpStaffToiletC, "P"),
					NumberCell(model, fieldDWheelchair),
					SelectCell(model, rgrpWheelchairToiletC, "G"),
					SelectCell(model, rgrpWheelchairToiletC, "F"),
					SelectCell(model, rgrpWheelchairToiletC, "P")
				);

				conditionalFields.Add(
					ConditionalField.IfAny(
					$"Toilets.D.{ttI:00}.Pupil.M",
					$"Toilets.D.{ttI:00}.Pupil.C"
				));
				conditionalFields.Add(
					ConditionalField.IfAny(
					$"Toilets.D.{ttI:00}.Pupil.F",
					$"Toilets.D.{ttI:00}.Pupil.C"
				));
				conditionalFields.Add(
					ConditionalField.IfAny(
					$"Toilets.D.{ttI:00}.Staff.All",
					$"Toilets.D.{ttI:00}.Staff.C"
				));
				conditionalFields.Add(
					ConditionalField.IfAny(
					$"Toilets.D.{ttI:00}.Wheelchair.All",
					$"Toilets.D.{ttI:00}.Wheelchair.C"
				));

				ttI++;

			}

			toiletTypes.AsFields(document.GetPdfDocument()
				, (i) => $"Toilets.R.{i:00}.K", (i) => $"Toilets.R.{i:00}.V");

			document.Add(tableToilets);

			ValidationManager.AddConditionalFields(document.GetPdfDocument(), conditionalFields);
			return document;
		}
	}
}
