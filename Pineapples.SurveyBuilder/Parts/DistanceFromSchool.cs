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
	public class DistanceFromSchool
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document, List<KeyValuePair<string, string>> distanceCodes)
		{
			document.Add(new Paragraph(@"Record all pupils according to the distance they have to travel to reach the school "
			+ "and their means of transport."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 40, 20, 20, 20 }))
						.UseAllAvailableWidth();

			Cell model = new Cell().SetHeight(18);

			// first row of headings
			table.AddCell(TextCell(new Cell(2, 1), "Distance"));
			table.AddCell(TextCell(new Cell(1, 3), "Number of Pupil"));
			// second row of headings
			table.AddCell(TextCell(model, "On Foot"));
			table.AddCell(TextCell(model, "Transport"));
			table.AddCell(TextCell(model, "Total"));

			// data rows
			int i = 0;
			foreach (var item in distanceCodes)
			{
				table.AddCell(new Cell().Add(new Paragraph(item.Value)));
				table.AddCell(NumberCell(model, $"DT.D.{i:00}.00.All"));
				table.AddCell(NumberCell(model, $"DT.D.{i:00}.01.All"));
				table.AddCell(NumberCell(model, $"DT.T.{i:00}.T.All"));

				i++;
			}

			// Totals
			table.AddCell(TextCell(model, "Total"));

			for (int j = 0; j < 3; j++)
			{
				// TODO - Add support for read only NumberCell
				table.AddCell(NumberCell(model, $"DT.T.T.{j:00}.All"));
			}

			document.Add(table);
			return document;
		}
	}
}
