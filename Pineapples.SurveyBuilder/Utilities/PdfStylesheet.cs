using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Forms.Fields;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.XMP.Impl;
using iText.Layout;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

using System;


namespace surveybuilder
{

	public class PdfStyle
	{
		// Font properties
		public int FontSize { get; set; }
		public Color FontColor { get; set; }
		public string FontName { get; set; }
		public bool FontBold { get; set; }
		public bool FontItalic { get; set; }
		public bool FontUnderline { get; set; }

		// Paragraph properties
		public TextAlignment TextAlignment { get; set; }
		public float LineSpacing { get; set; }
		public bool KeepTogether { get; set; }
		public bool KeepWithNext { get; set; }
		public float IndentationLeft { get; set; }
		public float IndentationRight { get; set; }
		public float SpacingBefore { get; set; }
		public float SpacingAfter { get; set; }

		// Border properties
		public Border TopBorder { get; set; }
		public Border BottomBorder { get; set; }
		public Border LeftBorder { get; set; }
		public Border RightBorder { get; set; }
		public Border Border { get; set; }

		// background (for cells)
		public Color BackgroundColor { get; set; }
		public string BackgroundImage { get; set; }
		public int Height = 0;
		public int ColSpan = 1;
		public int RowSpan = 1;

		// Constructors
		public PdfStyle()
		{
			// Font properties
			FontSize = 10;
			FontColor = ColorConstants.BLACK;
			FontName = "Helvetica";
			FontBold = false;
			FontItalic = false;
			FontUnderline = false;
			TextAlignment = TextAlignment.LEFT;
			LineSpacing = 1.2f;
			KeepTogether = false;
			KeepWithNext = false;
			IndentationLeft = 0;
			IndentationRight = 0;
			SpacingBefore = 0;
			SpacingAfter = 0;
			TopBorder = null;
			BottomBorder = null;
			LeftBorder = null;
			RightBorder = null;
		}

		public PdfStyle(PdfStyle inherits)
		{
			FontSize = inherits.FontSize;
			FontColor = inherits.FontColor;
			FontName = inherits.FontName;
			FontBold = inherits.FontBold;
			FontItalic = inherits.FontItalic;
			FontUnderline = inherits.FontUnderline;
			TextAlignment = inherits.TextAlignment;
			LineSpacing = inherits.LineSpacing;
			KeepTogether = inherits.KeepTogether;
			KeepWithNext = inherits.KeepWithNext;
			IndentationLeft = inherits.IndentationLeft;
			IndentationRight = inherits.IndentationRight;
			SpacingBefore = inherits.SpacingBefore;
			SpacingAfter = inherits.SpacingAfter;
			TopBorder = inherits.TopBorder;
			BottomBorder = inherits.BottomBorder;
			LeftBorder = inherits.LeftBorder;
			RightBorder = inherits.RightBorder;
			Border = inherits.Border;
			BackgroundColor = inherits.BackgroundColor;
			Height = inherits.Height;
		}

		// Apply method
		public Paragraph Apply(Paragraph paragraph)
		{
			paragraph.SetFontSize(FontSize)
				.SetFontColor(FontColor)
				.SetTextAlignment(TextAlignment)
				.SetMultipliedLeading(LineSpacing)
				.SetKeepTogether(KeepTogether)
				.SetKeepWithNext(KeepWithNext)
				.SetMarginLeft(IndentationLeft)
				.SetMarginRight(IndentationRight)
				.SetMarginTop(SpacingBefore)
				.SetMarginBottom(SpacingAfter)
				.SetBackgroundColor(BackgroundColor);

			if (FontBold)
				paragraph.SetBold();

			if (FontItalic)
				paragraph.SetItalic();

			if (FontUnderline)
				paragraph.SetUnderline();

			if (TopBorder != null)
				paragraph.SetBorderTop(TopBorder);

			if (BottomBorder != null)
				paragraph.SetBorderBottom(BottomBorder);

			if (LeftBorder != null)
				paragraph.SetBorderLeft(LeftBorder);

			if (RightBorder != null)
				paragraph.SetBorderRight(RightBorder);

			return paragraph;
		}

