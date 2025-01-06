using iText.Forms.Fields.Properties;
using iText.Forms.Fields;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Layout.Renderer;
using Colors = iText.Kernel.Colors;
using Borders = iText.Layout.Borders;
using iText.Kernel.Pdf.Action;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Forms.Form.Element;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.Kernel.Pdf.Navigation;
using iText.Commons.Actions.Data;
using static surveybuilder.CellMakers;
using System.Runtime.CompilerServices;
using System.Configuration;
using iText.Kernel.XMP.Impl;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.Design;
using System.Security.Principal;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Org.BouncyCastle.Utilities;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Net.NetworkInformation;
using iText.Layout.Borders;
using iText.IO.Image;
using iText.Kernel.Pdf.Canvas;
using iText.IO.Font;
using Newtonsoft.Json.Linq;
using iText.Kernel.Colors;
using surveybuilder.Utilities;

namespace surveybuilder
{

	public class CellMakers
	{
		public static float MeasureTextWidth(Document document, string text, float fontSize)
		{
			// Create a temporary Paragraph and measure its width
			Paragraph paragraph = new Paragraph(text).SetFontSize(fontSize);
			var bbox =  paragraph.CreateRendererSubTree().SetParent(document.GetRenderer())
				.Layout(new iText.Layout.Layout.LayoutContext(new iText.Layout.Layout.LayoutArea(0, new iText.Kernel.Geom.Rectangle(0, 0, float.MaxValue, float.MaxValue))))
				.GetOccupiedArea().GetBBox();

			return bbox.GetWidth();
		}

		/// <summary>
		/// A configurer to apply a PdfStyle to a textformfield. This provides a way tp ass this configuration
		/// into the cellRenderer that creates the form field
		/// </summary>
		/// <param name="style">style to apply</param>
		/// <returns></returns>
		public static TextFormFieldStyler StyleConfigurer(PdfStyle style)
		{
			return (txtField) =>
			{
				style.Apply(txtField);
				// change the /DA? - this prevents readony fields from chaning font size when focused
				string da = $"/{style.FontName} {style.FontSize} Tf 0 g";
				txtField.GetPdfObject().Put(PdfName.DA, new PdfString(da));
				return txtField;
			};
		}
		public static TextFormFieldStyler ReadOnlyConfigurer(bool readOnly)
		{
			return (txtField) =>
			{
				txtField.SetReadOnly(readOnly);

				var w = txtField.GetFirstFormAnnotation();
				w.SetBackgroundColor(readOnly?NamedColors.White:NamedColors.OldLace);
				return txtField;
			};
		}
		#region cells for checkboxes
		public static Cell YesCell(Cell cellmodel, PdfButtonFormField grp)
		{
			return CheckCell(cellmodel, grp, CheckBoxType.CHECK, NamedColors.DarkGreen,"Y");
		}
		public static Cell NoCell(Cell cellmodel, PdfButtonFormField grp)
		{
			return CheckCell(cellmodel, grp,CheckBoxType.CROSS, NamedColors.DarkRed, "N" );
		}
		public static Cell SelectCell(Cell cellmodel, PdfButtonFormField grp, string exportValue)
		{
			return CheckCell(cellmodel, grp, CheckBoxType.SQUARE, ColorConstants.GRAY, exportValue );
		}

		public static Cell CheckCell(Cell cellmodel, PdfButtonFormField grp, string exportValue,
			CheckBoxType typ)
		{
			return CheckCell(cellmodel, grp, typ, ColorConstants.GRAY, exportValue);
		}
		public static Cell CheckCell(Cell cellmodel, PdfButtonFormField grp, 
			CheckBoxType typ, Color forecolor, string export )
		{
			Cell cell = cellmodel.Clone(false);

			Paragraph pp = new Paragraph();
			PdfDictionary ap = lookups.Ap(typ, forecolor, export);
			pp.SetNextRenderer(new CheckBoxGroupCellRenderer(cell, grp, ap, export));

			return cell.Add(pp);
		}
		#endregion

