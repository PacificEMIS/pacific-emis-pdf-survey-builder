using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using iText.Forms;
using iText.Forms.Fields;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System.Runtime.CompilerServices;
using iText.Kernel.Colors;


namespace surveybuilder
{
	public static class ColorExtensions
	{
		/// <summary>
		/// Extension method to get the contrast color of a given color, using standard algorhythm
		/// Use this to derive a textcolor from a backgroundcolor in a Style
		/// Can be used with dynamic styles
		/// </summary>
		/// <param name="color">the background color</param>
		/// <returns>the contrast color - black or white</returns>
		public static Color Contrast(this Color color)
		{
			// Get the color values as an array of floats (normalized to 0-1)
			float[] rgbValues = color.GetColorValue();

			// Calculate luminance using the standard formula
			double luminance = GetLuminance(rgbValues[0], rgbValues[1], rgbValues[2]);

			// Return black or white based on luminance
			return luminance > 0.5 ? ColorConstants.BLACK : ColorConstants.WHITE;
		}

		private static double GetLuminance(float red, float green, float blue)
		{
			// Convert normalized RGB values to linear RGB
			double r = (red <= 0.03928) ? (red / 12.92) : Math.Pow((red + 0.055) / 1.055, 2.4);
			double g = (green <= 0.03928) ? (green / 12.92) : Math.Pow((green + 0.055) / 1.055, 2.4);
			double b = (blue <= 0.03928) ? (blue / 12.92) : Math.Pow((blue + 0.055) / 1.055, 2.4);

			// Calculate luminance based on the sRGB standard
			return 0.2126 * r + 0.7152 * g + 0.0722 * b;
		}
	}

}
