using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Layout.Renderer;
using Colors = iText.Kernel.Colors;
using Borders = iText.Layout.Borders;
using iText.Kernel.Pdf.Action;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Forms.Form.Element;
using System.Net;
using iText.Commons.Actions.Data;
using System.Runtime.Remoting.Contexts;
using static surveybuilder.CellMakers;
using System.Diagnostics;
using surveybuilder.Utilities;

namespace surveybuilder
{

	public class GenderedGridmaker
	{
		public LookupList Columns;
		public LookupList Rows;
		public string Tag;

		public Boolean RowTotals;
		public Boolean ColumnTotals;
		public Boolean EditRowValue;

		// Import common table/grid styles

		public int RowHeight = 12;

		public GenderedGridmaker()
		{
			RowTotals = true;
			ColumnTotals = true;
			EditRowValue = false;
		}
		public GenderedGridmaker(string name) { }

		public Table Make(PdfBuilder builder, Document document)
		{
		
			PdfStylesheet ss = builder.stylesheet;
			PdfTableStylesheet ts = new PdfTableStylesheet(ss);
			// style names as used in the grid
			//column header at the top of the grid - hold the colun labels
			const string ColHeader = "colheader";
			// second row of headers holds gender headers M and F
			const string GenderHeader = "genderheader";
			// row header holds the row labels in left-most column of grid
			const string OddRow = "oddrow";
			const string EvenRow = "evenrow";
			const string OddRowHeader = "oddrowheader";
			const string EvenRowHeader = "evenrowheader";
			const string OddRowTotal = "oddrowtotal";
			const string EvenRowTotal = "evenrowtotal";
			const string ColTotal = "coltotal";
			const string ColTotalHeader = "coltotalheader";
			const string GridTotal = "coltotal";
			const string ColLabeller = "collabeller";
			const string RowLabeller = "rowlabeller";

			/*
			 *		+------------------------------------------------------------------------------------------+
			 *		+ (blank)   + colheader + colheader + colheader + colheader + colheader + rowtotalheader   +
			 *		+------------------------------------------------------------------------------------------+
			 *		+           + M  +  F  +  M  +  F  +  M  +  F  +  M  +  F  +  M  +  F  +  M  +  F  + Total +  (genderheader)
			 *		+ rowheader + (evenrow / oddrow)                                       + rowtotal		   +
			 *		+------------------------------------------------------------------------------------------+
			 *		+ coltotalheader + coltotal											   + coltotal+ (blank)  +
			 *		+ coltotalheader + coltotal											   + (blank) + gridtotal+
			 * When EditRowValue is true, the top corner blanks are replaced by cells styled
			 * collabeller and rowlabeller
			 
			 */
			builder.pdfDoc.VerboseConsole($"GenderedGrid: {Tag}");

			string color1 = "eaeaea";
			string color2 = "cccccc";
			string color3 = "a8a8a8";


			//// cell models for table construction
			// Row headings and data fields
			Cell oddmodel = new Cell().SetHeight(RowHeight)
				.Style(ss["oddrow"]); ;

			Cell oddmodel2 = new Cell(1, 2).SetHeight(RowHeight)
				.Style(ss["oddrow"]);

			Cell evenmodel = new Cell().SetHeight(RowHeight)
				.Style(ss["evenrow"]);

			var totalmodel = new Cell()
				.SetHeight(RowHeight);
			//.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));

			var totalmodel2 = new Cell(1, 2)
				.SetHeight(RowHeight);
				//.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));

			var colheadermodel = new Cell()
				.SetHeight(RowHeight);

			var colheadermodel2 = new Cell(1, 2)
				.SetHeight(RowHeight);

			var colheadermodel4 = new Cell(1, 4)
				.SetHeight(RowHeight);
				

			// Create a table with a row heading array
			int numValueCols = 1           // row label
				+ Columns.Count * 2 + (RowTotals ? 4 : 0) ;
			int[] widths = Enumerable.Repeat(10, numValueCols).ToArray();
			if (EditRowValue)
			{
				//We make the first col wider to take the data entry. For fixed values there is 
				// resizing to fit on the column
				widths[0] = 30;
			}

			Table table = CellStyleFactory.DefaultTable(widths);

			#region Top Row
			
			if (!EditRowValue)
			{
				// Empty cell in the top-left corner
				table.AddRow(ss["abstractcell"],
					TextCell(CellStyleFactory.TwoRowOneColumn, "")
				);
			}
			
			if (EditRowValue)
			{
				// labels for column labels and row label
				table.AddRow(ss[ColLabeller],
					TextCell(colheadermodel, "Age"));
			}

			for (int j = 0; j < Columns.Count; j++)
			{
				// rowspan is 2
				table.AddRow(ss[ColHeader],
					ValueCell(colheadermodel2, $"{Tag}.C.{j:00}.V", Columns[j].N,
						StyleConfigurer(ss[ColHeader])
					)
				);


			}
			if (RowTotals)
			{
				table.AddCell(
					TextCell(colheadermodel4, "Totals").Style(ss[ColHeader]));
			}
			#endregion


			#region gender headings row

			if (!EditRowValue)
			{
				// Empty cell in the top-left corner created on row above
				//table.AddCell(new Cell().SetHeight(RowHeight));
			}
			
			if (EditRowValue)
			{
				table.AddRow(ss[RowLabeller],
					TextCell(colheadermodel, "Name of preschool"));
			}

			for (int i = 0; i < Columns.Count; i++)
			{
				// rowspan is 1
				table.AddRow(ss[GenderHeader],
					TextCell(colheadermodel, "M"),
					TextCell(colheadermodel, "F")
					);
			}
			if (RowTotals)
			{
				table.AddRow(ss[GenderHeader],
					TextCell(colheadermodel, "M"),
					TextCell(colheadermodel, "F"),
					TextCell(colheadermodel2, "Totals (M+F)")
				);
			}

			#endregion

			#region Data Rows

			// data rows 
			for (int i = 0; i < Rows.Count; i++)
			{
				// first the row header
				var cellmodel = (i % 2 == 0) ? evenmodel : oddmodel;
				var rowStyle = (i % 2 == 0) ? ss[EvenRow]: ss[OddRow];
				var headerStyle = (i % 2 == 0) ? ss[EvenRowHeader] : ss[OddRowHeader];
				var totalStyle = (i % 2 == 0) ? ss[EvenRowTotal] : ss[OddRowTotal];

				if (EditRowValue)
				{
					string name = $"{Tag}.R.{i:00}.V";

					table.AddRow(rowStyle,
						InputCell(cellmodel,name)
					);
				}

				if (!EditRowValue)

				// if we add a paragraph here:
				//TextCell(cellmodel, Rows[i].N.Nbsp())
				// the table can automatically resize the column to fit
				// but a form field does not force this resize
				// we need to do it manually

				{
					float textwidth = headerStyle.Measure(Rows[i].N.Nbsp());

					Debug.WriteLine($"{Rows[i].N.Nbsp()} {textwidth}");
					table.AddRow(headerStyle,
							ValueCell(cellmodel.Clone(false).SetMinWidth(textwidth), $"{Tag}.R.{i:00}.V", Rows[i].N.Nbsp(),
							StyleConfigurer(headerStyle))
					);

				}
				// Data fields for each row
				for (int j = 0; j < Columns.Count; j++)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string name = $"{Tag}.D.{i:00}.{j:00}.{g}";
						Paragraph pp = new Paragraph();
						//pp.SetNextRenderer(new GridDataFieldCellRenderer(cellmodel, name));
						table.AddRow(rowStyle,
							GridCell(cellmodel, name)
						);
					}

				}
				if (RowTotals)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string n = $"{Tag}.T.{i:00}.T.{g}";
						Paragraph p = new Paragraph();
						p.SetNextRenderer(new TotalFieldRenderer(totalmodel, n));
						table.AddRow(totalStyle, totalmodel.Clone(false).Add(p));
					}
					string name = $"{Tag}.T.{i:00}.T.T";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TotalFieldRenderer(totalmodel2, name));
					table.AddRow(totalStyle, totalmodel2.Clone(false).SetHeight(RowHeight).Add(pp));
				}

			}

			#endregion Data Rows

			#region Column Gender Totals

			if (ColumnTotals)
			{

				// gender totals
				
				table.AddRow(ss[ColTotalHeader],
					TextCell(CellStyleFactory.TwoRowOneColumn,"Totals")
				);

				// Data fields for each row
				for (int j = 0; j < Columns.Count; j++)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string name = $"{Tag}.T.T.{j:00}.{g}";
						Paragraph pp = new Paragraph();
						pp.SetNextRenderer(new TotalFieldRenderer(totalmodel, name));
						table.AddRow(ss[ColTotal],totalmodel.Clone(false).Add(pp));
					}
				}
				if (RowTotals)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string n = $"{Tag}.T.T.T.{g}";
						Paragraph p = new Paragraph();
						p.SetNextRenderer(new TotalFieldRenderer(totalmodel, n));
						table.AddRow(ss[ColTotal],totalmodel.Clone(false).Add(p));
					}
					Paragraph pp = new Paragraph();
					table.AddRow(ss["abstractcell"], 
						TextCell(new Cell(1,2), "")
					);
				}
			}

			#endregion Column gender totals

			#region column totals / grand total
			if (ColumnTotals)
			{
				// from above 1 column

				for (int j = 0; j < Columns.Count; j++)
				{
					string name = $"{Tag}.T.T.{j:00}.T";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TotalFieldRenderer(totalmodel, name));
					table.AddRow(ss[ColTotal], totalmodel2.Clone(false).Add(pp));

				}
				if (RowTotals)
				{
					Paragraph p = new Paragraph();
					table.AddRow(ss["abstractcell"],
						TextCell(CellStyleFactory.TwoColumn, "")
					);

					string name = $"{Tag}.T.T.T.T";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TotalFieldRenderer(totalmodel, name));
					table.AddRow(ss[GridTotal],
						new Cell(1, 2).Add(pp));

				}
			}
			#endregion column totals grand total


			// export row and column info to xfdf
			Columns.CodesAsFields(builder.pdfDoc
				, i => $"{Tag}.C.{i:00}.K");
			Rows.CodesAsFields(builder.pdfDoc
				, i => $"{Tag}.R.{i:00}.K");

			//// apply some formatting before returning the table
			return table
			//	.SetBorder(new Borders.SolidBorder(Colors.WebColors.GetRGBColor(color3), 1));
			;
		}
	}

	// Add the table to the document

}