		#region cells for numeric inputs
		public static Cell NumberCell(Cell cellmodel, string fieldname, float? value = null,
			int decimals = 0,
			AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT,
			AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true
			, TextFormFieldStyler configurer = null)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new NumberFieldCellRenderer(cellmodel, fieldname, value,
				decimals, sepStyle, negStyle, currStyle, strCurrency, prePend, configurer));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;

		}
		/// <summary>
		/// Cell to appear in GenderedGrids,  Actions support running totals of the grid
		/// and row/column tracking within the row headers and column headers
		/// 
		/// </summary>
		/// <param name="cellmodel"></param>
		/// <param name="fieldname"></param>
		/// <param name="value"></param>
		/// <param name="decimals"></param>
		/// <param name="sepStyle"></param>
		/// <param name="negStyle"></param>
		/// <param name="currStyle"></param>
		/// <param name="strCurrency"></param>
		/// <param name="prePend"></param>
		/// <param name="configurer"></param>
		/// <returns></returns>
		public static Cell GridCell(Cell cellmodel, string fieldname, 
			int decimals = 0,
			AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT,
			AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true
			, TextFormFieldStyler configurer = null)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new GridDataFieldCellRenderer(cellmodel, fieldname));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}
		#endregion

		#region cells for date inputs
		/// <summary>
		/// Creates a cell for date inputs, using a custom renderer for handling dates.
		/// </summary>
		/// <param name="cellmodel">The model cell to clone.</param>
		/// <param name="fieldname">The unique field name associated with the date cell.</param>
		/// <param name="dateValue">The optional date value to prepopulate.</param>
		/// <param name="dateFormat">The date format string (e.g., "yyyy-MM-dd").</param>
		/// <returns>A new cell configured for date input.</returns>
		public static Cell DateCell(Cell cellmodel, string fieldname, DateTime? dateValue = null, string dateFormat = "yyyy-MM-dd")
		{
			// Create a paragraph and set a custom renderer for dates
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new DateFieldCellRenderer(cellmodel, fieldname, dateValue, dateFormat));

			// Clone the cell model and add the paragraph
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}
		#endregion

		#region Text cells
		// text input
		public static Cell InputCell(Cell cellmodel, string fieldname, int maxLen = 0,
			string value = null, bool readOnly = false, TextFormFieldStyler configurer = null)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new TextFieldCellRenderer(cellmodel, fieldname, maxLen, value, readOnly, configurer));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}

		public static Cell MultiLineInputCell(Cell cellmodel, string fieldname, int maxLen = 0,
			string value = null)
		{
			Paragraph pp = new Paragraph();
			Func<PdfFormField, PdfFormField> styler = (fld) =>
			{
				PdfTextFormField txt = fld as PdfTextFormField;

				txt.SetMaxLen((maxLen == 0) ? 4000 : maxLen)
					.SetMultiline(true)
					.SetFontSize(0);
				txt.SetValue(value);

				var w = fld.GetFirstFormAnnotation();
				w.SetBackgroundColor(Colors.WebColors.GetRGBColor("OldLace"));
				return txt;
			};
			pp.SetNextRenderer(new FieldCellRenderer(cellmodel, fieldname, styler));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}
		/// <summary>
		/// a readonly text field The reason to create a field to hold
		/// this value is to have it exported to the Xfdf, so that this constant value is
		/// accessible to the stored procedure uploading the data. 
		/// Note: this approach is somewhat obselete, and is here for legacy support
		/// or in cases where e.g. a column or row header is implemented as a field
		/// so it can be manipulated in Javascript. 
		/// Otherwise, use flat text to render the value in its table cell and create a hidden field on the 
		/// same value to pass into the Xfdf using ExportValue
		/// </summary>
		/// <param name="cellmodel"></param>
		/// <param name="fieldname"></param>
		/// <param name="value"></param>
		/// <param name="hidden"></param>
		/// <returns></returns>
		public static Cell ValueCell(Cell cellmodel, string fieldname, 	string value
			, TextFormFieldStyler configurer = null)
		{
			return InputCell(cellmodel, fieldname, 0, value, true, configurer);
		}

		public static Cell TotalCell(Cell cellmodel, string fieldname, string value = null
			, TextFormFieldStyler configurer = null)
		{
			Paragraph p = new Paragraph();
			p.SetNextRenderer(new TotalFieldRenderer(cellmodel, fieldname));
			Cell cell = cellmodel.Clone(false);
			cell.Add(p);
			return cell;
		}
		/// <summary>
		/// create a hidden form field to pass a value into the Xfdf
		/// Note that does not return a cell, since we need no visual presence
		/// </summary>
		/// <param name="pdfDoc"></param>
		/// <param name="fieldname"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static PdfFormField ExportValue(PdfDocument pdfDoc, string fieldname, string value)
		{
			var fld = new TextFormFieldBuilder(pdfDoc, fieldname)
				.CreateText()
				.SetReadOnly(true)
				.SetValue(value);
			PdfAcroForm.GetAcroForm(pdfDoc, true).AddField(fld);
			return fld;
		}

		#endregion
		// combo box
		public static Cell ComboCell(Cell cellmodel, string fieldname, PdfArray options,
	string value = null, bool readOnly = false, bool hidden = false)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new ComboboxCellRenderer(cellmodel, fieldname, options));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}

		/// <summary>
		/// Create a cell filled with a push button
		/// </summary>
		/// <param name="cellmodel">cell prototype</param>
		/// <param name="fieldname">field name of button</param>
		/// <param name="label">text caption on button</param>
		/// <param name="js">javascript command for push action</param>
		/// <param name="configurer">configuration action</param>
		/// <returns></returns>
		public static Cell PushButtonCell(Cell cellmodel, string fieldname, string label,
			string js, Action<PdfButtonFormField> configurer = null)

		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new PushButtonCellRenderer(cellmodel, fieldname, label, js, configurer));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}

		/// <summary>
		/// Cell containing static text
		/// </summary>
		/// <param name="cellmodel">cell prototype</param>
		/// <param name="text">text to display in cell</param>
		/// <returns></returns>
		public static Cell TextCell(Cell cellmodel, string text)
		{
			Cell cell = cellmodel.Clone(false);
			cell.Add(new Paragraph(text));
			return cell;
		}

		/// <summary>
		/// Cell containing static text 
		/// </summary>
		/// <param name="cellmodel">cell prototype</param>
		/// <param name="pp">Paragraph to include in cell</param>
		/// <returns></returns>
		public static Cell TextCell(Cell cellmodel, Paragraph pp)
		{
			cellmodel.GetProperty<Border>(Property.BORDER);  //?? does nothing
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}

		public static Cell ElementCell(Cell cellmodel, IBlockElement element)
		{
			Cell cell = cellmodel.Clone(false);
			cell.Add(element);
			return cell;
		}

		private static LookupManager lookups;
		public static void SetLookups(LookupManager lkp)
		{
			lookups = lkp;
		}

	}
}