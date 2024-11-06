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
using iText.Kernel.Colors.Gradients;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Forms.Fields.Properties;
using System.Runtime.InteropServices;
using System.Globalization;
using itext4.Utilities;
namespace surveybuilder
{

	/// <summary>
	/// Make a chekcbox in a table cell, that is part of a mutully exclusive group
	/// Note that the rgrp element need to be created as a Radio Group, but then
	/// call rgrp.SetFiieldFlgs(0) to get the expected behavoys and appearance of the group
	/// of buttons
	/// </summary>
	public class CheckBoxGroupCellRenderer : CellRenderer
	{
		// The name of the check box field
		//protected internal String name;
		protected internal String export;
		protected internal PdfButtonFormField rgrp;

		protected internal CheckBoxType chktype;

		public CheckBoxGroupCellRenderer(Cell modelElement, PdfButtonFormField rgrp, string export
			, CheckBoxType chktype = CheckBoxType.SQUARE
			)
			: base(modelElement)
		{
			//this.name = name;
			this.export = export;
			this.rgrp = rgrp;
			this.chktype = chktype;
		}

		// If renderer overflows on the next area, iText uses getNextRender() method to create a renderer for the overflow part.
		// If getNextRenderer isn't overriden, the default method will be used and thus a default rather than custom
		// renderer will be created
		public override IRenderer GetNextRenderer()
		{
			return new CheckBoxGroupCellRenderer((Cell)modelElement, rgrp, export, chktype);
		}

		public override void Draw(DrawContext drawContext)
		{

			PdfDocument thisDoc = drawContext.GetDocument();

			// Define the coordinates of the middle
			float r = 10; // radius
			float x = (GetOccupiedAreaBBox().GetLeft() + GetOccupiedAreaBBox().GetRight()) / 2;
			float y = (GetOccupiedAreaBBox().GetTop() + GetOccupiedAreaBBox().GetBottom()) / 2;
			// Define the position of a check box that measures 20 by 20
			Rectangle rect = new Rectangle(x - r, y - r, 2 * r, 2 * r);

			var cb = new CheckBox("tmp");

			var chkbtn = new CheckBoxFormFieldBuilder(thisDoc, rgrp.GetFieldName().ToString())

				.SetWidgetRectangle(rect)
				.CreateCheckBox();

			chkbtn
				.SetCheckType(chktype)
				.SetValue(export);

			var w = chkbtn.GetFirstFormAnnotation();

			w.SetBorderColor(ColorConstants.LIGHT_GRAY)
				.SetBorderWidth(1);

			// draw the border here - tryng to do it in the annotation later
			// forces a regenerate, which in turn loses the checkbox type 
			// refer to DrawCheckBoxAndSaveAppearance in PdfFormAnnotation
			//PdfCanvas canvas = drawContext.GetCanvas();
			/// note the above action on the annotation here fixes it
			//canvas.Rectangle(rect)
			//		.SetStrokeColor(ColorConstants.LIGHT_GRAY)
			//		.Stroke();

			rgrp.AddKid(chkbtn);
			rgrp.SetValue(String.Empty);

			// this would seem to add rgrp over and over,
			// but it is necessary in order for this widget to get drawn
			PdfAcroForm.GetAcroForm(thisDoc, true).AddField(rgrp);

			base.Draw(drawContext);
		}
	}

	public class RadioButtonGroupCellRenderer : CellRenderer
	{
		// The name of the check box field
		//protected internal String name;
		protected internal String export;
		protected internal PdfButtonFormField rgrp;

		public RadioButtonGroupCellRenderer(Cell modelElement, PdfButtonFormField rgrp, string export)
			: base(modelElement)
		{
			//this.name = name;
			this.export = export;
			this.rgrp = rgrp;
		}

