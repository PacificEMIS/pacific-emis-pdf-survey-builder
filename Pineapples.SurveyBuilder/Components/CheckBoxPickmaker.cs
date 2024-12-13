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
using surveybuilder.Utilities;
using System.Net.Http.Headers;

namespace surveybuilder
{
	/// <summary>
	/// Calss to create a set of mutually exclusive checkboxes bound to multiple export values
	/// </summary>
	public class CheckBoxPickmaker
	{
		/// <summary>
		/// Names to appear in first row of table, over the checkbox. The size of this array determnes
		/// the number of columns in the table
		/// </summary>
		public string[] Names;		
		/// <summary>
		/// The export values. Must be 1 export value for each name
		/// </summary>
		public object[] Values;

		/// <summary>
		/// The checkbox symbol may be specified individually in Types array, or by the single DefaultType
		/// If neither, default SQUARE is used
		/// If Types.Length < Names.Length, or if aany element in types is null, the default is used 
		/// fo missing values
		/// </summary>
		public CheckBoxType[] Types;
		public CheckBoxType DefaultType;
		/// <summary>
		/// Colors for each checkbox symbol, or DefaultColor for all. If neither, a default is provided.
		/// Default is used for missing items in Colors array. 
		/// </summary>
		public Color[] Colors;
		public Color DefaultColor;

		public string Tag;
		// Import common table/grid styles
		

		public CheckBoxPickmaker()
		{
			Types = new CheckBoxType[] { };
			Colors = new Color[] {};
			DefaultType = CheckBoxType.SQUARE;
			DefaultColor = ColorConstants.DARK_GRAY;
		}

		public Document Make(PdfBuilder builder, Document document)
		{
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
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
				CheckBoxType type = (i >= Types.Length ? DefaultType : Types[i]);
				Color color = (i >= Colors.Length? DefaultColor : Colors[i]??DefaultColor);
				//Cell outCell = CellMakers.SelectCell(model, rgrp, Values[i]);
				Cell outCell = CellMakers.CheckCell(model, rgrp, type, color, Values[i].ToString());
				table.AddCell(outCell);

			}
			var form = PdfFormCreator.GetAcroForm(builder.pdfDoc, true);

			form.AddField(rgrp);

			var fields = form.GetAllFormFields();

			document.Add(table);
			return document;
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