		// applytocell
		public Cell ApplyCell(Cell c)
		{
			if (Border != null)
				c.SetBorder(Border);

			if (TopBorder != null)
				c.SetBorderTop(TopBorder);

			if (BottomBorder != null)
				c.SetBorderBottom(BottomBorder);

			if (LeftBorder != null)
				c.SetBorderLeft(LeftBorder);

			if (RightBorder != null)
				c.SetBorderRight(RightBorder);
			if (BackgroundColor != null)
				c.SetBackgroundColor(BackgroundColor);

			if (Height != 0)
				c.SetHeight(Height);

			return c;
		}
	}

	public class PdfStylesheet : Dictionary<string, PdfStyle>
	{
		public PdfStylesheet()
		{
			Add("base", new PdfStyle());
		}

		public Paragraph ApplyStyle(string styleName, string text)
		{
			return ApplyStyle(styleName, new Paragraph(text));
		}
		public Paragraph ApplyStyle(string styleName, Paragraph p)
		{
			return this[styleName].Apply(p);
		}
		public Cell ApplyCell(string styleName, Cell c)
		{
			return this[styleName].ApplyCell(c);
		}
	}



	#region Textfield styles

	public class PdfTextFieldStyle
	{
		public int FontSize { get; set; }
		public Color FontColor { get; set; }
		public string FontName { get; set; }
		public bool FontBold { get; set; }
		public bool FontItalic { get; set; }
		public bool FontUnderline { get; set; }
		public Color BackgroundColor { get; set; }
		public TextAlignment TextAlignment { get; set; }
		public bool ReadOnly { get; set; }
		public bool Visible { get; set; } // New property to control visibility

		// Default constructor with initial values
		public PdfTextFieldStyle()
		{
			FontSize = 10;
			FontColor = ColorConstants.BLACK;
			FontName = "Helvetica";
			FontBold = false;
			FontItalic = false;
			FontUnderline = false;
			BackgroundColor = ColorConstants.WHITE;
			TextAlignment = TextAlignment.LEFT;
			ReadOnly = false;
			Visible = true; // By default, the field is visible
		}

		// Constructor to inherit styles from another PdfTextFieldStyle
		public PdfTextFieldStyle(PdfTextFieldStyle inherits)
		{
			FontSize = inherits.FontSize;
			FontColor = inherits.FontColor;
			FontName = inherits.FontName;
			FontBold = inherits.FontBold;
			FontItalic = inherits.FontItalic;
			FontUnderline = inherits.FontUnderline;
			BackgroundColor = inherits.BackgroundColor;
			TextAlignment = inherits.TextAlignment;
			ReadOnly = inherits.ReadOnly;
			Visible = inherits.Visible;
		}

		// Apply method
		public PdfTextFormField Apply(PdfTextFormField field)
		{
			field.SetFontSize(FontSize)
				 .SetColor(FontColor)
				 //.SetFontAndSize(iText.Kernel.Font.PdfFontFactory.CreateFont(FontName), FontSize)
				 .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(FontName));
			field
				 .SetJustification(TextAlignment == TextAlignment.LEFT ? TextAlignment.LEFT :
								   TextAlignment == TextAlignment.CENTER ? TextAlignment.CENTER :
								   TextAlignment == TextAlignment.RIGHT ? TextAlignment.RIGHT : TextAlignment.LEFT)
				 .SetReadOnly(ReadOnly);

			//if (!Visible)
			//	field.SetVisibility(PdfFormField.HIDDEN);

			//if (FontBold)
			//	field.SetBold();

			//if (FontItalic)
			//	field.SetItalic();

			//if (FontUnderline)
			//	field.SetUnderline();

