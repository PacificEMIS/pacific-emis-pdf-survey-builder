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
using itext4.Utilities;

namespace surveybuilder
{
	public class SingleTeacherClasses
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{
			// Cell layout/styling models
			var model = CellStyleFactory.CreateCell(rowSpan: 1, colSpan: 1, height: 18);
			var model21 = CellStyleFactory.CreateCell(rowSpan: 2, colSpan: 1, height: 18);

			document.Add(new Paragraph(@"Enter the employment number and name of each teacher teaching a single-teacher class group. Enter "
			+ @"the number of pupils enrolled at class level in the group. If all pupils in the class are at the same class "
			+ @"level, enter the enrolment in the appropriate column. If the class group contains pupils at different "
			+ @"levels, enter the number at each level in the appropriate column."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 4, 4, 3, 3, 3, 3, 3, 3 }))
						.UseAllAvailableWidth();

			table.AddCell(TextCell(new Cell(2, 1), "Employment No")
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));
			table.AddCell(TextCell(new Cell(1, 2), "Teacher Name")
				.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));

			for (int j = 0; j < 6; j++)
			{
				table.AddCell(TextCell(model21, $"Class {j + 1:0}")
					.SetVerticalAlignment(VerticalAlignment.MIDDLE)
					.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));
			}

			// second row of headings
			table.AddCell(TextCell(model, "Given Name")
				.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));
			table.AddCell(TextCell(model, "Family Name")
				.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));

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
			table.AddCell(TextCell(new Cell(1, 3), "Total Pupils")
				.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));

			for (int j = 0; j < 6; j++)
			{
				// TODO - Add support for read only NumberCell
				table.AddCell(NumberCell(model, $"Class.T.T.{j:00}.All"));
			}

			document.Add(table);
			return document;
		}
	}
}