		// If renderer overflows on the next area, iText uses getNextRender() method to create a renderer for the overflow part.
		// If getNextRenderer isn't overriden, the default method will be used and thus a default rather than custom
		// renderer will be created
		public override IRenderer GetNextRenderer()
		{
			return new RadioButtonGroupCellRenderer((Cell)modelElement, rgrp, export);
		}
		public override void Draw(DrawContext drawContext)
		{
			PdfDocument thisDoc = drawContext.GetDocument();

			// Define the coordinates of the middle
			float r = 10; // radius
			float x = (GetOccupiedAreaBBox().GetLeft() + GetOccupiedAreaBBox().GetRight()) / 2;
			float y = (GetOccupiedAreaBBox().GetTop() + GetOccupiedAreaBBox().GetBottom()) / 2;
			// Define the position of a check box that measures 20 by 20
			Rectangle rect = new Rectangle(x - r, y - r, 2 * r, 2 * r);

			PdfFormAnnotation rb = new RadioFormFieldBuilder(thisDoc, rgrp.GetFieldName().ToString())
				.SetWidgetRectangle(rect)
				//.SetPage()
				.CreateRadioButton(export, null);
			rb.SetColor(Colors.ColorConstants.GREEN);
			rgrp.AddKid(rb);

			// now we draw the circle
			drawContext.GetCanvas()
					.Circle(x, y, 10)
					.SetStrokeColor(ColorConstants.LIGHT_GRAY)
					.Stroke();

			// this would seem to add rgrp over and over, but it is necessary in order for this widget to get drawn
			PdfAcroForm.GetAcroForm(thisDoc, true).AddField(rgrp);

			base.Draw(drawContext);
		}
	}


	/// <summary>
	/// Draws a text box within a field cell
	/// model - a model cell: set e.g. ColSpan, RowSpan and height
	/// name - name for the text field
	/// maxLen - maximum number of characeters
	/// value - initial value
	/// readOnly - read only true/ false
	/// hidden - true false
	/// </summary>
	public class TextFieldCellRenderer : CellRenderer
	{
		private string name;
		private int maxLen;
		private bool readOnly;
		private bool hidden;
		private string value;

		public TextFieldCellRenderer(Cell modelElement, string name) :
			this(modelElement, name, 0, null, false, false)
		{ }
		public TextFieldCellRenderer(Cell modelElement, string name, int maxLen) :
				this(modelElement, name, maxLen, null, false, false)
		{ }
		public TextFieldCellRenderer(Cell modelElement, string name, int maxLen, string value) :
			this(modelElement, name, maxLen, value, false, false)
		{ }
		public TextFieldCellRenderer(Cell modelElement, string name, int maxLen, string value, bool readOnly, bool hidden)
			: base(modelElement)
		{
			this.name = name;
			this.maxLen = maxLen;
			this.value = value;
			this.readOnly = readOnly;
			this.hidden = hidden;
		}

		public override IRenderer GetNextRenderer()
		{
			return new TextFieldCellRenderer((Cell)modelElement, name, maxLen, value, readOnly, hidden);
		}

