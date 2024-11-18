using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
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
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Forms.Fields.Properties;
using iText.Kernel.Pdf.Filespec;
using static surveybuilder.CellMakers;
using itext4.Utilities;

namespace surveybuilder
{

	public class CheckBoxPickmaker
	{
		public string[] Names;
		public int[] Values;
		public CheckBoxType[] Types;
		public string Tag;
		// Import common table/grid styles
		PdfTableStylesheet ts = new PdfTableStylesheet();

		public CheckBoxPickmaker()
		{
			Types = new CheckBoxType[] { };
		}

		public Document Make(PdfBuilder builder, Document document)
		{
			// Cell layout/styling models
			var model = CellStyleFactory.Default;

			Console.WriteLine($"Checkbox picker: {Tag}");
			PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, Tag)
				.CreateRadioGroup();


			// Create a table with 
			Table table = new Table(UnitValue.CreatePercentArray(Names.Length))
						  .UseAllAvailableWidth();

			// Column headings

			for (int i = 0; i < Names.Length; i++)
			{
				table.AddCell(ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle(Names[i]))));
			}

			for (int i = 0; i < Names.Length; i++)
			{
				Rectangle rect = new Rectangle(50 + 100 * i, 200, 40, 40);
				Cell cell = new Cell().SetHeight(50);

				Paragraph pp = new Paragraph().SetFontColor(Colors.ColorConstants.GREEN);
				pp.SetNextRenderer(new CheckBoxGroupCellRenderer(cell, rgrp, Values[i].ToString()
					, (Types.Length > 0) ? Types[i] : CheckBoxType.SQUARE));

				table.AddCell(new Cell().SetHeight(50).Add(pp));

			}
			var form = PdfFormCreator.GetAcroForm(builder.pdfDoc, true);

			form.AddField(rgrp);

			var fields = form.GetAllFormFields();

			document.Add(table);
			// now all the fields are created, together with their annotations. We can clean up....
			// THIS is how you turn off the Radio flag, and hence get real toggle off
			rgrp.SetFieldFlags(0);

			return document;
		}

		private void Lint(Document document)
		{
			//various experiments at coloring a checkbox symbol - NOT USED
		}

		// returnthe dingbat character for each checkbox type
		private string CbString(CheckBoxType type)
		{
			switch (type)
			{
				case CheckBoxType.CHECK:
					return "4";
				case CheckBoxType.SQUARE:
					return "n";
				case CheckBoxType.CIRCLE:
					return "l";
				case CheckBoxType.DIAMOND:
					return "u";
				case CheckBoxType.STAR:
					return "H";
				case CheckBoxType.CROSS:
					return "6";
				default:
					return "n";
			}
		}
	}
}
