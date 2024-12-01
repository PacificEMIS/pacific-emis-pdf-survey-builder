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
		public static Table AddRow(this Table tbl, params Cell[] row)
		{
			foreach (var cell in row)
			{
				tbl.AddCell(cell);
			}
			return tbl;
		}

		public static Table AddRow(this Table tbl, IEnumerable<Cell> row)
		{
			foreach (var cell in row)
			{
				tbl.AddCell(cell);
			}
			return tbl;
		}


		public static Table AddHeaderRow(this Table tbl, PdfTableStylesheet ts, params Cell[] row)
		{
			
			foreach (var cell in row)
			{
				tbl.AddHeaderCell(ts.TableHeaderStyle(cell));
			}
			return tbl;
		}

		// replace ' ' in a string with nonbreaking space

		public static string Nbsp(this string str)
		{
			string nonBreakingSpace = "\u00A0";
			return str.Replace(" ", nonBreakingSpace);
		}
	}
}
