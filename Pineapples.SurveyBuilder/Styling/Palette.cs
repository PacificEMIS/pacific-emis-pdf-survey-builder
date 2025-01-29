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
	public enum PaletteMode
	{
		Scheme,                 // use Primary Secondary, Tertiary Surface and Error
		Theme,                  // the original AngukarJS Material model of theme
		Swatch
	}

	public class Palette
	{
		// static interface 
		public static Palette Scheme(string name)
		{
			return new Palette(PaletteMode.Scheme)
			{
				Name = name
			};
		}
		public static Palette Theme(string name)
		{
			return new Palette(PaletteMode.Theme)
			{
				Name = name
			};
		}

		public static Palette Swatch(string name)
		{
			return new Palette(PaletteMode.Swatch)
			{
				Name = name
			};
		}

		public string Name { get; set; }
		public Dictionary<int, Color> Hues { get; private set; }
		public Color TextColor { get; set; } // Default text color
		public Color SecondaryTextColor { get; set; } // Optional secondary text color

		protected Palette(PaletteMode mode)
		{
			Hues = new Dictionary<int, Color>();
			this.PaletteMode = mode;
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
		public PaletteMode PaletteMode { get; protected set; }

		protected int DefaultHueIndex
		{
			get
			{
				switch (PaletteMode)
				{
					case PaletteMode.Scheme:
						return 40;
					case PaletteMode.Theme:
						return 500;
					case PaletteMode.Swatch:
						return 1;
					default:
						return 500;
				}
			}
		}
		public Color DefaultHue => GetColor(DefaultHueIndex);

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

		public Palette AddHue(Color color) => AddHue(DefaultHueIndex, color); // Default to hue 500

		// Public ReplaceHue method
		public Palette ReplaceHue(int hue, Color color) => AddOrReplaceHue(hue, color, allowReplace: true);

		public Palette ReplaceHue(Color color) => ReplaceHue(DefaultHueIndex, color); // Default to hue 500

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
		public static Palette Blue => Palette.Theme("Blue")
			.AddHue(50, "#E4E8F5")
			.AddHue(100, "#C0C7E2")
			.AddHue(200, "#97A3CD")
			.AddHue(300, "#6E80B8")
			.AddHue(400, "#4C62A3")
			.AddHue(500, "#24336E") // Primary hue
			.AddHue(600, "#202E63")
			.AddHue(700, "#1B2757")
			.AddHue(800, "#161F4B")
			.AddHue(900, "#101535")
			.AddHue(1000, "#0C0F24");

		public static Palette Blue2 => Palette.Theme("Blue2")
			.AddHue(50, "#E4E6F2")
			.AddHue(100, "#C0C4E0")
			.AddHue(200, "#979FCA")
			.AddHue(300, "#6E7AB3")
			.AddHue(400, "#4C5C9E")
			.AddHue(500, "#3A4883") // Primary hue
			.AddHue(600, "#343F75")
			.AddHue(700, "#2D3666")
			.AddHue(800, "#252D57")
			.AddHue(900, "#1A1F3C")
			.AddHue(1000, "#121427");

		public static Palette Yellow => Palette.Theme("Yellow")
			.AddHue(50, "#F5F1E4")
			.AddHue(100, "#E2D9C0")
			.AddHue(200, "#CDBF97")
			.AddHue(300, "#B8A66E")
			.AddHue(400, "#A38C4C")
			.AddHue(500, "#A29026") // Primary hue
			.AddHue(600, "#8E8220")
			.AddHue(700, "#7A6F1B")
			.AddHue(800, "#655B16")
			.AddHue(900, "#4B4310")
			.AddHue(1000, "#322E0C");

		public static Palette Red => Palette.Theme("Red")
			.AddHue(100, "#FFCDD2")
			.AddHue(200, "#EF9A9A")
			.AddHue(300, "#E57373")
			.AddHue(400, "#EF5350")
			.AddHue(500, "#F44336") // Primary hue
			.AddHue(600, "#E53935")
			.AddHue(700, "#D32F2F")
			.AddHue(800, "#C62828")
			.AddHue(900, "#B71C1C");

		public static Palette Orange => Palette.Theme("Orange")
			.AddHue(50, "#F5EDE4")
			.AddHue(100, "#E8D2C0")
			.AddHue(200, "#D6B297")
			.AddHue(300, "#C4916E")
			.AddHue(400, "#B3754C")
			.AddHue(500, "#A26426") // Primary hue
			.AddHue(600, "#8E5820")
			.AddHue(700, "#7A4C1B")
			.AddHue(800, "#654016")
			.AddHue(900, "#4B3010")
			.AddHue(1000, "#32210C");


		public static Palette Green => Palette.Theme("Green")
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

		public static Palette Pink => Palette.Theme("Pink")
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


		public static Palette Amber => Palette.Theme("Amber")
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

		public static Palette Purple => Palette.Theme("Purple")
			.AddHue(50, "#E9E4F5")
			.AddHue(100, "#D1C7E8")
			.AddHue(200, "#B5A3D6")
			.AddHue(300, "#987EC4")
			.AddHue(400, "#7C5AAE")
			.AddHue(500, "#3B236F") // Primary hue
			.AddHue(600, "#352064")
			.AddHue(700, "#2D1B56")
			.AddHue(800, "#251647")
			.AddHue(900, "#190F31")
			.AddHue(1000, "#120A21");




	}
	#endregion
}