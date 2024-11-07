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

namespace surveybuilder
{

	public class GenderedGridmaker
	{
		public List<KeyValuePair<string, string>> Columns;
		public List<KeyValuePair<string, string>> Rows;
		public string Tag;

		public Boolean RowTotals;
		public Boolean ColumnTotals;
		public Boolean IncludeFirstColumn;

		public PdfStylesheet gridstyles = new PdfStylesheet();

		public int RowHeight = 12;

		public GenderedGridmaker()
		{
			RowTotals = true;
			ColumnTotals = true;
			IncludeFirstColumn = false;

			gridstyles.Add("gridbase",
				new PdfStyle() { FontSize = 8 });
			gridstyles.Add("rowheader",
				new PdfStyle(gridstyles["gridbase"])
				{
					TextAlignment = TextAlignment.RIGHT,
					LineSpacing = 1
				});
			gridstyles.Add("colheader",
				new PdfStyle(gridstyles["gridbase"]) { TextAlignment = TextAlignment.CENTER });
			gridstyles.Add("genderheader",
				new PdfStyle(gridstyles["colheader"]) { FontSize = gridstyles["colheader"].FontSize - 2 });

					}
		public GenderedGridmaker(string name) { }

		Paragraph rowheader(string text)
		{
			return gridstyles.ApplyStyle("rowheader", text);
		}
		Paragraph colheader(string text)
		{
			return gridstyles.ApplyStyle("colheader", text);
		}
		Paragraph genderheader(string text)
		{
			return gridstyles.ApplyStyle("genderheader", text);
		}

		public Table Make(PdfBuilder builder)
		{
			Console.WriteLine($"GenderedGrid: {Tag}");

			string color1 = "eaeaea";
			string color2 = "cccccc";
			string color3 = "a8a8a8";

			//// dell models for table construction
			// Row headings and data fields
			Cell oddmodel = new Cell().SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color1));

