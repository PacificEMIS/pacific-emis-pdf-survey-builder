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
	public class TransfersIn
	{
		public Document Build(PdfBuilder builder, Document document, GenderedGridmaker grd, LookupList islands)
		{
			document.Add(new Paragraph()
				.Add(@"Record the number of pupils that transferred in from another islands "
				+ @"at the beginning of the school year.")
			);

			grd.Tag = "TRIN";
			grd.Rows = islands;
			document.Add(grd.Make(builder));

			return document;
		}
	}
}
