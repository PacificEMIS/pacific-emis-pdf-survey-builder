using iText.Kernel.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using iText.Kernel.Colors;

namespace surveybuilder
{

	public class Palette
	{
		public string Name { get; set; }
		public Dictionary<int, Color> Hues { get; private set; }
		public Color TextColor { get; set; } // Default text color
		public Color SecondaryTextColor { get; set; } // Optional secondary text color

		public Palette()
		{
			Hues = new Dictionary<int, Color>();
		}

		// Get a color by hue
		public Color GetColor(int hue)
		{
			if (Hues.TryGetValue(hue, out var color))
			{
				return color;
			}

			throw new ArgumentException($"Hue {hue} not defined in the palette.");
		}

		// A quick way to get the default hue (500)
		public Color DefaultHue => GetColor(500);

		// Unified method to add or replace a hue
		private Palette AddOrReplaceHue(int hue, Color color, bool allowReplace)
		{
			if (Hues.ContainsKey(hue) && !allowReplace)
			{
				throw new ArgumentException($"Hue {hue} already exists in the palette. Use ReplaceHue to update it.");
			}

			Hues[hue] = color; // Add or replace
			return this; // Fluent API
		}

		// Public AddHue method
		public Palette AddHue(int hue, Color color) => AddOrReplaceHue(hue, color, allowReplace: false);

		public Palette AddHue(Color color) => AddHue(500, color); // Default to hue 500

		// Public ReplaceHue method
		public Palette ReplaceHue(int hue, Color color) => AddOrReplaceHue(hue, color, allowReplace: true);

		public Palette ReplaceHue(Color color) => ReplaceHue(500, color); // Default to hue 500

		// Overload for RGB values
		public Palette AddHue(int hue, int red, int green, int blue) =>
			AddHue(hue, new DeviceRgb(red, green, blue));

		public Palette AddHue(int red, int green, int blue) =>
			AddHue(500, new DeviceRgb(red, green, blue)); // Default to hue 500

		public Palette ReplaceHue(int hue, int red, int green, int blue) =>
			ReplaceHue(hue, new DeviceRgb(red, green, blue));

		public Palette ReplaceHue(int red, int green, int blue) =>
			ReplaceHue(500, new DeviceRgb(red, green, blue)); // Default to hue 500

		// Overload for System.Drawing.Color
		public Palette AddHue(int hue, System.Drawing.Color systemColor) =>
			AddHue(hue, new DeviceRgb(systemColor.R, systemColor.G, systemColor.B));

		public Palette AddHue(System.Drawing.Color systemColor) =>
			AddHue(500, new DeviceRgb(systemColor.R, systemColor.G, systemColor.B)); // Default to hue 500

		public Palette ReplaceHue(int hue, System.Drawing.Color systemColor) =>
			ReplaceHue(hue, new DeviceRgb(systemColor.R, systemColor.G, systemColor.B));

		public Palette ReplaceHue(System.Drawing.Color systemColor) =>
			ReplaceHue(500, new DeviceRgb(systemColor.R, systemColor.G, systemColor.B)); // Default to hue 500

		// Overload for hex color
		public Palette AddHue(int hue, string hexColor) =>
			AddHue(hue, ParseHexColor(hexColor));

		public Palette AddHue(string hexColor) =>
			AddHue(500, ParseHexColor(hexColor)); // Default to hue 500

		public Palette ReplaceHue(int hue, string hexColor) =>
			ReplaceHue(hue, ParseHexColor(hexColor));

		public Palette ReplaceHue(string hexColor) =>
			ReplaceHue(500, ParseHexColor(hexColor)); // Default to hue 500

		// Helper method for hex color parsing
		private static Color ParseHexColor(string hexColor)
		{
			if (string.IsNullOrWhiteSpace(hexColor) || !hexColor.StartsWith("#"))
			{
				throw new ArgumentException("Hex color must be in the format '#RRGGBB'.");
			}

			if (hexColor.Length != 7)
			{
				throw new ArgumentException("Hex color must be in the format '#RRGGBB'.");
			}

			var r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
			var g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
			var b = Convert.ToInt32(hexColor.Substring(5, 2), 16);

			return new DeviceRgb(r, g, b);
		}
	}

	#region  predefined palettes
	public static class PredefinedPalettes
	{
		public static Palette Blue => new Palette { Name = "Blue" }
			.AddHue(50, "#E3F2FD")
			.AddHue(100, "#BBDEFB")
			.AddHue(200, "#90CAF9")
			.AddHue(300, "#64B5F6")
			.AddHue(400, "#42A5F5")
			.AddHue(500, "#2196F3") // Primary hue
			.AddHue(600, "#1E88E5")
			.AddHue(700, "#1976D2")
			.AddHue(800, "#1565C0")
			.AddHue(900, "#0D47A1")
			.AddHue(1000, "#896b42");

		public static Palette Red => new Palette { Name = "Red" }
			.AddHue(50, "#FFEBEE")
			.AddHue(100, "#FFCDD2")
			.AddHue(200, "#EF9A9A")
			.AddHue(300, "#E57373")
			.AddHue(400, "#EF5350")
			.AddHue(500, "#F44336") // Primary hue
			.AddHue(600, "#E53935")
			.AddHue(700, "#D32F2F")
			.AddHue(800, "#C62828")
			.AddHue(900, "#B71C1C");

		public static Palette Green => new Palette { Name = "Green" }
		.AddHue(50, "#E8F5E9")
		.AddHue(100, "#C8E6C9")
		.AddHue(200, "#A5D6A7")
		.AddHue(300, "#81C784")
		.AddHue(400, "#66BB6A")
		.AddHue(500, "#4CAF50") // Primary hue
		.AddHue(600, "#43A047")
		.AddHue(700, "#388E3C")
		.AddHue(800, "#2E7D32")
		.AddHue(900, "#1B5E20")
		.AddHue(1000, "#000000");

		public static Palette Pink => new Palette { Name = "Pink" }
			.AddHue(50, "#FCE4EC")
			.AddHue(100, "#F8BBD0")
			.AddHue(200, "#F48FB1")
			.AddHue(300, "#F06292")
			.AddHue(400, "#EC407A")
			.AddHue(500, "#E91E63") // Primary hue
			.AddHue(600, "#D81B60")
			.AddHue(700, "#C2185B")
			.AddHue(800, "#AD1457")
			.AddHue(900, "#880E4F");


		public static Palette Amber => new Palette { Name = "Amber" }
			.AddHue(50, "#FFF8E1")
			.AddHue(100, "#FFECB3")
			.AddHue(200, "#FFE082")
			.AddHue(300, "#FFD54F")
			.AddHue(400, "#FFCA28")
			.AddHue(500, "#FFC107") // Primary hue
			.AddHue(600, "#FFB300")
			.AddHue(700, "#FFA000")
			.AddHue(800, "#FF8F00")
			.AddHue(900, "#FF6F00");

		public static Palette Purple => new Palette { Name = "Purple" }
			.AddHue(50, "#F3E5F5")
			.AddHue(100, "#E1BEE7")
			.AddHue(200, "#CE93D8")
			.AddHue(300, "#BA68C8")
			.AddHue(400, "#AB47BC")
			.AddHue(500, "#9C27B0") // Primary hue
			.AddHue(600, "#8E24AA")
			.AddHue(700, "#7B1FA2")
			.AddHue(800, "#6A1B9A")
			.AddHue(900, "#4A148C");
			
	}
	#endregion
}