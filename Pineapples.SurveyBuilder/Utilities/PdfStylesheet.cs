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

using iText.Kernel.Colors;
using System;
using System.Linq;
using surveybuilder.Utilities;
using iText.Forms.Fields.Merging;
using System.Diagnostics;




namespace surveybuilder
{
	public delegate Cell CellStyler(Cell cell);
	public delegate Paragraph ParagraphStyler(Paragraph p);

	public class PdfStyle
	{

		// Constructors
		public PdfStyle() { }

		public PdfStyle(PdfStyle inherits)
		{
			this.inherits = inherits;
			CurrentTheme = inherits.CurrentTheme;
			GetFontColor = inherits.GetFontColor;
			GetBackgroundColor = inherits.GetBackgroundColor;
			GetBorder = inherits.GetBorder;

		}

		// dynamic theming support
		public Theme CurrentTheme { get; set; }

		// inheritance hierarchy
		public PdfStyle inherits = null;

		// A static default base style shared across instances
		private static readonly PdfStyle DefaultBaseStyle = new PdfStyle()
		{
			// Font properties
			FontSize = 10,
			FontColor = ColorConstants.BLACK,
			FontName = "Helvetica",
			FontBold = false,
			FontItalic = false,
			FontUnderline = false,
			TextAlignment = TextAlignment.LEFT,
			LineSpacing = 1.2f,
			KeepTogether = false,
			KeepWithNext = false,
			IndentationLeft = 0,
			IndentationRight = 0,
			SpacingBefore = 0,
			SpacingAfter = 0,
			TopBorder = Border.NO_BORDER,
			BottomBorder = Border.NO_BORDER,
			LeftBorder = Border.NO_BORDER,
			RightBorder = Border.NO_BORDER,
			Border = Border.NO_BORDER,
			//ignore the dynamic colors
			GetFontColor = null,
			GetBackgroundColor = null,
		};

		private PdfStyle baseStyle = DefaultBaseStyle;
		public string Name { get; set; } 

		// Local field for each property
		private int? fontSize;
		private Color fontColor = null;
		private string fontName;
		private bool? fontBold;
		private bool? fontItalic;
		private bool? fontUnderline;
		private TextAlignment? textAlignment;
		private float? lineSpacing;
		private bool? keepTogether;
		private bool? keepWithNext;
		private float? indentationLeft;
		private float? indentationRight;
		private float? spacingBefore;
		private float? spacingAfter;
		private Border topBorder;
		private Border bottomBorder;
		private Border leftBorder;
		private Border rightBorder;
		private Border border;
		private Color backgroundColor;
		private string backgroundImage;

		//dynamic color properties
		public Func<Theme, Color> GetFontColor { get; set; }
		public Func<Theme, Color> GetBackgroundColor { get; set; }
		public Func<Theme, Border> GetBorder { get; set; }
		public Func<Theme, Border> GetTopBorder { get; set; }
		public Func<Theme, Border> GetLeftBorder { get; set; }
		public Func<Theme, Border> GetRightBorder { get; set; }
		public Func<Theme, Border> GetBottomBorder { get; set; }

		// Font properties
		public int FontSize
		{
			get => fontSize ?? inherits?.FontSize ?? baseStyle.FontSize;
			set => fontSize = value;
		}

		public Color FontColor
		{
			
			get => GetFontColor?.Invoke(CurrentTheme)
					??fontColor ?? inherits?.FontColor ?? baseStyle.FontColor;
			set
			{
				fontColor = value;
			}
		}

		public string FontName
		{
			get => fontName ?? inherits?.FontName ?? baseStyle.FontName;
			set => fontName = value;
		}

		public bool FontBold
		{
			get => fontBold ?? inherits?.FontBold ?? baseStyle.FontBold;
			set => fontBold = value;
		}

		public bool FontItalic
		{
			get => fontItalic ?? inherits?.FontItalic ?? baseStyle.FontItalic;
			set => fontItalic = value;
		}

		public bool FontUnderline
		{
			get => fontUnderline ?? inherits?.FontUnderline ?? baseStyle.FontUnderline;
			set => fontUnderline = value;
		}

		// Paragraph properties
		public TextAlignment TextAlignment
		{
			get => textAlignment ?? inherits?.TextAlignment ?? baseStyle.TextAlignment;
			set => textAlignment = value;
		}

