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
using itext4.Utilities;

namespace surveybuilder
{
	public class WASHSanitation
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public WASHSanitation() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document, List<LookupEntry> toiletTypes)
		{
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
			tableToilets.AddRow(
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Toilet Type"))),
				ts.TableHeaderStyle(TextCell(model12, ts.TableHeaderStyle("No of Pupil Toilets"))),
				ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Condition"))),
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("No of Staff Toilets"))),
				ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Condition"))),
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("No of Wheelchair Accessible Toilets"))),
				ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Condition")))
			);

			tableToilets.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("M"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("F"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("G"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("F"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("P"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("G"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("F"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("P"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("G"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("F"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("P")))
			);

			// Rows
			var ttI = 0;
			foreach (var toiletType in toiletTypes)
			{
				// TODO need to include the field key
				string fieldK = $"Toilets.R.{ttI:00}.K";
				string fieldV = $"Toilets.R.{ttI:00}.V";
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

				tableToilets.AddRow(
					TextCell(model, ts.TableBaseStyle(toiletType.N)), // fieldK/fieldV missing
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

				ttI++;

			}

			document.Add(tableToilets);

			return document;
		}
	}
}