			Cell oddmodel2 = new Cell(1,2).SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color1));

			Cell evenmodel = new Cell().SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color2));
			
			var totalmodel = new Cell()
				.SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));

			var totalmodel2 = new Cell(1,2)
				.SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));

			var colheadermodel = new Cell()
				.SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));

			var colheadermodel2 = new Cell(1,2)
				.SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));
			var colheadermodel4 = new Cell(1, 4)
				.SetHeight(RowHeight)
				.SetBackgroundColor(Colors.WebColors.GetRGBColor(color3));

			// Create a table with )
			Table table = new Table(UnitValue.CreatePercentArray(Columns.Count * 2 + 1 + (RowTotals ? 4 : 0) + (IncludeFirstColumn ? 1 : 0)))
						  .UseAllAvailableWidth();

			// Empty cell in the top-left corner
			table.AddCell(new Cell().SetHeight(RowHeight));

			// Column headings
			if (IncludeFirstColumn)
			{
				table.AddCell(
					TextCell(colheadermodel, colheader("Age")));
			}

			foreach (var kvp in Columns)
			{
				// rowspan is 2
				table.AddCell(
					TextCell(colheadermodel2 ,colheader(kvp.Value)));

			}
			if (RowTotals)
			{
				table.AddCell(colheadermodel4.Clone(false).Add(colheader("Totals")));
			}

			// gender headings row
			// Empty cell in the top-left corner
			table.AddCell(new Cell().SetHeight(RowHeight));
						
			if (IncludeFirstColumn)
			{
				table.AddCell(
					TextCell(colheadermodel, colheader("Name of Pre-School")));
			}

			for (int i = 0; i < Columns.Count; i++)
			{
				// rowspan is 1
				table.AddCell(TextCell(colheadermodel, genderheader("M")));
				table.AddCell(TextCell(colheadermodel, genderheader("F")));
			}
			if (RowTotals)
			{
				table.AddCell(TextCell(colheadermodel, genderheader("M")));
				table.AddCell(TextCell(colheadermodel, genderheader("F")));
				table.AddCell(TextCell(colheadermodel2, colheader("F")));
			}


			// data rows 
			for (int i = 0; i < Rows.Count; i++)
			{
				// first the row header
				var cellmodel = (i % 2 == 0) ? evenmodel : oddmodel;
				table.AddCell(cellmodel.Clone(false)
					.Add(rowheader(Rows[i].Value.Nbsp()))
				);

				if (IncludeFirstColumn)
				{
					string name = $"{Tag}.R.{i:00}.V";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new GridDataFieldCellRenderer(cellmodel, name));
					table.AddCell(cellmodel.Clone(false).Add(pp));
				}

				// Data fields for each row
				for (int j = 0; j < Columns.Count; j++)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string name = $"{Tag}.D.{i:00}.{j:00}.{g}";
						Paragraph pp = new Paragraph();
						pp.SetNextRenderer(new GridDataFieldCellRenderer(cellmodel, name));
						table.AddCell(cellmodel.Clone(false).Add(pp));
					}

				}
				if (RowTotals)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string n = $"{Tag}.T.{i:00}.T.{g}";
						Paragraph p = new Paragraph();
						p.SetNextRenderer(new TotalFieldRenderer(totalmodel, n));
						table.AddCell(totalmodel.Clone(false).Add(p));
					}
					string name = $"{Tag}.T.{i:00}.T.T";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TotalFieldRenderer(totalmodel2, name));
					table.AddCell(totalmodel2.Clone(false).SetHeight(RowHeight).Add(pp));
				}

			}

			if (ColumnTotals)
			{
				// gender totals
				if (IncludeFirstColumn)
				{
					table.AddCell(new Cell().SetHeight(RowHeight));
				}
				table.AddCell(totalmodel.Clone(false).Add(rowheader("Totals")));

				// Data fields for each row
				for (int j = 0; j < Columns.Count; j++)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string name = $"{Tag}.T.T.{j:00}.{g}";
						Paragraph pp = new Paragraph();
						pp.SetNextRenderer(new TotalFieldRenderer(totalmodel, name));
						table.AddCell(totalmodel.Clone(false).Add(pp));
					}
				}
				if (RowTotals)
				{
					foreach (string g in new string[] { "M", "F" })
					{
						string n = $"{Tag}.T.T.T.{g}";
						Paragraph p = new Paragraph();
						p.SetNextRenderer(new TotalFieldRenderer(totalmodel, n));
						table.AddCell(totalmodel.Clone(false).Add(p));
					}
					Paragraph pp = new Paragraph();
					table.AddCell(totalmodel2.Clone(false).Add(pp));
				}
			}
			if (ColumnTotals)
			{
				// column totals
				if (IncludeFirstColumn)
				{
					table.AddCell(new Cell().SetHeight(RowHeight));
				}
				table.AddCell(totalmodel.Clone(false).Add(rowheader("Totals (M+F)")));				

				for (int j = 0; j < Columns.Count; j++)
				{
					string name = $"{Tag}.T.T.{j:00}.T";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TotalFieldRenderer(totalmodel, name));
					table.AddCell(totalmodel2.Clone(false).Add(pp));

				}
				if (RowTotals)
				{
					Paragraph p = new Paragraph();
					table.AddCell(new Cell(1, 2).Add(p));

					string name = $"{Tag}.T.T.T.T";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TotalFieldRenderer(totalmodel, name));
					table.AddCell(new Cell(1, 2).Add(pp));

				}
			}
			// apply some formatting before returning the table
			var form = PdfFormCreator.GetAcroForm(builder.pdfDoc, true);
			var fields = form.GetAllFormFields();
			for (int i = 0; i < Columns.Count; i++)
			{
				var txt = new TextFormFieldBuilder(builder.pdfDoc, $"{Tag}.C.{i:00}.K")   // K for key
					.CreateText();
				txt.SetValue(Columns[i].Key);
				form.AddField(txt);
			}
			for (int i = 0; i < Columns.Count; i++)
			{
				var txt = new TextFormFieldBuilder(builder.pdfDoc, $"{Tag}.C.{i:00}.V")   // K for key
					.CreateText();
				txt.SetValue(Columns[i].Value);
				form.AddField(txt);
			}
			for (int i = 0; i < Rows.Count; i++)
			{
				var txt = new TextFormFieldBuilder(builder.pdfDoc, $"{Tag}.R.{i:00}.K")
					.CreateText();
				txt.SetValue(Rows[i].Key);
				form.AddField(txt);
			}
			for (int i = 0; i < Rows.Count; i++)
			{
				var txt = new TextFormFieldBuilder(builder.pdfDoc, $"{Tag}.R.{i:00}.V")
					.CreateText();
				txt.SetValue(Rows[i].Value);
				form.AddField(txt);
			}

			return table
				//.SetBackgroundColor(Colors.WebColors.GetRGBColor("ivory"))
				.SetBorder(new Borders.SolidBorder(Colors.WebColors.GetRGBColor(color3), 1));
			;
		}
	}

	// Add the table to the document

}
