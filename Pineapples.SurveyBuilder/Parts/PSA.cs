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
	/// <summary>
	/// PSA (Preschool Attenders) lists new entrants into firts year of primary
	/// who have attended preschool. Columns are Age and Gender.
	/// This grid is unique in allowing the Row header to be editable.
	/// (EditRowValue parameter is true.)
	/// Also the row value and row key are passed as Null to facilitate this
	/// (see stored proc dbo.xfdfGrid for handling of rowdef = '<?>'
	/// Note that the setting of properties in metaPupilTable items is critical
	/// </summary>
	public class PSA
	{

		public Document Build(PdfBuilder builder, Document document)
		{

			GenderedGridmaker grd = new GenderedGridmaker();
			var rows = new LookupList();
			rows.Add(new LookupEntry("PSA", "Attended Preschool"));
			grd.Rows = rows;

			
			var ages = new LookupList();

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
			grd.RowTotals = true;
			document.Add(grd.Make(builder, document));

			return document;
		}
	}
}
