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


namespace surveybuilder
{
	public class Gridmaker
	{
		public string[] Columns;
		public string[] Rows;
		public string Tag;

		public Gridmaker() { }
		public Gridmaker(string name) { }


		public Table Make()
		{
			// Create a table with )
			Table table = new Table(UnitValue.CreatePercentArray(Columns.Length + 1))
						  .UseAllAvailableWidth();

			// Empty cell in the top-left corner
			table.AddCell(new Cell());

			// Column headings

			for (int i = 0; i < Columns.Length; i++)
			{
				////////// Create a text form field for each column heading
				////////PdfTextFormField field = PdfTextFormField.CreateText(pdfDoc, new Rectangle(0, 0), $"C.0{i}", columnHeadings[i]);
				////////field.SetValue(columnHeadings[i]);
				////////field.SetReadOnly(true);
				////////PdfAcroForm.GetAcroForm(pdfDoc, true).AddField(field);

				table.AddCell(new Cell().Add(new Paragraph(Columns[i])));
			}

			// Row headings and data fields

			for (int i = 0; i < Rows.Length; i++)
			{
				// Row heading cell
				////PdfTextFormField rowField = PdfTextFormField.CreateText(pdfDoc, new Rectangle(0, 0), $"R.0{i}", rowHeadings[i].ToString());
				////rowField.SetValue(rowHeadings[i].ToString());
				////rowField.SetReadOnly(true);
				////PdfAcroForm.GetAcroForm(pdfDoc, true).AddField(rowField);

				table.AddCell(new Cell().Add(new Paragraph(Rows[i].ToString())));

				// Data fields for each row
				for (int j = 0; j < Columns.Length; j++)
				{
					string name = $"{Tag}.D.{i:00}.{j:00}";
					Paragraph pp = new Paragraph();
					pp.SetNextRenderer(new TextFieldCellRenderer(new Cell(), name));
					table.AddCell(new Cell().Add(pp));
				}
			}

			return table;
		}

	}
}
