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
using iText.Kernel.Colors;
using static System.Net.Mime.MediaTypeNames;
using surveybuilder.Utilities;
using iText.Kernel.Pdf;


namespace surveybuilder
{
	public class EnrolmentGrid
	{
		public EnrolmentGrid()
		{

		}
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Enrolment Grid");
			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

			document.Add(new Paragraph(@"Record the number of pupils enrolled at your school this year according to their age, class level and gender. "
+ @"Age is at 31 March this year."));

			GenderedGridmaker grd = new GenderedGridmaker();
			grd.Rows = builder.lookups["ages"];
			grd.Columns = builder.lookups["classLevels"];

			grd.Tag = "Enrol";
			document.Add(grd.Make(builder, document));

			RequiredFields flds = new RequiredFields("Enrolment",
				"No enrolments are recorded. Review now?"
				);
			flds.Add("Enrol.T.T.T.T");
			flds.GenerateJavaScript(document.GetPdfDocument());
			return document;
		}
	}
}

