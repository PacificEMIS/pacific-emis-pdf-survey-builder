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

namespace itext4.Utilities
{
	internal class Actions
	{
		/// <summary>
		/// create a numeric format action using the built-in acrobat macro
		/// </summary>
		public static PdfAction NFormat(int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{
			return AF("Format", decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend);
		}
		public static PdfAction NKeystroke(int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{
			return AF("Keystroke", decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend);
		}

		private static PdfAction AF(string type, int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{
			return PdfAction.CreateJavaScript(AFjs(type, decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend));
		}
		public static String NFormatJs(int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{
			return AFjs("Format", decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend);

		}
		public static string NKeystrokeJs(int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{
			return AFjs("Keystroke", decimals, sepStyle, negStyle, currStyle, strCurrency, bPrePend);
		}
		private static string AFjs(string type, int decimals = 0, int sepStyle = 0, int negStyle = 0,
			int currStyle = 0, string strCurrency = "", bool bPrePend = true)
		{

			string args = $"{decimals:0}, {sepStyle:0}, {negStyle:0}, {currStyle:0}, '{strCurrency}', {(bPrePend ? "true" : "false")}";
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
