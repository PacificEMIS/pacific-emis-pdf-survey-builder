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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Colors = iText.Kernel.Colors;
using surveybuilder.Utilities;

namespace surveybuilder
{
	public class SingleTeacherClasses
	{
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Single Teacher Classes");

			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);

			// Cell layout/styling models
			var model = CellStyleFactory.CreateCell(rowSpan: 1, colSpan: 1, height: 18);
			var model12 = CellStyleFactory.TwoColumn;
			var model13 = CellStyleFactory.ThreeColumn;
			var model21 = CellStyleFactory.CreateCell(rowSpan: 2, colSpan: 1, height: 18);

			document.Add(new Paragraph(@"Enter the employment number and name of each teacher teaching a single-teacher class group. Enter "
			+ @"the number of pupils enrolled at class level in the group. If all pupils in the class are at the same class "
			+ @"level, enter the enrolment in the appropriate column. If the class group contains pupils at different "
			+ @"levels, enter the number at each level in the appropriate column."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 4, 4, 3, 3, 3, 3, 3, 3 }))
						.UseAllAvailableWidth();

			table.AddCell(ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Employment No"))));
			table.AddCell(ts.TableHeaderStyle(TextCell(model12, ts.TableHeaderStyle("Teacher Name"))));

			for (int j = 0; j < 6; j++)
			{
				table.AddCell(ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle($"Class {j + 1:0}"))));
			}

			// second row of headings
			table.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Given Name"))));
			table.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Family Name"))));

			for (int i = 0; i <= 29; i++)
			{
				table.AddCell(InputCell(model, $"Class.D.{i:00}.T0.ID", 50));
				table.AddCell(InputCell(model, $"Class.D.{i:00}.T0.Given", 50));
				table.AddCell(InputCell(model, $"Class.D.{i:00}.T0.Family", 50));

				for (int j = 0; j < 6; j++)
				{
					table.AddCell(NumberCell(model, $"Class.D.{i:00}.{j:00}.All"));
				}
			}

			// Totals
			table.AddCell(ts.TableHeaderStyle(TextCell(model13, ts.TableRowHeaderTotalStyle("Total Pupils"))));

			for (int j = 0; j < 6; j++)
			{
				// TODO - Add support for read only NumberCell (see gendered grid)
				table.AddCell(NumberCell(model, $"Class.T.T.{j:00}.All"));
			}

			document.Add(table);
			return document;
		}
	}
}
