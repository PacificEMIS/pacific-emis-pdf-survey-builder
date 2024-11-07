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

namespace surveybuilder
{
	public class JointTeacherClasses
	{
		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{

			document.Add(new Paragraph(@"Enter the employment number and name of each teacher teaching a class where two teachers share the "
			+ @"teaching of one class. Enter in the box opposite their name and the number of pupils enrolled in the "
			+ @"class they teach."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 4,4,4,3,3,3,3,3,3}))
						.UseAllAvailableWidth();

			Cell model = new Cell().SetHeight(18);
			Cell model21 = new Cell(2, 1).SetHeight(18);
			Cell model2 = new Cell(2, 1).SetHeight(40);

			// first row of headings
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

			// data rows
			for (int i = 0; i <= 20; i++)
			{
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T0.ID", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T0.Given", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T0.Family", 50));	

				for (int j = 0; j < 6; j++)
				{
					table.AddCell(NumberCell(model2, $"ClassJ.D.{i:00}.{j:00}.All"));
				}

				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T1.ID", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T1.Given", 50));
				table.AddCell(InputCell(model, $"ClassJ.D.{i:00}.T1.Family", 50));

			}

			// Totals
			table.AddCell(TextCell(new Cell(1, 3), "Total Pupils")
				.SetBackgroundColor(Colors.ColorConstants.LIGHT_GRAY));

			for (int j = 0; j < 6; j++)
			{
				// TODO - Add support for read only NumberCell
				table.AddCell(NumberCell(model, $"ClassJ.T.T.{j:00}.All"));
			}

			document.Add(table);
			return document;
		}
	}
}
