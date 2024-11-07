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

namespace surveybuilder
{
	public class Disabilities
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document, GenderedGridmaker grd, List<KeyValuePair<string, string>> disabilities)
		{
			document.Add(new Paragraph()
				.Add(new Text(@"Notes: ").AddStyle(new Style().SetBold()))
				.Add(@"This section provides definitions related to information in the table below. "
				+ @"This tool helps to identify students with functional difficulties or indications of disability; "
				+ @"it is not designed to provide a disability diagnosis.If the child identified with indicators of a disability has not previously "
				+ @"met with a doctor about their condition, work with the family to ensure that the child visits a village / clinic nurse or doctor "
				+ @"for further advice.")
			);

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 40, 60 }));
			table.SetMarginTop(5);

			table.AddCell(new Paragraph(new Text(@"Type of functional difficulty or disability").AddStyle(new Style().SetBold())));
			table.AddCell(new Paragraph(new Text(@"Description").AddStyle(new Style().SetBold())));
			table.AddCell("Blind");
			table.AddCell("Total absence of vision.");
			table.AddCell("Seeing difficulty");
			table.AddCell("Difficulty seeing things close up or far away, like objects, faces or pictures. "
				+ @"If a student wears glasses that correct their vision, they are not classified as having seeing difficulty.");
			table.AddCell("Deaf");
			table.AddCell("Total loss of hearing");
			table.AddCell("Hearing difficulty");
			table.AddCell("Partial loss of hearing; difficulty hearing sounds like peoples’ voices or music.");

			table.AddCell("Behaviour / Concentration / Socialisation / Hyperactive");
			table.AddCell("Difficulty controlling his / her own behaviour, and/or focusing and concentrating, "
			+ "and / or accepting changes in routine, and / or making friends.");

			table.AddCell("Speaking");
			table.AddCell("Difficulty being understood when speaking (in the language that is most usual for the student)");

			table.AddCell("Gross motor");
			table.AddCell("Difficulty walking or climbing stairs");

			table.AddCell("Fine motor");
			table.AddCell("Difficulty using hands and fingers, such as picking up small objects");

			table.AddCell("Learning / intellectual");
			table.AddCell("Difficulty with general intellectual functions such as learning and remembering");

			table.AddCell("Specific learning disability (dyslexia)");
			table.AddCell("Restrictions in one or a few specific activities related to writing, spelling, "
			+ "understanding, or reading, including decoding and comprehension.");

			table.AddCell("Multiple difficulties / disabilities");
			table.AddCell("The student has more than one of the above types of difficulties");

			document.Add(table);

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
