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
using Colors = iText.Kernel.Colors;
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class JointTeacherClasses
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public Document Build(PdfBuilder builder, Document document)
		{
			// Cell layout/styling models
			var model = CellStyleFactory.CreateCell(rowSpan: 1, colSpan: 1, height: 18);
			var model12 = CellStyleFactory.CreateCell(rowSpan: 1, colSpan: 2, height: 18);
			var model13 = CellStyleFactory.CreateCell(rowSpan: 1, colSpan: 3, height: 18);
			var model21 = CellStyleFactory.CreateCell(rowSpan: 2, colSpan: 1, height: 18);
			var model21b = CellStyleFactory.CreateCell(rowSpan: 2, colSpan: 1, height: 40);

			document.Add(new Paragraph(@"Enter the employment number and name of each teacher teaching a class where two teachers share the "
			+ @"teaching of one class. Enter in the box opposite their name and the number of pupils enrolled in the "
			+ @"class they teach."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 4, 4, 3, 3, 3, 3, 3, 3 }))
						.UseAllAvailableWidth();

			// first row of headings
			table.AddCell(ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Employment No"))));
			table.AddCell(ts.TableHeaderStyle(TextCell(model12, ts.TableHeaderStyle("Teacher Name"))));

			for (int j = 0; j < 6; j++)
			{
				table.AddCell(ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle($"Class {j + 1:0}"))));
			}

			// second row of headings
			table.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Given Name"))));
			table.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Family Name"))));

			// data rows
			for (int i = 0; i <= 20; i++)
			{
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T0.ID", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T0.Given", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T0.Family", 50));

				for (int j = 0; j < 6; j++)
				{
					table.AddCell(NumberCell(model21b, $"ClassJ.D.{i:00}.{j:00}.All"));
				}

				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T1.ID", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T1.Given", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T1.Family", 50));

			}

			// Totals
			table.AddCell(ts.TableHeaderStyle(TextCell(model13, ts.TableRowHeaderTotalStyle("Total Pupils"))));

			for (int j = 0; j < 6; j++)
			{
				// TODO - Add support for read only NumberCell (see gendered gridmaker)
				table.AddCell(NumberCell(model, $"ClassJ.T.T.{j:00}.All"));
			}

			document.Add(table);
			return document;
		}
	}
}
