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
		public Document Build(PdfBuilder builder, Document document, LookupList distanceCodes)
		{
			Console.WriteLine("Part: Distance from School");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model13 = CellStyleFactory.ThreeColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;

			document.Add(new Paragraph(@"Record all pupils according to the distance they have to travel to reach the school "
			+ "and their means of transport."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 40, 20, 20, 20 }))
						.UseAllAvailableWidth();

			// first row of headings
			table.AddRow(
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Distance"))),
				ts.TableHeaderStyle(TextCell(model13, ts.TableHeaderStyle("Number of Pupil")))
			);
			table.AddRow(
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("On Foot"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Transport"))),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Total")))
			);
			// second row of headings


			// data rows
			int i = 0;
			foreach (var item in distanceCodes)
			{
				table.AddRow(
					TextCell(model, ts.TableRowHeaderStyle(item.N)),
					NumberCell(model, $"DT.D.{i:00}.00.All"),
					NumberCell(model, $"DT.D.{i:00}.01.All"),
					NumberCell(model, $"DT.T.{i:00}.T.All")
				);
				i++;
			}

			// Totals
			table.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableRowHeaderTotalStyle("Total"))));

			for (int j = 0; j < 3; j++)
			{
				// TODO - Add support for read only NumberCell (See gendered grid)
				table.AddCell(NumberCell(model, $"DT.T.T.{j:00}.All"));
			}

			document.Add(table);
			return document;
		}
	}
}