		public override void Draw(DrawContext drawContext)
		{
			PdfDocument thisDoc = drawContext.GetDocument();
			var pttb = new TextFormFieldBuilder(thisDoc, name);

			Rectangle rect = GetOccupiedAreaBBox();
			PdfTextFormField dataField;
			if (hidden)
			{
				dataField = pttb.CreateText();
			}
			else
			{
				dataField = pttb
					.SetWidgetRectangle(rect)
					.CreateText();
			}
			dataField.SetMaxLen(maxLen == 0 ? 100 : maxLen)
				.SetReadOnly(readOnly);
			var w = dataField.GetFirstFormAnnotation();

			w.SetBackgroundColor(Colors.WebColors.GetRGBColor("OldLace"));

			if (value != null)
			{
				dataField.SetValue(value);
			}

			PdfAcroForm.GetAcroForm(thisDoc, true).AddField(dataField);

			base.Draw(drawContext);



		}
	}

/**
	 * Arguments
nDec (number of decimals):

Type: Integer
Description: Specifies the number of decimal places to display. If set to 0, no decimal places will be shown.
Example: 0 (no decimal places), 2 (two decimal places).

sepStyle (separator style):

	Type: Integer
	Description: Defines the style of the thousands separator.
	0: No separator.
	1: Comma (,) separator.
	2: Period (.) separator.
	3: Space ( ) separator.
	Example: 1 (comma as thousands separator).
negStyle (negative number style):

	Type: Integer
	Description: Determines how negative numbers are displayed.
	0: Minus sign before the number (-1234).
	1: Parentheses around the number ((1234)).
	2: Red color for negative numbers (if supported by the viewer).
	Example: 0 (minus sign), 1 (parentheses).

currStyle (currency style):

	Type: Integer
	Description: Specifies whether the currency symbol should be used.
	0: No currency symbol.
	1: Currency symbol is used.
	Example: 0 (no currency symbol), 1 (use currency symbol).

strCurrency (currency string):

	Type: String
	Description: The currency symbol to display, such as $, €, or £. This is only used if currStyle is set to 1.
	Example: "$" (dollar sign), "€" (euro sign).

bCurrencyPrepend (currency prepend):

	Type: Boolean
	Description: Determines the position of the currency symbol. If true, the currency symbol is prepended to the number (e.g., $1234). If false, it is appended to the number (e.g., 1234$).
	Example: true (currency symbol before number), false (currency symbol after number).
	Example Usage
For the function call AFNumber_Format(0, 1, 0, 0, '', true):
	 */
	/// <summary>
	/// Creates a text box for numeric input in a cell

	/// </summary>
	public class NumberFieldCellRenderer : FieldCellRenderer
	{

		string formataction = string.Empty;
		string keystrokeaction = string.Empty;

		public NumberFieldCellRenderer(Cell modelElement, string name, float? value = null,
			int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
			: base(modelElement, name)
		{	
			formataction = Actions.NFormatJs(decimals,sepStyle,negStyle, currStyle, strCurrency, bPrePend);
			keystrokeaction = Actions.NKeystrokeJs(decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend);
			base.styler = FieldStyle;
		}

		public NumberFieldCellRenderer(Cell modelElement, string name, string formataction, string keystrokeaction)
			: base(modelElement, name)
		{
			this.formataction = formataction;
			this.keystrokeaction = keystrokeaction;
			base.styler = FieldStyle;
		}
		public override IRenderer GetNextRenderer()
		{
			return new NumberFieldCellRenderer((Cell)modelElement, name, formataction, keystrokeaction);
		}

		public PdfFormField FieldStyle(PdfFormField field)
		{
			field
				.SetReadOnly(false)
				.SetAdditionalAction(PdfName.F, PdfAction.CreateJavaScript(formataction))
				.SetAdditionalAction(PdfName.K, PdfAction.CreateJavaScript(keystrokeaction))
				.SetJustification(TextAlignment.RIGHT)
				.SetFontSize(10)
				.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
				
				var w = field.GetFirstFormAnnotation();
				w.SetBackgroundColor(Colors.WebColors.GetRGBColor("OldLace"));
			return (PdfFormField)field;
		}
	}


	/// <summary>
	/// Cell in a grid. As well as the usual numeric constraints, has the Validation macro grdv
	/// to update the grid totals
	/// </summary>
	public class GridDataFieldCellRenderer : FieldCellRenderer
	{

		public GridDataFieldCellRenderer(Cell modelElement, string name)
			: base(modelElement, name, FieldStyle)
		{
		}
		public override IRenderer GetNextRenderer()
		{
			return new GridDataFieldCellRenderer((Cell)modelElement, name);
		}

		public static PdfFormField FieldStyle(PdfFormField field)
		{
			field
				.SetReadOnly(false)
				.SetAdditionalAction(PdfName.F, Actions.NFormat())
				.SetAdditionalAction(PdfName.K, Actions.NKeystroke())
				.SetAdditionalAction(PdfName.V, PdfAction.CreateJavaScript("p.grdv(event)"))
				.SetJustification(TextAlignment.RIGHT)
				.SetFontSize(10)
				.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));

