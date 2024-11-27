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
		public Document Build(PdfBuilder builder, Document document)
		{

			GenderedGridmaker grd = new GenderedGridmaker();
			//var rows = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }
			//				.Select(n => new KeyValuePair<string, string>(n, n))
			//				.ToList();
			var rows = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }
							.Select(n => new LookupEntry
							{
								C = n, // Set the primary code (C)
								N = n  // Set the primary name (N)
							})
							.ToList();

			grd.Rows = rows;

			//var ages = new List<KeyValuePair<string, string>>();

			//ages.Add(new KeyValuePair<string, string>("5", "5"));
			//ages.Add(new KeyValuePair<string, string>("6", "6"));
			//ages.Add(new KeyValuePair<string, string>("7", "7"));
			//ages.Add(new KeyValuePair<string, string>("8", "8"));
			var ages = new List<LookupEntry>();

			ages.Add(new LookupEntry { C = "5", N = "5" });
			ages.Add(new LookupEntry { C = "6", N = "6" });
			ages.Add(new LookupEntry { C = "7", N = "7" });
			ages.Add(new LookupEntry { C = "8", N = "8" });

			grd.Columns = ages;

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
