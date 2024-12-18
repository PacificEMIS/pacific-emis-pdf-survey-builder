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
using surveybuilder.Utilities;

namespace surveybuilder
{

	/// <summary>
	/// Make a chekcbox in a table cell, that is part of a mutually exclusive group
	/// Note that the rgrp element needs to be created as a Radio Group, 
	/// and the 'checkboxes start life as radio buttons with a customised Appearance Dictionary
	/// </summary>
	public class CheckBoxGroupCellRenderer : CellRenderer
	{
		// The name of the check box field
		//protected internal String name;
		protected internal String export;
		protected internal PdfButtonFormField rgrp;

		protected PdfDictionary ap;

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
		public CheckBoxGroupCellRenderer(Cell modelElement, PdfButtonFormField rgrp, PdfDictionary ap, object export)
			: base(modelElement)
		{
			this.export = export.ToString();
			this.ap = ap;
			this.rgrp = rgrp;
		}

		// If renderer overflows on the next area, iText uses getNextRender() method to create a renderer for the overflow part.
		// If getNextRenderer isn't overriden, the default method will be used and thus a default rather than custom
		// renderer will be created
		public override IRenderer GetNextRenderer()
		{
			return new CheckBoxGroupCellRenderer((Cell)modelElement, rgrp,ap, export);
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

			var chkbtn = new RadioFormFieldBuilder(thisDoc, "tmp")
				.CreateRadioButton(export, rect);

			/// call rgrp.SetFieldFlags(0) to get the expected behaviours and appearance of the group
			/// of buttons - Allows toggling and removes the radio group flag 
			rgrp.SetFieldFlags(0);

			PdfDictionary wdic = chkbtn.GetPdfObject();
			PdfDictionary apDic = ap.GetAsDictionary(PdfName.AP);
			PdfDictionary mkDic = ap.GetAsDictionary(PdfName.MK);

			wdic.Put(PdfName.AP, apDic);
			wdic.Put(PdfName.MK, mkDic);
			wdic.Put(PdfName.AS, new PdfName("Off"));

			rgrp.AddKid(chkbtn);
			// this would seem to add rgrp over and over,
			// but it is necessary in order for this widget to get drawn
			// Note that some widgets seems to end up in Kids twice. this is resolved by a clean up
			// pass at the end of each builder.
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

			w.SetBackgroundColor(readOnly?null:Colors.WebColors.GetRGBColor("OldLace"));

			if (value != null)
			{
				dataField.SetValue(value);
			}

			PdfAcroForm.GetAcroForm(thisDoc, true).AddField(dataField);

			base.Draw(drawContext);



		}
	}

	/// <summary>
	/// A custom renderer for creating and managing numeric input fields in PDF cells.
	/// This renderer uses JavaScript actions for formatting and keystroke validation.
	/// </summary>
	public class NumberFieldCellRenderer : FieldCellRenderer
	{

		string formataction = string.Empty;
		string keystrokeaction = string.Empty;

