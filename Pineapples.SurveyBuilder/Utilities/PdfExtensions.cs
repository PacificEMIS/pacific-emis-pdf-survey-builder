using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using iText.Forms;
using iText.Forms.Fields;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System.Runtime.CompilerServices;


namespace surveybuilder
{
	public static class PdfExtensions
	{
		#region Table creation and styling

		public static Table AddRow(this Table tbl, params Cell[] row)
		{
			return tbl.AddRow((IEnumerable<Cell>) row);
		}

		public static Table AddRow(this Table tbl, IEnumerable<Cell> row)
		{
			foreach (var cell in row)
			{
				tbl.AddCell(cell);
			}
			return tbl;
		}

		public static Table AddRow(this Table tbl, PdfStyle cellStyle, params Cell[] row)
		{
			return tbl.AddRow(cellStyle, (IEnumerable<Cell>)row);
		}
		public static Table AddRow(this Table tbl, PdfStyle cellStyle, IEnumerable<Cell> row)
		{
			foreach (var cell in row)
			{
				tbl.AddCell(cell.Style(cellStyle));
			}
			return tbl;
		}
		public static Table AddRow(this Table tbl, CellStyler cellStyler, params Cell[] row)
		{
			return tbl.AddRow(cellStyler, row);
		}
		public static Table AddRow(this Table tbl, CellStyler cellStyler, IEnumerable<Cell> row)
		{
			foreach (var cell in row)
			{
				tbl.AddCell(cell.Style(cellStyler));
			}
			return tbl;
		}

		public static Table AddHeaderRow(this Table tbl, PdfStyle cellStyle, params Cell[] row)
		{
			foreach (var cell in row)
			{
				tbl.AddHeaderCell(cell.Style(cellStyle));
			}
			return tbl;
		}

		public static Table AddHeaderRow(this Table tbl, CellStyler cellStyler, params Cell[] row)
		{
			foreach (var cell in row)
			{
				tbl.AddHeaderCell(cell.Style(cellStyler));
			}
			return tbl;
		}

		// styling shrtcuts
		public static Cell Style(this Cell c, CellStyler styler)
		{ 
			return styler(c); 
		}
		public static Cell Style(this Cell c, PdfStyle style)
		{
			return style.Apply(c);
		}
		// styling shrtcuts
		public static Paragraph Style(this Paragraph p, ParagraphStyler styler)
		{
			return styler(p);
		}
		public static Paragraph Style(this Paragraph p, PdfStyle style)
		{
			return style.Apply(p);
		}
		#endregion Table creation and styling

		// replace ' ' in a string with nonbreaking space

		public static string Nbsp(this string str)
		{
			string nonBreakingSpace = "\u00A0";
			return str.Replace(" ", nonBreakingSpace);
		}
	}
}