		public float LineSpacing
		{
			get => lineSpacing ?? inherits?.LineSpacing ?? baseStyle.LineSpacing;
			set => lineSpacing = value;
		}

		public bool KeepTogether
		{
			get => keepTogether ?? inherits?.KeepTogether ?? baseStyle.KeepTogether;
			set => keepTogether = value;
		}

		public bool KeepWithNext
		{
			get => keepWithNext ?? inherits?.KeepWithNext ?? baseStyle.KeepWithNext;
			set => keepWithNext = value;
		}

		public float IndentationLeft
		{
			get => indentationLeft ?? inherits?.IndentationLeft ?? baseStyle.IndentationLeft;
			set => indentationLeft = value;
		}

		public float IndentationRight
		{
			get => indentationRight ?? inherits?.IndentationRight ?? baseStyle.IndentationRight;
			set => indentationRight = value;
		}

		public float SpacingBefore
		{
			get => spacingBefore ?? inherits?.SpacingBefore ?? baseStyle.SpacingBefore;
			set => spacingBefore = value;
		}

		public float SpacingAfter
		{
			get => spacingAfter ?? inherits?.SpacingAfter ?? baseStyle.SpacingAfter;
			set => spacingAfter = value;
		}

		// Border properties
		public Border TopBorder
		{
			get => GetTopBorder?.Invoke(CurrentTheme)
					?? topBorder ?? inherits?.TopBorder ?? baseStyle?.TopBorder;
			set => topBorder = value;
		}

		public Border BottomBorder
		{
			get => GetBottomBorder?.Invoke(CurrentTheme)
					?? bottomBorder ?? inherits?.BottomBorder ?? baseStyle?.BottomBorder;
			set => bottomBorder = value;
		}

		public Border LeftBorder
		{
			get => GetLeftBorder?.Invoke(CurrentTheme)
					?? leftBorder ?? inherits?.LeftBorder ?? baseStyle?.LeftBorder;
			set => leftBorder = value;
		}

		public Border RightBorder
		{
			get => GetRightBorder?.Invoke(CurrentTheme)
					?? rightBorder ?? inherits?.RightBorder ?? baseStyle?.RightBorder;
			set => rightBorder = value;
		}

		public Border Border
		{
			get => GetBorder?.Invoke(CurrentTheme)
					??border ?? inherits?.Border ?? baseStyle?.Border;
			set => border = value;
		}

		// Background properties
		public Color BackgroundColor
		{
			get => GetBackgroundColor?.Invoke(CurrentTheme)
					?? backgroundColor 
					?? inherits?.BackgroundColor
					?? baseStyle?.BackgroundColor;

			set => backgroundColor = value;
		}

		public string BackgroundImage
		{
			get => backgroundImage ?? inherits?.BackgroundImage ?? baseStyle.BackgroundImage;
			set => backgroundImage = value;
		}


		public int Height = 0;
		public int ColSpan = 1;
		public int RowSpan = 1;

		// Apply method

		public T Apply<T>(T t) where T : IElement
		{
			if (t == null)
				throw new ArgumentNullException(nameof(t));

			// Use a traditional switch statement to handle different types
			if (t is Paragraph p)
			{
				return (T)(IElement)Apply(p);
			}
			else if (t is Cell c)
			{
				return (T)(IElement)Apply(c);
			}
			//else if (t is Table tbl)
			//{
			//	return (T)(IElement)Apply(tbl);
			//}
			else
			{
				throw new NotSupportedException($"Type {typeof(T).Name} is not supported by ApplyStyle.");
			}
		}

		public Paragraph Apply(string text)
		{
			return Apply(new Paragraph(text));
		}

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

		// applytocell - this will cascade the style to any text
		public Cell Apply(Cell c)
		{

			if (Border != null)
				c.SetBorder(Border);
			else
				c.SetBorder(Border.NO_BORDER);

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

			// now cascade to apply the style to any paragraphs in the cell
			foreach (var element in c.GetChildren()
			.Where(x => x.GetType() == typeof(Paragraph)))
			{
				PdfStyle cc = Cascader();
				Paragraph p = (Paragraph)element;
				cc.Apply(p);
			}

			return c;
		}
		private PdfStyle Cascader()
		{
			// Use the inheritance constructor and override Border
			return new PdfStyle(this)
			{
				Border = Border.NO_BORDER
			};
		}
	}

