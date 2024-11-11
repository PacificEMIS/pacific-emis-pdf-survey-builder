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
	public class SchoolResources
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			document.Add(new Paragraph()
				.Add(@"Record whether each of the following resources are available at your school, the number available and "
				+ @"the condition they are in.")
			);			

			//document.Add(table);

			return document;
		}
	}
}