			return field;
		}
	}

	public class PdfTextFieldStylesheet : Dictionary<string, PdfTextFieldStyle>
	{
		public PdfTextFieldStylesheet()
		{
			PdfTextFieldStyle baseStyle = new PdfTextFieldStyle();
			Add("base", baseStyle);

			PdfTextFieldStyle inputFieldBase = new PdfTextFieldStyle(baseStyle)
			{
				BackgroundColor = ColorConstants.LIGHT_GRAY,
				FontColor = ColorConstants.BLACK
			};
			Add("inputFieldBase", inputFieldBase);

			Add("inputFieldSmall", new PdfTextFieldStyle(inputFieldBase)
			{
				FontSize = 8
			});

			Add("inputFieldLarge", new PdfTextFieldStyle(inputFieldBase)
			{
				FontSize = 14
			});

			Add("inputFieldReadOnly", new PdfTextFieldStyle(inputFieldBase)
			{
				ReadOnly = true,
				BackgroundColor = ColorConstants.GRAY
			});

			Add("inputFieldHidden", new PdfTextFieldStyle(inputFieldBase)
			{
				Visible = false // Set the visibility to false
			});
		}
	}
	#endregion

	#region Table styles

	/// <summary>
	/// Represents a stylesheet for managing table styles in a PDF document.
	/// This is done a bit more "directly" then the previous <see cref="PdfTextFieldStylesheet"/>
	/// One style othe the other should be chosen and then remain consistent.
	/// </summary>
	public class PdfTableStylesheet
	{
		/// <summary>
		/// A collection of base styles available for table elements.
		/// </summary>
		public PdfStylesheet styles = new PdfStylesheet();

		/// <summary>
		/// Initializes a new instance of the <see cref="PdfTableStylesheet"/> class.
		/// Sets up predefined styles for table headers, header cells, and subheader cells.
		/// </summary>
		public PdfTableStylesheet()
		{
			// Define colors using hexadecimal values
			// Colors eventually to move to a ThemeColors.cs (something like WebColors.cs but only necessary colors
			// with better theming design
			string color1 = "eaeaea"; // Light gray for potential use
			string color2 = "cccccc"; // Medium gray for potential use
			string color3 = "a8a8a8"; // Darker gray for subheader cells

			// Define and add styles
			styles.Add("tablebase",
				new PdfStyle(styles["base"])
				{
					FontSize = 10
				});

			styles.Add("tableheader",
				new PdfStyle(styles["base"])
				{
					FontSize = 10,
					TextAlignment = TextAlignment.CENTER,
					FontBold = true
				});

			styles.Add("tablerowheader",
				new PdfStyle(styles["base"])
				{
					TextAlignment = TextAlignment.LEFT
				});

			styles.Add("tablerowheadertotal",
				new PdfStyle(styles["tablerowheader"])
				{
					FontBold = true
				});

			styles.Add("tableheadercell",
				new PdfStyle(styles["base"])
				{
					BackgroundColor = ColorConstants.LIGHT_GRAY
				});

			// Define and add a style for table subheader cells
			styles.Add("tablesubheadercell",
				new PdfStyle(styles["base"])
				{
					BackgroundColor = iText.Kernel.Colors.WebColors.GetRGBColor(color3)
				});

			styles.Add("gridbase",
				new PdfStyle()
				{
					FontSize = 8
				});

			styles.Add("rowheader",
				new PdfStyle(styles["gridbase"])
				{
					TextAlignment = TextAlignment.RIGHT,
					LineSpacing = 1
				});

			styles.Add("rowheadertotal",
				new PdfStyle(styles["rowheader"])
				{
					FontBold = true
				});

			styles.Add("colheader",
				new PdfStyle(styles["gridbase"])
				{
					TextAlignment = TextAlignment.CENTER,
					FontBold = true
				});

			styles.Add("genderheader",
				new PdfStyle(styles["colheader"]) { FontSize = styles["colheader"].FontSize - 2 });

		}

		/// <summary>
		/// Applies the "tablebase" style to a <see cref="Paragraph"/>.
		/// </summary>
		/// <param name="text">The text to apply the table base style to.</param>
		/// <returns>A <see cref="Paragraph"/> styled as a table base.</returns>
		public Paragraph TableBaseStyle(string text)
		{
			return styles.ApplyStyle("tablebase", text);
		}

		/// <summary>
		/// Applies the "tableheader" style to a <see cref="Paragraph"/>.
		/// </summary>
		/// <param name="text">The text to apply the table header style to.</param>
		/// <returns>A <see cref="Paragraph"/> styled as a table header.</returns>
		public Paragraph TableHeaderStyle(string text)
		{
			return styles.ApplyStyle("tableheader", text);
		}

		/// <summary>
		/// Applies the "tablerowheader" style to a <see cref="Paragraph"/>.
		/// </summary>
		/// <param name="text">The text to apply the table row header style to.</param>
		/// <returns>A <see cref="Paragraph"/> styled as a table row header.</returns>
		public Paragraph TableRowHeaderStyle(string text)
		{
			return styles.ApplyStyle("tablerowheader", text);
		}

		/// <summary>
		/// Applies the "tablerowheadertotal" style to a <see cref="Paragraph"/>.
		/// </summary>
		/// <param name="text">The text to apply the table row header total style to.</param>
		/// <returns>A <see cref="Paragraph"/> styled as a table row header total.</returns>
		public Paragraph TableRowHeaderTotalStyle(string text)
		{
			return styles.ApplyStyle("tablerowheadertotal", text);
		}

		/// <summary>
		/// Applies the "tableheadercell" style to a <see cref="Cell"/>.
		/// </summary>
		/// <param name="cell">The cell to apply the table header cell style to.</param>
		/// <returns>A <see cref="Cell"/> styled as a table header cell.</returns>
		public Cell TableHeaderStyle(Cell cell)
		{
			return styles.ApplyCell("tableheadercell", cell);
		}

		/// <summary>
		/// Applies the "tablesubheadercell" style to a <see cref="Cell"/>.
		/// </summary>
		/// <param name="cell">The cell to apply the table subheader cell style to.</param>
		/// <returns>A <see cref="Cell"/> styled as a table subheader cell.</returns>
		public Cell TableSubHeaderStyle(Cell cell)
		{
			return styles.ApplyCell("tablesubheadercell", cell);
		}

		/// <summary>
		/// Applies the "gridbase" style to a <see cref="Paragraph"/> containing text.
		/// </summary>
		/// <param name="text">The text to style as a grid base.</param>
		/// <returns>A <see cref="Paragraph"/> styled with the "gridbase" style.</returns>
		public Paragraph GridBase(string text)
		{
			return styles.ApplyStyle("gridbase", text);
		}

		/// <summary>
		/// Applies the "rowheader" style to a <see cref="Paragraph"/> containing text.
		/// </summary>
		/// <param name="text">The text to style as a row header.</param>
		/// <returns>A <see cref="Paragraph"/> styled with the "rowheader" style.</returns>
		public Paragraph GridRowHeader(string text)
		{
			return styles.ApplyStyle("rowheader", text);
		}

		/// <summary>
		/// Applies the "rowheadertotal" style to a <see cref="Paragraph"/> containing text.
		/// </summary>
		/// <param name="text">The text to style as a row header total.</param>
		/// <returns>A <see cref="Paragraph"/> styled with the "rowheadertotal" style.</returns>
		public Paragraph GridRowHeaderTotal(string text)
		{
			return styles.ApplyStyle("rowheadertotal", text);
		}

		/// <summary>
		/// Applies the "colheader" style to a <see cref="Paragraph"/> containing text.
		/// </summary>
		/// <param name="text">The text to style as a column header.</param>
		/// <returns>A <see cref="Paragraph"/> styled with the "colheader" style.</returns>
		public Paragraph GridColHeader(string text)
		{
			return styles.ApplyStyle("colheader", text);
		}

		/// <summary>
		/// Applies the "genderheader" style to a <see cref="Paragraph"/> containing text.
		/// </summary>
		/// <param name="text">The text to style as a gender header.</param>
		/// <returns>A <see cref="Paragraph"/> styled with the "genderheader" style.</returns>
		public Paragraph GridGenderHeader(string text)
		{
			return styles.ApplyStyle("genderheader", text);
		}

	}

	#endregion

}