	public class PdfStylesheet : Dictionary<string, PdfStyle>
	{

		// Default constructor with the default theme
		public PdfStylesheet()
		{
			Theme = new Theme
			{
				Primary = PredefinedPalettes.Blue,
				Accent = PredefinedPalettes.Pink,
				Warn = PredefinedPalettes.Red,
				Background = PredefinedPalettes.Pink
			};

			InitialiseStyles();
		}

		// Constructor that accepts a Theme
		public PdfStylesheet(Theme theme)
		{
			Theme = theme ?? throw new ArgumentNullException(nameof(theme));
			InitialiseStyles();
		}
		private Theme _theme;

		public Theme Theme
		{
			get => _theme;
			set
			{
				_theme = value ?? throw new ArgumentNullException(nameof(value));
				foreach (var style in this.Values)
				{
					style.CurrentTheme = _theme; // Ensure all styles have the updated theme
				}
			}
		}

		// Use the indexer to support add or update
		// ie this allows you to change the definition of a style associated to a name.
		public new PdfStyle this[string key]
		{
			get => base[key]; // Return the PdfStyle if it exists
			set
			{
				base[key] = value; // Update or add the PdfStyle
				value.Name = key;
				// Ensure the new style has the current theme
				value.CurrentTheme = Theme;
				// make sure any inherits are updated
				foreach (PdfStyle style in this.Values.Where(v => v.inherits?.Name == key))
				{
					style.inherits = value;
				}
			}
		}

		public new void Add(string name, PdfStyle style)
		{
			style.Name = name;
			style.CurrentTheme = _theme; // Ensure all styles have the updated theme
			base.Add(name, style);
		}

		public Paragraph ApplyStyle(string styleName, string text)
		{
			return ApplyStyle(styleName, new Paragraph(text));
		}
		public Paragraph ApplyStyle(string styleName, Paragraph p)
		{
			return this[styleName].Apply(p);
		}
		public Cell ApplyStyle(string styleName, Cell c)
		{
			return this[styleName].Apply(c);
		}
		public T ApplyStyle<T>(string styleName, T t) where T : IElement
		{
			return this[styleName].Apply<T>(t);
		}

		#region Initialisation

		/// <summary>
		/// Initialise the default styles for a stylesheet
		/// These are based on the styles that are predefined in MS Word, and therefore available
		/// to any document.
		/// </summary>
		protected virtual void InitialiseStyles()
		{

			#region Heading styles
			Add("base", new PdfStyle());
			PdfStyle headingbase = new PdfStyle(this["base"])
			{
				FontBold = true,
				FontColor = iText.Kernel.Colors.WebColors.GetRGBColor("606060"),
				KeepWithNext = true,  // Headings often keep with the next paragraph
				SpacingBefore = 10,   // Add some space before the heading
				SpacingAfter = 5      // Add some space after the heading
			};

			// Add the base style for headings to the stylesheet
			Add("headingbase", headingbase);

			// Define Heading 1 style
			Add("Heading 1", new PdfStyle(this["headingbase"])
			{
				FontSize = 24,
				GetBackgroundColor = (theme) => theme.Primary.DefaultHue,
				GetFontColor = (theme) => theme.Primary.TextColor,
			});

			// Define Heading 2 style
			Add("Heading 2", new PdfStyle(this["headingbase"])
			{
				FontSize = 20,
				GetTopBorder = (theme) => new SolidBorder(theme.Accent.DefaultHue, 3),
				GetFontColor = (theme) => theme.Accent.GetColor(200)
			});

			// Define Heading 3 style
			Add("Heading 3", new PdfStyle(this["headingbase"])
			{
				FontSize = 16,
				//FontColor = ColorConstants.ORANGE
			});

			// Define Heading 4 style
			Add("Heading 4", new PdfStyle(this["headingbase"])
			{
				FontSize = 12
				
				//FontColor = ColorConstants.CYAN
			});

			// Define Heading 5 style
			Add("Heading 5", new PdfStyle(this["headingbase"])
			{
				FontSize = 12,
				//FontColor = ColorConstants.GREEN
			});
			#endregion

			Add("Normal", new PdfStyle(this["base"])
			{
				FontSize = 12,
				FontColor = NamedColors.Black,
				SpacingBefore = 5,
				SpacingAfter = 5
			});


		}
		#endregion initialise
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

