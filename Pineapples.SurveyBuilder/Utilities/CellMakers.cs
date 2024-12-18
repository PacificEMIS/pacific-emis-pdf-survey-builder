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
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new NumberFieldCellRenderer(cellmodel, fieldname, value,
				decimals, sepStyle, negStyle, currStyle, strCurrency, prePend));
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
		/// <param name="dateFormat">The date format string (e.g., "MM/dd/yyyy").</param>
		/// <returns>A new cell configured for date input.</returns>
		public static Cell DateCell(Cell cellmodel, string fieldname, DateTime? dateValue = null, string dateFormat = "MM/dd/yyyy")
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
			string value = null, bool readOnly = false, bool hidden = false)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new TextFieldCellRenderer(cellmodel, fieldname, maxLen, value, readOnly, hidden));
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
		/// Note: this approach is somewhat obselete, and is here for legacy support. 
		/// Use flat text to render the value in its table cell and create a hidden field on the 
		/// same value to pass into the Xfdf using ExportValue
		/// </summary>
		/// <param name="cellmodel"></param>
		/// <param name="fieldname"></param>
		/// <param name="value"></param>
		/// <param name="hidden"></param>
		/// <returns></returns>
		public static Cell ValueCell(Cell cellmodel, string fieldname, 	string value, bool hidden = false)
		{
			return InputCell(cellmodel, fieldname, 0, value, true, hidden);
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

		public static Cell PushButtonCell(Cell cellmodel, string fieldname, string label,
			string js, Action<PdfButtonFormField> configurer = null)

		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new PushButtonCellRenderer(cellmodel, fieldname, label, js, configurer));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}


		//static text in a cell
		public static Cell TextCell(Cell cellmodel, string text)
		{
			Cell cell = cellmodel.Clone(false);
			cell.Add(new Paragraph(text));
			return cell;
		}
		// static text passing a paragraph - allows a style to be applied
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