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
	public class PSA
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document, GenderedGridmaker grd)
		{
			document.Add(new Paragraph()
				.Add(@"Record the number of pupils starting class 1 this year (ie. not repeaters) who have attended Pre-School "
				+ @"for at least 100 days, or half days, during the two years before they started Class 1. Obtain the "
				+ @"information from the pupil's parents.")
			);

			grd.Tag = "PSA";
			grd.RowTotals = false;
			grd.IncludeFirstColumn = true;
			document.Add(grd.Make(builder));

			return document;
		}
	}
}
