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
	public class SchoolSupplies
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			document.Add(new Paragraph()
				.Add(@"Did you receive all of the school supplies for students that were sent by MoE head office at the start of "
				+ @"this year? You should have received enough for 1 set per student.Tick the box below that indicates the "
				+ @"amount you received.")
			);			

			//document.Add(table);

			return document;
		}
	}
}
