using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveybuilder.Utilities
{
	using iText.Layout.Element;
	using iText.Layout.Properties;
	using System.Collections;

	public static class CellStyleFactory
	{

		public static PdfStyle BaseStyle { get; set; }


		/// <summary>
		/// Creates a styled cell with specified row and column spans, height, and vertical alignment.
		/// </summary>
		/// <param name="rowSpan">Number of rows the cell spans.</param>
		/// <param name="colSpan">Number of columns the cell spans.</param>
		/// <param name="height">Height of the cell.</param>
		/// <param name="verticalAlignment">Vertical alignment of the cell content.</param>
		/// <param name="applyHeight">Specifies whether to set the height of the cell.</param>
		/// <returns>A styled cell.</returns>
		public static Cell CreateCell(int rowSpan = 1, int colSpan = 1, float height = 20, VerticalAlignment verticalAlignment = VerticalAlignment.MIDDLE, bool applyHeight = true)
		{
			var cell = new Cell(rowSpan, colSpan)
				.SetVerticalAlignment(verticalAlignment);

			if (applyHeight)
			{
				cell.SetHeight(height);
			}
			if (BaseStyle != null)
			{
				BaseStyle.Apply(cell);
			}

			return cell;
		}

		/// <summary>
		/// Predefined default cell style.
		/// </summary>
		public static Cell Default => CreateCell();

		/// <summary>
		/// Predefined default cell style.
		/// </summary>
		public static Cell DefaultNoHeight => CreateCell(applyHeight: false);

		/// <summary>
		/// Predefined style for a cell spanning two columns.
		/// </summary>
		public static Cell TwoColumn => CreateCell(1, 2);

		/// <summary>
		/// Predefined style for a cell spanning three columns.
		/// </summary>
		public static Cell ThreeColumn => CreateCell(1, 3);

		/// <summary>
		/// Predefined style for a cell spanning four columns.
		/// </summary>
		public static Cell FourColumn => CreateCell(1, 4);

		/// <summary>
		/// Predefined style for a cell spanning five columns.
		/// </summary>
		public static Cell FiveColumn => CreateCell(1, 5);

		/// <summary>
		/// Predefined style for a cell spanning six columns.
		/// </summary>
		public static Cell SixColumn => CreateCell(1, 6);

		/// <summary>
		/// Predefined style for a cell spanning seven columns.
		/// </summary>
		public static Cell SevenColumn => CreateCell(1, 7);

		/// <summary>
		/// Predefined style for a cell spanning two rows and one column.
		/// </summary>
		public static Cell TwoRowOneColumn => CreateCell(2, 1);

		/// <summary>
		/// Predefined style for a cell spanning three rows and one column.
		/// </summary>
		public static Cell ThreeRowOneColumn => CreateCell(3, 1);

		/// <summary>
		/// Predefined style for a cell spanning two rows and two columns.
		/// </summary>
		public static Cell TwoRowTwoColumn => CreateCell(2, 2);

		public static Table DefaultTable(params int[] columnwidths)
		{
			float[] floatWidths = Array.ConvertAll(columnwidths, x => (float)x);
			return DefaultTable(floatWidths);
		}

		public static Table DefaultTable(float[] columnwidths) 
		{ 
			return new Table(UnitValue.CreatePercentArray(columnwidths))
				.UseAllAvailableWidth()
				.SetMarginBottom(15); 
		}
	}
}
