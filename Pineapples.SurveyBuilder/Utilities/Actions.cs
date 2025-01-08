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
using System.Reflection;
using System.CodeDom;

namespace surveybuilder.Utilities
{
	public enum AF_SEPSTYLE
	{
		/// <summary>
		/// Comma-separated thousands with a period for decimals.
		/// Example: 1,234.56
		/// </summary>
		COMMA_DOT = 0,

		/// <summary>
		/// No thousands separator with a period for decimals.
		/// Example: 1234.56
		/// </summary>
		NONE_DOT = 1,

		/// <summary>
		/// Period-separated thousands with a comma for decimals.
		/// Example: 1.234,56
		/// </summary>
		DOT_COMMA = 2,

		/// <summary>
		/// No thousands separator with a comma for decimals.
		/// Example: 1234,56
		/// </summary>
		NONE_COMMA = 3
	}
	public enum AF_NEGSTYLE
	{
		/// <summary>
		/// Negative sign before the number.
		/// Example: -1234.56
		/// </summary>
		MINUS = 0,

		/// <summary>
		/// Negative sign before the number with red text.
		/// Example: -1234.56 (displayed in red)
		/// </summary>
		RED_MINUS = 1,

		/// <summary>
		/// Negative number enclosed in parentheses.
		/// Example: (1234.56)
		/// </summary>
		PARENS = 2,

		/// <summary>
		/// Negative number enclosed in parentheses with red text.
		/// Example: (1234.56) (displayed in red)
		/// </summary>
		RED_PARENS = 3
	}

	internal class Actions
	{
		/// <summary>
		/// create a numeric format action using the built-in acrobat macro
		/// </summary>
		public static PdfAction NFormat(int decimals = 0,
			AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT,
			AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{
			return AF("Format", decimals, sepStyle, negStyle, currStyle, strCurrency, prePend);
		}
		public static PdfAction NKeystroke(int decimals = 0, AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT, AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{
			return AF("Keystroke", decimals, sepStyle, negStyle, currStyle, strCurrency, prePend);
		}

		private static PdfAction AF(string type, int decimals = 0, AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT, AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{
			return PdfAction.CreateJavaScript(AFjs(type, decimals, sepStyle, negStyle, currStyle, strCurrency, prePend));
		}
		public static String NFormatJs(int decimals = 0, AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT, AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{
			return AFjs("Format", decimals, sepStyle, negStyle, currStyle, strCurrency, prePend);

		}
		public static string NKeystrokeJs(int decimals = 0, AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT, AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{
			return AFjs("Keystroke", decimals, sepStyle, negStyle, currStyle, strCurrency, prePend);
		}
		private static string AFjs(string type, int decimals = 0, AF_SEPSTYLE sepStyle = AF_SEPSTYLE.NONE_DOT, AF_NEGSTYLE negStyle = AF_NEGSTYLE.MINUS,
			int currStyle = 0, string strCurrency = "", bool prePend = true)
		{

			string args = $"{decimals:0}, {(int)sepStyle:0}, {(int)negStyle:0}, {currStyle:0}, '{strCurrency}', {(prePend ? "true" : "false")}";
			return ($"AFNumber_{type}({args});");
		}

		public static PdfAction NValidate(float min = 0, float max = 9999, bool integerRequired = true)
		{
			return PdfAction.CreateJavaScript(NValidateJs(min, max, integerRequired));
		}
		public static string NValidateJs(float min, float max, bool integerRequired)
		{
			string args = $"event, {min:0}, {max:0},  {(integerRequired ? "true" : "false")}";
			return ($"p.numchk({args});");
		}

	}
}
