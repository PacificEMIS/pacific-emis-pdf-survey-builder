using iText.Forms.Fields.Properties;
using iText.Forms.Fields;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveybuilder
{
	public class CellMakers
	{
		#region cells for checkboxes
		public static Cell YesCell(Cell cellmodel, PdfButtonFormField grp)
		{
			return CheckCell(cellmodel, grp, "Y", CheckBoxType.CHECK);
		}
		public static Cell NoCell(Cell cellmodel, PdfButtonFormField grp)
		{
			return CheckCell(cellmodel, grp, "N", CheckBoxType.CROSS);
		}
		public static Cell SelectCell(Cell cellmodel, PdfButtonFormField grp, string exportValue)
		{
			return CheckCell(cellmodel, grp, exportValue, CheckBoxType.SQUARE);
		}

		public static Cell CheckCell(Cell cellmodel, PdfButtonFormField grp, object value,
			CheckBoxType typ)
		{
			Cell cell = cellmodel.Clone(false);

			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new CheckBoxGroupCellRenderer(cell, grp, value.ToString()
				, typ));

			return cell.Add(pp);
		}

		#endregion

		#region cells for numeric inputs
		public static Cell NumberCell(Cell cellmodel, string fieldname, float? value = null,
			int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new NumberFieldCellRenderer(cellmodel, fieldname, value,
				decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend));
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;

		}
		#endregion

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


		// combo box
		public static Cell ComboCell(Cell cellmodel, string fieldname, List<KeyValuePair<string, string>> options,
	string value = null, bool readOnly = false, bool hidden = false)
		{
			Paragraph pp = new Paragraph();
			pp.SetNextRenderer(new ComboboxCellRenderer(cellmodel, fieldname, options));
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
			Cell cell = cellmodel.Clone(false);
			cell.Add(pp);
			return cell;
		}
	}
}
