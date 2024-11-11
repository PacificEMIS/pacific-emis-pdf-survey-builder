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
	public class NewStaff
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			document.Add(new Paragraph()
				.Add(@"Complete the following table for every teacher that is teaching at your school who is not listed in the previous table.")
			);

			//document.Add(table);

			return document;
		}
	}
}