				var w = field.GetFirstFormAnnotation();
				w.SetBackgroundColor(Colors.WebColors.GetRGBColor("OldLace"));
			return (PdfFormField)field;


;		}
	}
	public class TotalFieldRenderer : FieldCellRenderer
	{


		public TotalFieldRenderer(Cell modelElement, string name)
			: base(modelElement, name, TotalFieldStyle)
		{
		}
		public override IRenderer GetNextRenderer()
		{
			return new TotalFieldRenderer((Cell)modelElement, name);
		}

		public static PdfFormField TotalFieldStyle(PdfFormField field)
		{
			field
			.SetReadOnly(true)
			.SetAdditionalAction(PdfName.K, PdfAction.CreateJavaScript("AFNumber_Format(0, 1, 0, 0, '', true);"))
			.SetJustification(TextAlignment.RIGHT)
			.SetFontSize(10)
			.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));

			var w = field.GetFirstFormAnnotation();
			w.SetBackgroundColor(null);

			return (PdfFormField)field;
		}
	}

	public class FieldCellRenderer : CellRenderer
	{
		protected string name;
		protected Func<PdfFormField, PdfFormField> styler;

		public FieldCellRenderer(Cell modelElement, string name, Func<PdfFormField, PdfFormField> styler) : base(modelElement)
		{
			this.name = name;
			this.styler = styler;
		}
	
		public FieldCellRenderer(Cell modelElement, string name) : base(modelElement)
		{
			this.name = name;

		}
		public override IRenderer GetNextRenderer()
		{
			return new FieldCellRenderer((Cell)modelElement, name, styler);
		}

		public override void Draw(DrawContext drawContext)
		{
			PdfDocument thisDoc = drawContext.GetDocument();

			var pttb = new TextFormFieldBuilder(thisDoc, name);

			Rectangle rect = GetOccupiedAreaBBox();

			PdfTextFormField dataField = pttb
				.SetWidgetRectangle(rect)
				.CreateText();
			
			styler(dataField); 


			var form = PdfAcroForm.GetAcroForm(thisDoc, true);
			form.AddField(dataField);

			base.Draw(drawContext);
		}
	}

	public class ComboboxCellRenderer : CellRenderer
	{
		// the name of the combo box
		protected internal String name;
		protected internal string[][] optsarray;

		public ComboboxCellRenderer(Cell modelElement, string name, List<KeyValuePair<string, string>> options)
			: base(modelElement)
		{
			this.name = name;

			String[][] optionsArray = new String[options.Count][];
			for (int i = 0; i < options.Count; i++)
			{
				var kv = options[i];
				optionsArray[i] = new String[2];
				optionsArray[i][0] = kv.Key;
				optionsArray[i][1] = kv.Value;
			}
			this.optsarray = optionsArray;

		}

		public ComboboxCellRenderer(Cell modelElement, string name, string[][] optsarray)
				: base(modelElement)
		{
			this.name = name;
			this.optsarray = optsarray;

		}

		// If renderer overflows on the next area, iText uses getNextRender() method to create a renderer for the overflow part.
		// If getNextRenderer isn't overriden, the default method will be used and thus a default rather than custom
		// renderer will be created
		public override IRenderer GetNextRenderer()
		{
			return new ComboboxCellRenderer((Cell)modelElement, name, optsarray);
		}

		public override void Draw(DrawContext drawContext)
		{

			PdfDocument thisDoc = drawContext.GetDocument();

			// Define the coordinates of the middle

			// Define the position of a check box that measures 20 by 20
			Rectangle rect = GetOccupiedAreaBBox();


			var combo = new ChoiceFormFieldBuilder(thisDoc, name)
				.SetWidgetRectangle(rect)
				.SetOptions(optsarray)
				.CreateComboBox();

			var w = combo.GetFirstFormAnnotation();
			w.SetBackgroundColor(Colors.WebColors.GetRGBColor("OldLace"));
			w.SetBorderColor(ColorConstants.LIGHT_GRAY)
				.SetBorderWidth(1);

			var form = PdfAcroForm.GetAcroForm(thisDoc, true);
			form.AddField(combo);
		}
	}
}
