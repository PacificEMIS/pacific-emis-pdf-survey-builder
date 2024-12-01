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
	public class Disabilities
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public Document Build(PdfBuilder builder, Document document, GenderedGridmaker grd, LookupList disabilities)
		{
			// Cell layout/styling models
			var model = CellStyleFactory.DefaultNoHeight;

			document.Add(new Paragraph()
				.Add(new Text(@"Notes: ").AddStyle(new Style().SetBold()))
				.Add(@"This section provides definitions related to information in the table below. "
				+ @"This tool helps to identify students with functional difficulties or indications of disability; "
				+ @"it is not designed to provide a disability diagnosis.If the child identified with indicators of a disability has not previously "
				+ @"met with a doctor about their condition, work with the family to ensure that the child visits a village / clinic nurse or doctor "
				+ @"for further advice.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 }));
			table.SetMarginTop(5);

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Type of functional difficulty or disability"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Description")))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Blind")),
				TextCell(model, ts.TableBaseStyle("Total absence of vision."))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Seeing difficulty")),
				TextCell(model, ts.TableBaseStyle("Difficulty seeing things close up or far away, like objects, faces or pictures. "
				+ @"If a student wears glasses that correct their vision, they are not classified as having seeing difficulty."))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Deaf")),
				TextCell(model, ts.TableBaseStyle("Total loss of hearing"))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Hearing difficulty")),
				TextCell(model, ts.TableBaseStyle("Partial loss of hearing; difficulty hearing sounds like peoples’ voices or music."))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Behaviour / Concentration / Socialisation / Hyperactive")),
				TextCell(model, ts.TableBaseStyle("Difficulty controlling his / her own behaviour, and/or focusing and concentrating, "
			+ "and / or accepting changes in routine, and / or making friends."))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Speaking")),
				TextCell(model, ts.TableBaseStyle("Difficulty being understood when speaking (in the language that is most usual for the student)"))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Gross motor")),
				TextCell(model, ts.TableBaseStyle("Difficulty walking or climbing stairs"))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Fine motor")),
				TextCell(model, ts.TableBaseStyle("Difficulty using hands and fingers, such as picking up small objects"))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Learning / intellectual")),
				TextCell(model, ts.TableBaseStyle("Difficulty with general intellectual functions such as learning and remembering"))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Specific learning disability (dyslexia)")),
				TextCell(model, ts.TableBaseStyle("Restrictions in one or a few specific activities related to writing, spelling, "
			+ "understanding, or reading, including decoding and comprehension."))
			);

			table.AddRow(
				TextCell(model, ts.TableBaseStyle("Multiple difficulties / disabilities")),
				TextCell(model, ts.TableBaseStyle("The student has more than one of the above types of difficulties"))
			);

			document.Add(table);

			builder.NewPage(document);

			document.Add(new Paragraph(@"Record the number of children with a functional difficulty or a disability who are attending your school."
			+ @"Record each pupil only once. Record pupils with more than one type of difficulty or disability under the category "
			+ @"'Multiple difficulties / disabilities'."));

			grd.Tag = "DIS";
			grd.Rows = disabilities;
			document.Add(grd.Make(builder));

			return document;
		}
	}
}