		#region Constructor - Table Style Definitions 
		/// <summary>
		/// Initializes a new instance of the <see cref="PdfTableStylesheet"/> class.
		/// Sets up predefined styles for table headers, header cells, and subheader cells.
		/// </summary>
		public PdfTableStylesheet()
		{


			// Define colors using hexadecimal values
			// Colors eventually to move to a ThemeColors.cs (something like WebColors.cs but only necessary colors
			// with better theming design
			//string color1 = "eaeaea"; // Light gray for potential use #66CCFF
			//string color2 = "cccccc"; // Medium gray for potential use #66FFFF
			//string color3 = "a8a8a8"; // Darker gray for subheader cells

			//string color1 = "66FFFF"; // Light gray for potential use #
			//string color2 = "66CCFF"; // Medium gray for potential use #66FFFF
			//string color3 = "6699FF"; // Darker gray for subheader cells

			string color1 = "66DDFF"; // Light gray for potential use #
			string color2 = "66AAFF"; // Medium gray for potential use #66FFFF
			string color3 = "6677FF"; // Darker gray for subheader cells

			// Define and add styles
			styles.Add("tablebase",
				new PdfStyle(styles["base"])
				{
					FontSize = 10,
					Border = new SolidBorder(ColorConstants.LIGHT_GRAY, 1, (float).5)
				});

			styles.Add("tableheader",
				new PdfStyle(styles["tablebase"])
				{
					TextAlignment = TextAlignment.CENTER,
					FontBold = true,
					BackgroundColor = iText.Kernel.Colors.WebColors.GetRGBColor(color1)
				});

			styles.Add("tablerowheader",
				new PdfStyle(styles["tablebase"])
				{
					TextAlignment = TextAlignment.LEFT

				});

			styles.Add("tablerowheadertotal",
				new PdfStyle(styles["tablebase"])
				{
					TextAlignment = TextAlignment.RIGHT,
					FontBold = true
				});

			// Define and add a style for table subheader cells
			styles.Add("tablesubheader",
				new PdfStyle(styles["tablebase"])
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

			styles.Add("abstractcell",
				new PdfStyle(styles["tablebase"])
				{
					TextAlignment = TextAlignment.CENTER,
					Border = Border.NO_BORDER
				});

			// seed the CellStyleFactory with tablebase
			// which will get applied to everything
			CellStyleFactory.BaseStyle = styles["tablebase"];

		}
		#endregion

		#region Convenience methods to implement styles
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

		public T TableRowHeaderStyle<T>(T target) where T : IElement
		{
			return styles.ApplyStyle("tablerowheader", target);
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
		public Cell TableBaseStyle(Cell cell)
		{
			return styles.ApplyStyle("tablebase", cell);
		}

		/// <summary>
		/// Applies the "tableheadercell" style to a <see cref="Cell"/>.
		/// </summary>
		/// <param name="cell">The cell to apply the table header cell style to.</param>
		/// <returns>A <see cref="Cell"/> styled as a table header cell.</returns>
		public Cell TableHeaderStyle(Cell cell)
		{
			return styles.ApplyStyle("tableheader", cell);

		}

		/// <summary>
		/// Applies the "tablesubheadercell" style to a <see cref="Cell"/>.
		/// </summary>
		/// <param name="cell">The cell to apply the table subheader cell style to.</param>
		/// <returns>A <see cref="Cell"/> styled as a table subheader cell.</returns>
		public Cell TableSubHeaderStyle(Cell cell)
		{
			return styles.ApplyStyle("tablesubheader", cell);
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

		/// <summary>
		/// Applies the "gabstract" style to a <see cref="Paragraph"/> containing text.
		/// </summary>
		/// <param name="text">The text to style as a gender header.</param>
		/// <returns>A <see cref="Paragraph"/> styled with the "genderheader" style.</returns>
		public Cell AbstractCell(Cell cell)
		{
			return styles.ApplyStyle("abstractcell", cell);
		}

		#endregion

	}

	#endregion

}
