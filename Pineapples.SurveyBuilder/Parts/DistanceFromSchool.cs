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
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class DistanceFromSchool
	{
		PdfStylesheet ss;
		public Document Build(PdfBuilder builder, Document document, LookupList distanceCodes)
		{
			Console.WriteLine("Part: Distance from School");

			// Import common table styles
			ss = builder.stylesheet;
			PdfTableStylesheet ts = new PdfTableStylesheet(ss);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model13 = CellStyleFactory.ThreeColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;

			document.Add(new Paragraph(@"Record all pupils according to the distance they have to travel to reach the school "
			+ "and their means of transport."));

			Table table = CellStyleFactory.DefaultTable(40, 20, 20, 20);

			// first row of headings
			table.AddRow(ts.TableHeaderStyle,
				TextCell(model21, "Distance"),
				TextCell(model13, "Number of Pupils")
			);
			// second row of headings

			table.AddRow(ts.TableHeaderStyle,
				TextCell(model, "On Foot"),
				TextCell(model, "Transport"),
				TextCell(model, "Total")
			);
			

			// data rows
			int i = 0;
			foreach (var item in distanceCodes)
			{
				table.AddRow(
					TextCell(model, item.N).Style(ts.TableHeaderStyle),
					GridCell(model, $"DT.D.{i:00}.00.All"),
					GridCell(model, $"DT.D.{i:00}.01.All"),
					TotalCell(model, $"DT.T.{i:00}.T.All", "").Style(ss["tabletotal"])
				);
				i++;
			}

			// Totals row
			table.AddRow(ts.TableHeaderStyle,
				TextCell(model,"Total")
			);

			for (int j = 0; j < 2; j++)
			{
				// TODO - Add support for read only NumberCell (See gendered grid)
				table.AddRow(ss["tabletotal"],
					TotalCell(model, $"DT.T.T.{j:00}.All")
				);
			}
			table.AddRow(ss["tabletotal"],
				TotalCell(model, $"DT.T.T.T.All")
			);

			document.Add(table);
			return document;
		}
	}
}