		/// <summary>
		/// Initializes a new instance of the <see cref="NumberFieldCellRenderer"/> class with specified formatting options.
		/// </summary>
		/// <param name="modelElement">The cell to which this renderer is applied.</param>
		/// <param name="name">The unique name of the numeric field.</param>
		/// <param name="value">The optional numeric value to prepopulate.</param>
		/// <param name="decimals">
		/// Specifies the number of decimal places to display:
		/// <list type="bullet">
		/// <item><description><c>0</c>: No decimal places.</description></item>
		/// <item><description><c>1</c> or higher: Specifies the number of decimal places (e.g., <c>2</c> for two decimal places).</description></item>
		/// </list>
		/// </param>
		/// <param name="sepStyle">
		/// Defines the style of the thousands separator:
		/// <list type="bullet">
		/// <item><description><c>0</c>: No separator.</description></item>
		/// <item><description><c>1</c>: Comma (e.g., <c>1,234</c>).</description></item>
		/// <item><description><c>2</c>: Period (e.g., <c>1.234</c>).</description></item>
		/// <item><description><c>3</c>: Space (e.g., <c>1 234</c>).</description></item>
		/// </list>
		/// </param>
		/// <param name="negStyle">
		/// Determines how negative numbers are displayed:
		/// <list type="bullet">
		/// <item><description><c>0</c>: Minus sign before the number (e.g., <c>-1234</c>).</description></item>
		/// <item><description><c>1</c>: Parentheses around the number (e.g., <c>(1234)</c>).</description></item>
		/// <item><description><c>2</c>: Red color for negative numbers (if supported by the viewer).</description></item>
		/// </list>
		/// </param>
		/// <param name="currStyle">
		/// Specifies whether the currency symbol should be used:
		/// <list type="bullet">
		/// <item><description><c>0</c>: No currency symbol.</description></item>
		/// <item><description><c>1</c>: Currency symbol is used (e.g., <c>$1234</c>).</description></item>
		/// </list>
		/// </param>
		/// <param name="strCurrency">
		/// The currency symbol to display (if <paramref name="currStyle"/> is <c>1</c>):
		/// <list type="bullet">
		/// <item><description>Examples: <c>$</c>, <c>€</c>, <c>£</c>.</description></item>
		/// </list>
		/// </param>
		/// <param name="prePend">
		/// Determines the position of the currency symbol:
		/// <list type="bullet">
		/// <item><description><c>true</c>: Prepends the currency symbol (e.g., <c>$1234</c>).</description></item>
		/// <item><description><c>false</c>: Appends the currency symbol (e.g., <c>1234$</c>).</description></item>
		/// </list>
		/// </param>
		public NumberFieldCellRenderer(Cell modelElement, string name, float? value = null,
			int decimals = 0, AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT, AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
			: base(modelElement, name)
		{
			formataction = Actions.NFormatJs(decimals, sepStyle, negStyle, currStyle, strCurrency, prePend);
			keystrokeaction = Actions.NKeystrokeJs(decimals, sepStyle, negStyle, currStyle, strCurrency, prePend);
			base.styler = FieldStyle;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NumberFieldCellRenderer"/> class with predefined JavaScript actions.
		/// </summary>
		/// <param name="modelElement">The cell to which this renderer is applied.</param>
		/// <param name="name">The unique name of the numeric field.</param>
		/// <param name="formataction">The JavaScript action for formatting the field.</param>
		/// <param name="keystrokeaction">The JavaScript action for validating keystrokes.</param>
		public NumberFieldCellRenderer(Cell modelElement, string name, string formataction, string keystrokeaction)
			: base(modelElement, name)
		{
			this.formataction = formataction;
			this.keystrokeaction = keystrokeaction;
			base.styler = FieldStyle;
		}
		/// <summary>
		/// Creates the next renderer for this field.
		/// This is used internally by iText's rendering engine for re-rendering operations.
		/// </summary>
		/// <returns>A new instance of <see cref="NumberFieldCellRenderer"/>.</returns>
		public override IRenderer GetNextRenderer()
		{
			return new NumberFieldCellRenderer((Cell)modelElement, name, formataction, keystrokeaction);
		}

		/// <summary>
		/// Styles the numeric field with predefined properties.
		/// </summary>
		/// <param name="field">The <see cref="PdfFormField"/> to style.</param>
		/// <returns>The styled <see cref="PdfFormField"/>.</returns>
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
	/// A custom renderer for creating and managing date input fields in PDF cells.
	/// This renderer uses JavaScript actions for date validation and formatting, and supports optional prepopulated date values.
	/// TODO Date cell renderer not fully working
	/// </summary>
	public class DateFieldCellRenderer : FieldCellRenderer
	{
		private readonly string dateFormat;
		private readonly string validationAction;
		private readonly string keystrokeAction;
		private readonly DateTime? dateValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="DateFieldCellRenderer"/> class.
		/// </summary>
		/// <param name="modelElement">The cell to which this renderer is applied.</param>
		/// <param name="name">The unique name of the date field.</param>
		/// <param name="dateValue">An optional prepopulated date value for the field.</param>
		/// <param name="dateFormat">The desired date format for input validation and display (e.g., "MM/dd/yyyy").</param>
		public DateFieldCellRenderer(Cell modelElement, string name, DateTime? dateValue = null, string dateFormat = "MM/dd/yyyy")
			: base(modelElement, name)
		{
			this.dateValue = dateValue;
			this.dateFormat = dateFormat;

			// Define JavaScript actions for date validation and keystroke formatting
			validationAction = $"AFDate_FormatEx('{dateFormat}');";
			keystrokeAction = $"AFDate_Keystroke('{dateFormat}');";

			base.styler = FieldStyle; // Apply field styling
		}

		/// <summary>
		/// Creates the next renderer for this field.
		/// This is used internally by iText's rendering engine for re-rendering operations.
		/// </summary>
		/// <returns>A new instance of <see cref="DateFieldCellRenderer"/>.</returns>
		public override IRenderer GetNextRenderer()
		{
			return new DateFieldCellRenderer((Cell)modelElement, name, dateValue, dateFormat);
		}

		/// <summary>
		/// Applies styling to the date field, including formatting, alignment, background color, and optional prepopulated value.
		/// </summary>
		/// <param name="field">The <see cref="PdfFormField"/> to style.</param>
		/// <returns>The styled <see cref="PdfFormField"/>.</returns>
		public PdfFormField FieldStyle(PdfFormField field)
		{
			// Set the initial value if provided
			if (dateValue.HasValue)
			{
				field.SetValue(dateValue.Value.ToString(dateFormat));
			}

			field
				.SetReadOnly(false)
				.SetAdditionalAction(PdfName.F, PdfAction.CreateJavaScript(validationAction)) // Format action
				.SetAdditionalAction(PdfName.K, PdfAction.CreateJavaScript(keystrokeAction)) // Keystroke action
				.SetJustification(TextAlignment.CENTER) // Center-align for dates
				.SetFontSize(10)
				.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));

			// Set the background color for the date field
			var annotation = field.GetFirstFormAnnotation();
			annotation.SetBackgroundColor(Colors.WebColors.GetRGBColor("OldLace"));

			return field;
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


			;
		}
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
		protected internal PdfArray optsarray;

		public ComboboxCellRenderer(Cell modelElement, string name, PdfArray optsarray)
				: base(modelElement)
		{
			this.name = name;
			this.optsarray = optsarray;

		}
		// If renderer overflows on the next area, iText uses getNextRenderer() method to create a renderer for the overflow part.
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

	/// <summary>
	/// Renderer to draw a push button inside a table cell.
	/// </summary>
	public class PushButtonCellRenderer : CellRenderer
	{
		private readonly string buttonName;
		private readonly string buttonLabel;
		private readonly string js;			
		private readonly Action<PdfButtonFormField> configurer;

		public PushButtonCellRenderer(Cell modelElement, string buttonName, string buttonLabel,
			string js, Action<PdfButtonFormField> configurer = null)
			: base(modelElement)
		{
			this.buttonName = buttonName;
			this.buttonLabel = buttonLabel;
			this.js = js;
			this.configurer = configurer;
		}
		public PushButtonCellRenderer(Cell modelElement, string buttonName, string buttonLabel,
			Action<PdfButtonFormField> configurer = null)
			: base(modelElement)
		{
			this.buttonName = buttonName;
			this.buttonLabel = buttonLabel;
			this.js = string.Empty;
			this.configurer = configurer;
		}

		public override IRenderer GetNextRenderer()
		{
			return new PushButtonCellRenderer((Cell)modelElement, buttonName, buttonLabel,js, configurer);
		}

		public override void Draw(DrawContext drawContext)
		{
			PdfDocument pdfDocument = drawContext.GetDocument();

			// Get the bounding rectangle of the current cell
			Rectangle rect = GetOccupiedAreaBBox();

			// Create the PushButtonFormFieldBuilder
			PushButtonFormFieldBuilder builder = new PushButtonFormFieldBuilder(pdfDocument, buttonName)
				.SetWidgetRectangle(rect)
				.SetCaption(buttonLabel);

			PdfButtonFormField button = builder.CreatePushButton();
			
			if (!string.IsNullOrEmpty(js))
			{
				button.SetAdditionalAction(PdfName.U, PdfAction.CreateJavaScript(js));
			}
			// Apply additional configurations if provided
			configurer?.Invoke(button);

			// Add the button to the AcroForm
			PdfAcroForm.GetAcroForm(pdfDocument, true).AddField(button);

			// Call base class to handle other drawing
			base.Draw(drawContext);
		}
	}
}
