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
using itext4.Utilities;

namespace surveybuilder
{
	public class Disabilities
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public Document Build(KEMIS_PRI_Builder builder, Document document, GenderedGridmaker grd, List<LookupEntry> disabilities)
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
				ts.TableHeaderStyle(TextCell(model, new Paragraph(new Text(@"Type of functional difficulty or disability").AddStyle(new Style().SetBold())))),
				ts.TableHeaderStyle(TextCell(model, new Paragraph(new Text(@"Description").AddStyle(new Style().SetBold()))))
			);

			table.AddRow(
				TextCell(model, "Blind"),
				TextCell(model, "Total absence of vision.")
			);

			table.AddRow(
				TextCell(model, "Seeing difficulty"),
				TextCell(model, "Difficulty seeing things close up or far away, like objects, faces or pictures. "
				+ @"If a student wears glasses that correct their vision, they are not classified as having seeing difficulty.")
			);

			table.AddRow(
				TextCell(model, "Deaf"),
				TextCell(model, "Total loss of hearing")
			);

			table.AddRow(
				TextCell(model, "Hearing difficulty"),
				TextCell(model, "Partial loss of hearing; difficulty hearing sounds like peoples’ voices or music.")
			);

			table.AddRow(
				TextCell(model, "Behaviour / Concentration / Socialisation / Hyperactive"),
				TextCell(model, "Difficulty controlling his / her own behaviour, and/or focusing and concentrating, "
			+ "and / or accepting changes in routine, and / or making friends.")
			);

			table.AddRow(
				TextCell(model, "Speaking"),
				TextCell(model, "Difficulty being understood when speaking (in the language that is most usual for the student)")
			);

			table.AddRow(
				TextCell(model, "Gross motor"),
				TextCell(model, "Difficulty walking or climbing stairs")
			);

			table.AddRow(
				TextCell(model, "Fine motor"),
				TextCell(model, "Difficulty using hands and fingers, such as picking up small objects")
			);

			table.AddRow(
				TextCell(model, "Learning / intellectual"),
				TextCell(model, "Difficulty with general intellectual functions such as learning and remembering")
			);

			table.AddRow(
				TextCell(model, "Specific learning disability (dyslexia)"),
				TextCell(model, "Restrictions in one or a few specific activities related to writing, spelling, "
			+ "understanding, or reading, including decoding and comprehension.")
			);

			table.AddRow(
				TextCell(model, "Multiple difficulties / disabilities"),
				TextCell(model, "The student has more than one of the above types of difficulties")
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
