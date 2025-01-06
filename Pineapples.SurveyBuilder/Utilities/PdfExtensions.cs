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
			return tbl.AddRow(cellStyler, (IEnumerable<Cell>) row);
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

		#region Layout and measurement
		public static float HeightEstimate(this Cell cell)
		{
			return (cell.GetHeight()?.GetValue()??0)
				+ (cell.GetPaddingTop()?.GetValue()??0) + (cell.GetPaddingBottom()?.GetValue()??0)
				+ (cell.GetStrokeWidth()??1) ;  
		}
		public static float HeightEstimate(this Table table, Cell cellprototype, int numRows)
		{
			return (table.GetMarginBottom()?.GetValue()??0) + (table.GetMarginTop()?.GetValue()??0)
				+ (cellprototype.HeightEstimate() * numRows);
		}

		#endregion

		#region PdfDocument

		/// <summary>
		/// Trap the Verbose flag as a Keyword of the document. 
		/// </summary>
		/// <param name="pdfDoc"></param>
		/// <param name="verbose"></param>
		/// <returns></returns>
		public static PdfDocument SetVerbose(this PdfDocument pdfDoc, bool verbose)
		{
			string keywords = pdfDoc.GetDocumentInfo().GetKeywords()??String.Empty;
			string[] kk = keywords.Split(',').Where( k => !String.IsNullOrEmpty(k)).ToArray();
			if (verbose)
			{
				if (!kk.Contains("Verbose"))
				{
					Array.Resize(ref kk, kk.Length + 1); // Increase the size by 1
					kk[kk.Length - 1] = "Verbose";        // Add the new element
					keywords = String.Join(",", kk);
					pdfDoc.GetDocumentInfo().SetKeywords(keywords);
				}
			}
			else
			{
				if (kk.Contains("Verbose"))
				{
					kk = kk.Where(k => (k != "Verbose")).ToArray();
					keywords = String.Join(",", kk);
					pdfDoc.GetDocumentInfo().SetKeywords(keywords);
				}
			}
			return pdfDoc;
		}
		public static bool IsVerbose(this PdfDocument pdfDoc)
		{
			string keywords = pdfDoc.GetDocumentInfo().GetKeywords()??String.Empty;
			string[] kk = keywords.Split(',');
			return kk.Contains("Verbose");
		}
		public static void VerboseConsole(this PdfDocument pdfDoc, string output)
		{
			if (pdfDoc.IsVerbose())
			{
				Console.WriteLine(output);
			}
		}
		public static void VerboseDebug(this PdfDocument pdfDoc, string output)
		{
			if (pdfDoc.IsVerbose())
			{
				System.Diagnostics.Debug.WriteLine(output);
			}
		}

		#endregion

		#region String cleaning and manipulation
		// replace ' ' in a string with nonbreaking space

		public static string Nbsp(this string str)
		{
			string nonBreakingSpace = "\u00A0";
			return str.Replace(" ", nonBreakingSpace);
		}

		public static string Clean(this string str)
		{
			
			return str.Replace(" ", "_").Replace(@"\","||").Replace(@"/", "|");
		}

		public static string CamelCase(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			return char.ToLowerInvariant(input[0]) + input.Substring(1);
		}

		public static string SingleQuote(this string name)
		{
			return $"\"{name}\"";
		}

		#endregion
	}
}
