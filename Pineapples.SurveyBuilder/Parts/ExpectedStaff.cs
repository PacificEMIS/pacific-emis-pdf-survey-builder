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
	public class ExpectedStaff
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			document.Add(new Paragraph()
				.Add(@"This list shows all the teachers recorded at your school in the last survey you submitted. "
				+ @"As well, it includes all teachers appointed to teach at your school in the current year. "
				+ @"To complete this table, answer Y in the On Staff? column to confirm that the teacher is working at your school. "
				+ @"Answer N otherwise. For teachers who are On Staff, review the remaining fields, and make any corrections required.")
			);	

			document.Add(new Paragraph()
				.Add(@"If there are teachers at your school who are not in this list, add their details in the next section – New Staff.")
			);


			//document.Add(table);

			return document;
		}
	}
}
