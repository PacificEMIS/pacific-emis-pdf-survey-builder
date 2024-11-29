﻿
using iText.Kernel.Colors;

namespace surveybuilder
{

	/// <summary>
	/// This class enumerates all the defined 'web colors' and return an iText-compatible
	/// Color (ie aiText.Kernel.Colors.Color. Specifically each is of type DeviceRgb
	/// This allows compilae time checking when a specific colour is required
	/// For a color as a variable, use
	/// Colors.WebColors.GetRGBColor(color)		where color is the color name
	/// </summary>
	public static class NamedColors
	{

		public static Color Black { get; } = new DeviceRgb(0, 0, 0);
		public static Color White { get; } = new DeviceRgb(255, 255, 255);
		public static Color Red { get; } = new DeviceRgb(255, 0, 0);
		public static Color Green { get; } = new DeviceRgb(0, 128, 0);
		public static Color Blue { get; } = new DeviceRgb(0, 0, 255);
		public static Color Yellow { get; } = new DeviceRgb(255, 255, 0);
		public static Color Cyan { get; } = new DeviceRgb(0, 255, 255);
		public static Color Magenta { get; } = new DeviceRgb(255, 0, 255);
		public static Color Silver { get; } = new DeviceRgb(192, 192, 192);
		public static Color Gray { get; } = new DeviceRgb(128, 128, 128);
		public static Color Maroon { get; } = new DeviceRgb(128, 0, 0);
		public static Color Olive { get; } = new DeviceRgb(128, 128, 0);
		public static Color Purple { get; } = new DeviceRgb(128, 0, 128);
		public static Color Teal { get; } = new DeviceRgb(0, 128, 128);
		public static Color Navy { get; } = new DeviceRgb(0, 0, 128);
		public static Color Lime { get; } = new DeviceRgb(0, 255, 0);
		public static Color Aqua { get; } = new DeviceRgb(0, 255, 255);
		public static Color Fuchsia { get; } = new DeviceRgb(255, 0, 255);
		public static Color Orange { get; } = new DeviceRgb(255, 165, 0);
		public static Color AliceBlue { get; } = new DeviceRgb(240, 248, 255);
		public static Color AntiqueWhite { get; } = new DeviceRgb(250, 235, 215);
		public static Color Aquamarine { get; } = new DeviceRgb(127, 255, 212);
		public static Color Azure { get; } = new DeviceRgb(240, 255, 255);
		public static Color Beige { get; } = new DeviceRgb(245, 245, 220);
		public static Color Bisque { get; } = new DeviceRgb(255, 228, 196);
		public static Color BlanchedAlmond { get; } = new DeviceRgb(255, 235, 205);
		public static Color BlueViolet { get; } = new DeviceRgb(138, 43, 226);
		public static Color Brown { get; } = new DeviceRgb(165, 42, 42);
		public static Color BurlyWood { get; } = new DeviceRgb(222, 184, 135);
		public static Color CadetBlue { get; } = new DeviceRgb(95, 158, 160);
		public static Color Chartreuse { get; } = new DeviceRgb(127, 255, 0);
		public static Color Chocolate { get; } = new DeviceRgb(210, 105, 30);
		public static Color Coral { get; } = new DeviceRgb(255, 127, 80);
		public static Color CornflowerBlue { get; } = new DeviceRgb(100, 149, 237);
		public static Color Cornsilk { get; } = new DeviceRgb(255, 248, 220);
		public static Color Crimson { get; } = new DeviceRgb(220, 20, 60);
		public static Color DarkBlue { get; } = new DeviceRgb(0, 0, 139);
		public static Color DarkCyan { get; } = new DeviceRgb(0, 139, 139);
		public static Color DarkGoldenRod { get; } = new DeviceRgb(184, 134, 11);
		public static Color DarkGray { get; } = new DeviceRgb(169, 169, 169);
		public static Color DarkGreen { get; } = new DeviceRgb(0, 100, 0);
		public static Color DarkKhaki { get; } = new DeviceRgb(189, 183, 107);
		public static Color DarkMagenta { get; } = new DeviceRgb(139, 0, 139);
		public static Color DarkOliveGreen { get; } = new DeviceRgb(85, 107, 47);
		public static Color DarkOrange { get; } = new DeviceRgb(255, 140, 0);
		public static Color DarkOrchid { get; } = new DeviceRgb(153, 50, 204);
		public static Color DarkRed { get; } = new DeviceRgb(139, 0, 0);
		public static Color DarkSalmon { get; } = new DeviceRgb(233, 150, 122);
		public static Color DarkSeaGreen { get; } = new DeviceRgb(143, 188, 143);
		public static Color DarkSlateBlue { get; } = new DeviceRgb(72, 61, 139);
		public static Color DarkSlateGray { get; } = new DeviceRgb(47, 79, 79);
		public static Color DarkTurquoise { get; } = new DeviceRgb(0, 206, 209);
		public static Color DarkViolet { get; } = new DeviceRgb(148, 0, 211);
		public static Color DeepPink { get; } = new DeviceRgb(255, 20, 147);
		public static Color DeepSkyBlue { get; } = new DeviceRgb(0, 191, 255);
		public static Color DimGray { get; } = new DeviceRgb(105, 105, 105);
		public static Color DodgerBlue { get; } = new DeviceRgb(30, 144, 255);
		public static Color FireBrick { get; } = new DeviceRgb(178, 34, 34);
		public static Color FloralWhite { get; } = new DeviceRgb(255, 250, 240);
		public static Color ForestGreen { get; } = new DeviceRgb(34, 139, 34);
		public static Color Gainsboro { get; } = new DeviceRgb(220, 220, 220);
		public static Color GhostWhite { get; } = new DeviceRgb(248, 248, 255);
		public static Color Gold { get; } = new DeviceRgb(255, 215, 0);
		public static Color GoldenRod { get; } = new DeviceRgb(218, 165, 32);
		public static Color GreenYellow { get; } = new DeviceRgb(173, 255, 47);
		public static Color HoneyDew { get; } = new DeviceRgb(240, 255, 240);
		public static Color HotPink { get; } = new DeviceRgb(255, 105, 180);
		public static Color IndianRed { get; } = new DeviceRgb(205, 92, 92);
		public static Color Indigo { get; } = new DeviceRgb(75, 0, 130);
		public static Color Ivory { get; } = new DeviceRgb(255, 255, 240);
		public static Color Khaki { get; } = new DeviceRgb(240, 230, 140);
		public static Color Lavender { get; } = new DeviceRgb(230, 230, 250);
		public static Color LavenderBlush { get; } = new DeviceRgb(255, 240, 245);
		public static Color LawnGreen { get; } = new DeviceRgb(124, 252, 0);
		public static Color LemonChiffon { get; } = new DeviceRgb(255, 250, 205);
		public static Color LightBlue { get; } = new DeviceRgb(173, 216, 230);
		public static Color LightCoral { get; } = new DeviceRgb(240, 128, 128);
		public static Color LightCyan { get; } = new DeviceRgb(224, 255, 255);
		public static Color LightGoldenRodYellow { get; } = new DeviceRgb(250, 250, 210);
		public static Color LightGray { get; } = new DeviceRgb(211, 211, 211);
		public static Color LightGreen { get; } = new DeviceRgb(144, 238, 144);
		public static Color LightPink { get; } = new DeviceRgb(255, 182, 193);
		public static Color LightSalmon { get; } = new DeviceRgb(255, 160, 122);
		public static Color LightSeaGreen { get; } = new DeviceRgb(32, 178, 170);
		public static Color LightSkyBlue { get; } = new DeviceRgb(135, 206, 250);
		public static Color LightSlateGray { get; } = new DeviceRgb(119, 136, 153);
		public static Color LightSteelBlue { get; } = new DeviceRgb(176, 196, 222);
		public static Color LightYellow { get; } = new DeviceRgb(255, 255, 224);
		public static Color LimeGreen { get; } = new DeviceRgb(50, 205, 50);
		public static Color Linen { get; } = new DeviceRgb(250, 240, 230);
		public static Color MediumAquamarine { get; } = new DeviceRgb(102, 205, 170);
		public static Color MediumBlue { get; } = new DeviceRgb(0, 0, 205);
		public static Color MediumOrchid { get; } = new DeviceRgb(186, 85, 211);
		public static Color MediumPurple { get; } = new DeviceRgb(147, 112, 219);
		public static Color MediumSeaGreen { get; } = new DeviceRgb(60, 179, 113);
		public static Color MediumSlateBlue { get; } = new DeviceRgb(123, 104, 238);
		public static Color MediumSpringGreen { get; } = new DeviceRgb(0, 250, 154);
		public static Color MediumTurquoise { get; } = new DeviceRgb(72, 209, 204);
		public static Color MediumVioletRed { get; } = new DeviceRgb(199, 21, 133);
		public static Color MidnightBlue { get; } = new DeviceRgb(25, 25, 112);
		public static Color MintCream { get; } = new DeviceRgb(245, 255, 250);
		public static Color MistyRose { get; } = new DeviceRgb(255, 228, 225);
		public static Color Moccasin { get; } = new DeviceRgb(255, 228, 181);
		public static Color NavajoWhite { get; } = new DeviceRgb(255, 222, 173);
		public static Color OldLace { get; } = new DeviceRgb(253, 245, 230);
		public static Color OliveDrab { get; } = new DeviceRgb(107, 142, 35);
		public static Color OrangeRed { get; } = new DeviceRgb(255, 69, 0);
		public static Color Orchid { get; } = new DeviceRgb(218, 112, 214);
		public static Color PaleGoldenRod { get; } = new DeviceRgb(238, 232, 170);
		public static Color PaleGreen { get; } = new DeviceRgb(152, 251, 152);
		public static Color PaleTurquoise { get; } = new DeviceRgb(175, 238, 238);
		public static Color PaleVioletRed { get; } = new DeviceRgb(219, 112, 147);
		public static Color PapayaWhip { get; } = new DeviceRgb(255, 239, 213);
		public static Color PeachPuff { get; } = new DeviceRgb(255, 218, 185);
		public static Color Peru { get; } = new DeviceRgb(205, 133, 63);
		public static Color Pink { get; } = new DeviceRgb(255, 192, 203);
		public static Color Plum { get; } = new DeviceRgb(221, 160, 221);
		public static Color PowderBlue { get; } = new DeviceRgb(176, 224, 230);
		public static Color RosyBrown { get; } = new DeviceRgb(188, 143, 143);
		public static Color RoyalBlue { get; } = new DeviceRgb(65, 105, 225);
		public static Color SaddleBrown { get; } = new DeviceRgb(139, 69, 19);
		public static Color Salmon { get; } = new DeviceRgb(250, 128, 114);
		public static Color SandyBrown { get; } = new DeviceRgb(244, 164, 96);
		public static Color SeaGreen { get; } = new DeviceRgb(46, 139, 87);
		public static Color SeaShell { get; } = new DeviceRgb(255, 245, 238);
		public static Color Sienna { get; } = new DeviceRgb(160, 82, 45);
		public static Color SkyBlue { get; } = new DeviceRgb(135, 206, 235);
		public static Color SlateBlue { get; } = new DeviceRgb(106, 90, 205);
		public static Color SlateGray { get; } = new DeviceRgb(112, 128, 144);
		public static Color Snow { get; } = new DeviceRgb(255, 250, 250);
		public static Color SpringGreen { get; } = new DeviceRgb(0, 255, 127);
		public static Color SteelBlue { get; } = new DeviceRgb(70, 130, 180);
		public static Color Tan { get; } = new DeviceRgb(210, 180, 140);
		public static Color Thistle { get; } = new DeviceRgb(216, 191, 216);
		public static Color Tomato { get; } = new DeviceRgb(255, 99, 71);
		public static Color Turquoise { get; } = new DeviceRgb(64, 224, 208);
		public static Color Violet { get; } = new DeviceRgb(238, 130, 238);
		public static Color Wheat { get; } = new DeviceRgb(245, 222, 179);
		public static Color WhiteSmoke { get; } = new DeviceRgb(245, 245, 245);
		public static Color YellowGreen { get; } = new DeviceRgb(154, 205, 50);
	}

	/// <summary>
	/// This class enumerates all the defined 'web colors' and return a System.Drawing.Color
	/// NamedColors is more useful in that it returns an iText color
	/// </summary>
	public static class WebColors
	{

		public static System.Drawing.Color Black { get; } = System.Drawing.Color.FromArgb(0, 0, 0);
		public static System.Drawing.Color White { get; } = System.Drawing.Color.FromArgb(255, 255, 255);
		public static System.Drawing.Color Red { get; } = System.Drawing.Color.FromArgb(255, 0, 0);
		public static System.Drawing.Color Green { get; } = System.Drawing.Color.FromArgb(0, 128, 0);
		public static System.Drawing.Color Blue { get; } = System.Drawing.Color.FromArgb(0, 0, 255);
		public static System.Drawing.Color Yellow { get; } = System.Drawing.Color.FromArgb(255, 255, 0);
		public static System.Drawing.Color Cyan { get; } = System.Drawing.Color.FromArgb(0, 255, 255);
		public static System.Drawing.Color Magenta { get; } = System.Drawing.Color.FromArgb(255, 0, 255);
		public static System.Drawing.Color Silver { get; } = System.Drawing.Color.FromArgb(192, 192, 192);
		public static System.Drawing.Color Gray { get; } = System.Drawing.Color.FromArgb(128, 128, 128);
		public static System.Drawing.Color Maroon { get; } = System.Drawing.Color.FromArgb(128, 0, 0);
		public static System.Drawing.Color Olive { get; } = System.Drawing.Color.FromArgb(128, 128, 0);
		public static System.Drawing.Color Purple { get; } = System.Drawing.Color.FromArgb(128, 0, 128);
		public static System.Drawing.Color Teal { get; } = System.Drawing.Color.FromArgb(0, 128, 128);
		public static System.Drawing.Color Navy { get; } = System.Drawing.Color.FromArgb(0, 0, 128);
		public static System.Drawing.Color Lime { get; } = System.Drawing.Color.FromArgb(0, 255, 0);
		public static System.Drawing.Color Aqua { get; } = System.Drawing.Color.FromArgb(0, 255, 255);
		public static System.Drawing.Color Fuchsia { get; } = System.Drawing.Color.FromArgb(255, 0, 255);
		public static System.Drawing.Color Orange { get; } = System.Drawing.Color.FromArgb(255, 165, 0);
		public static System.Drawing.Color AliceBlue { get; } = System.Drawing.Color.FromArgb(240, 248, 255);
		public static System.Drawing.Color AntiqueWhite { get; } = System.Drawing.Color.FromArgb(250, 235, 215);
		public static System.Drawing.Color Aquamarine { get; } = System.Drawing.Color.FromArgb(127, 255, 212);
		public static System.Drawing.Color Azure { get; } = System.Drawing.Color.FromArgb(240, 255, 255);
		public static System.Drawing.Color Beige { get; } = System.Drawing.Color.FromArgb(245, 245, 220);
		public static System.Drawing.Color Bisque { get; } = System.Drawing.Color.FromArgb(255, 228, 196);
		public static System.Drawing.Color BlanchedAlmond { get; } = System.Drawing.Color.FromArgb(255, 235, 205);
		public static System.Drawing.Color BlueViolet { get; } = System.Drawing.Color.FromArgb(138, 43, 226);
		public static System.Drawing.Color Brown { get; } = System.Drawing.Color.FromArgb(165, 42, 42);
		public static System.Drawing.Color BurlyWood { get; } = System.Drawing.Color.FromArgb(222, 184, 135);
		public static System.Drawing.Color CadetBlue { get; } = System.Drawing.Color.FromArgb(95, 158, 160);
		public static System.Drawing.Color Chartreuse { get; } = System.Drawing.Color.FromArgb(127, 255, 0);
		public static System.Drawing.Color Chocolate { get; } = System.Drawing.Color.FromArgb(210, 105, 30);
		public static System.Drawing.Color Coral { get; } = System.Drawing.Color.FromArgb(255, 127, 80);
		public static System.Drawing.Color CornflowerBlue { get; } = System.Drawing.Color.FromArgb(100, 149, 237);
		public static System.Drawing.Color Cornsilk { get; } = System.Drawing.Color.FromArgb(255, 248, 220);
		public static System.Drawing.Color Crimson { get; } = System.Drawing.Color.FromArgb(220, 20, 60);
		public static System.Drawing.Color DarkBlue { get; } = System.Drawing.Color.FromArgb(0, 0, 139);
		public static System.Drawing.Color DarkCyan { get; } = System.Drawing.Color.FromArgb(0, 139, 139);
		public static System.Drawing.Color DarkGoldenRod { get; } = System.Drawing.Color.FromArgb(184, 134, 11);
		public static System.Drawing.Color DarkGray { get; } = System.Drawing.Color.FromArgb(169, 169, 169);
		public static System.Drawing.Color DarkGreen { get; } = System.Drawing.Color.FromArgb(0, 100, 0);
		public static System.Drawing.Color DarkKhaki { get; } = System.Drawing.Color.FromArgb(189, 183, 107);
		public static System.Drawing.Color DarkMagenta { get; } = System.Drawing.Color.FromArgb(139, 0, 139);
		public static System.Drawing.Color DarkOliveGreen { get; } = System.Drawing.Color.FromArgb(85, 107, 47);
		public static System.Drawing.Color DarkOrange { get; } = System.Drawing.Color.FromArgb(255, 140, 0);
		public static System.Drawing.Color DarkOrchid { get; } = System.Drawing.Color.FromArgb(153, 50, 204);
		public static System.Drawing.Color DarkRed { get; } = System.Drawing.Color.FromArgb(139, 0, 0);
		public static System.Drawing.Color DarkSalmon { get; } = System.Drawing.Color.FromArgb(233, 150, 122);
		public static System.Drawing.Color DarkSeaGreen { get; } = System.Drawing.Color.FromArgb(143, 188, 143);
		public static System.Drawing.Color DarkSlateBlue { get; } = System.Drawing.Color.FromArgb(72, 61, 139);
		public static System.Drawing.Color DarkSlateGray { get; } = System.Drawing.Color.FromArgb(47, 79, 79);
		public static System.Drawing.Color DarkTurquoise { get; } = System.Drawing.Color.FromArgb(0, 206, 209);
		public static System.Drawing.Color DarkViolet { get; } = System.Drawing.Color.FromArgb(148, 0, 211);
		public static System.Drawing.Color DeepPink { get; } = System.Drawing.Color.FromArgb(255, 20, 147);
		public static System.Drawing.Color DeepSkyBlue { get; } = System.Drawing.Color.FromArgb(0, 191, 255);
		public static System.Drawing.Color DimGray { get; } = System.Drawing.Color.FromArgb(105, 105, 105);
		public static System.Drawing.Color DodgerBlue { get; } = System.Drawing.Color.FromArgb(30, 144, 255);
		public static System.Drawing.Color FireBrick { get; } = System.Drawing.Color.FromArgb(178, 34, 34);
		public static System.Drawing.Color FloralWhite { get; } = System.Drawing.Color.FromArgb(255, 250, 240);
		public static System.Drawing.Color ForestGreen { get; } = System.Drawing.Color.FromArgb(34, 139, 34);
		public static System.Drawing.Color Gainsboro { get; } = System.Drawing.Color.FromArgb(220, 220, 220);
		public static System.Drawing.Color GhostWhite { get; } = System.Drawing.Color.FromArgb(248, 248, 255);
		public static System.Drawing.Color Gold { get; } = System.Drawing.Color.FromArgb(255, 215, 0);
		public static System.Drawing.Color GoldenRod { get; } = System.Drawing.Color.FromArgb(218, 165, 32);
		public static System.Drawing.Color GreenYellow { get; } = System.Drawing.Color.FromArgb(173, 255, 47);
		public static System.Drawing.Color HoneyDew { get; } = System.Drawing.Color.FromArgb(240, 255, 240);
		public static System.Drawing.Color HotPink { get; } = System.Drawing.Color.FromArgb(255, 105, 180);
		public static System.Drawing.Color IndianRed { get; } = System.Drawing.Color.FromArgb(205, 92, 92);
		public static System.Drawing.Color Indigo { get; } = System.Drawing.Color.FromArgb(75, 0, 130);
		public static System.Drawing.Color Ivory { get; } = System.Drawing.Color.FromArgb(255, 255, 240);
		public static System.Drawing.Color Khaki { get; } = System.Drawing.Color.FromArgb(240, 230, 140);
		public static System.Drawing.Color Lavender { get; } = System.Drawing.Color.FromArgb(230, 230, 250);
		public static System.Drawing.Color LavenderBlush { get; } = System.Drawing.Color.FromArgb(255, 240, 245);
		public static System.Drawing.Color LawnGreen { get; } = System.Drawing.Color.FromArgb(124, 252, 0);
		public static System.Drawing.Color LemonChiffon { get; } = System.Drawing.Color.FromArgb(255, 250, 205);
		public static System.Drawing.Color LightBlue { get; } = System.Drawing.Color.FromArgb(173, 216, 230);
		public static System.Drawing.Color LightCoral { get; } = System.Drawing.Color.FromArgb(240, 128, 128);
		public static System.Drawing.Color LightCyan { get; } = System.Drawing.Color.FromArgb(224, 255, 255);
		public static System.Drawing.Color LightGoldenRodYellow { get; } = System.Drawing.Color.FromArgb(250, 250, 210);
		public static System.Drawing.Color LightGray { get; } = System.Drawing.Color.FromArgb(211, 211, 211);
		public static System.Drawing.Color LightGreen { get; } = System.Drawing.Color.FromArgb(144, 238, 144);
		public static System.Drawing.Color LightPink { get; } = System.Drawing.Color.FromArgb(255, 182, 193);
		public static System.Drawing.Color LightSalmon { get; } = System.Drawing.Color.FromArgb(255, 160, 122);
		public static System.Drawing.Color LightSeaGreen { get; } = System.Drawing.Color.FromArgb(32, 178, 170);
		public static System.Drawing.Color LightSkyBlue { get; } = System.Drawing.Color.FromArgb(135, 206, 250);
		public static System.Drawing.Color LightSlateGray { get; } = System.Drawing.Color.FromArgb(119, 136, 153);
		public static System.Drawing.Color LightSteelBlue { get; } = System.Drawing.Color.FromArgb(176, 196, 222);
		public static System.Drawing.Color LightYellow { get; } = System.Drawing.Color.FromArgb(255, 255, 224);
		public static System.Drawing.Color LimeGreen { get; } = System.Drawing.Color.FromArgb(50, 205, 50);
		public static System.Drawing.Color Linen { get; } = System.Drawing.Color.FromArgb(250, 240, 230);
		public static System.Drawing.Color MediumAquamarine { get; } = System.Drawing.Color.FromArgb(102, 205, 170);
		public static System.Drawing.Color MediumBlue { get; } = System.Drawing.Color.FromArgb(0, 0, 205);
		public static System.Drawing.Color MediumOrchid { get; } = System.Drawing.Color.FromArgb(186, 85, 211);
		public static System.Drawing.Color MediumPurple { get; } = System.Drawing.Color.FromArgb(147, 112, 219);
		public static System.Drawing.Color MediumSeaGreen { get; } = System.Drawing.Color.FromArgb(60, 179, 113);
		public static System.Drawing.Color MediumSlateBlue { get; } = System.Drawing.Color.FromArgb(123, 104, 238);
		public static System.Drawing.Color MediumSpringGreen { get; } = System.Drawing.Color.FromArgb(0, 250, 154);
		public static System.Drawing.Color MediumTurquoise { get; } = System.Drawing.Color.FromArgb(72, 209, 204);
		public static System.Drawing.Color MediumVioletRed { get; } = System.Drawing.Color.FromArgb(199, 21, 133);
		public static System.Drawing.Color MidnightBlue { get; } = System.Drawing.Color.FromArgb(25, 25, 112);
		public static System.Drawing.Color MintCream { get; } = System.Drawing.Color.FromArgb(245, 255, 250);
		public static System.Drawing.Color MistyRose { get; } = System.Drawing.Color.FromArgb(255, 228, 225);
		public static System.Drawing.Color Moccasin { get; } = System.Drawing.Color.FromArgb(255, 228, 181);
		public static System.Drawing.Color NavajoWhite { get; } = System.Drawing.Color.FromArgb(255, 222, 173);
		public static System.Drawing.Color OldLace { get; } = System.Drawing.Color.FromArgb(253, 245, 230);
		public static System.Drawing.Color OliveDrab { get; } = System.Drawing.Color.FromArgb(107, 142, 35);
		public static System.Drawing.Color OrangeRed { get; } = System.Drawing.Color.FromArgb(255, 69, 0);
		public static System.Drawing.Color Orchid { get; } = System.Drawing.Color.FromArgb(218, 112, 214);
		public static System.Drawing.Color PaleGoldenRod { get; } = System.Drawing.Color.FromArgb(238, 232, 170);
		public static System.Drawing.Color PaleGreen { get; } = System.Drawing.Color.FromArgb(152, 251, 152);
		public static System.Drawing.Color PaleTurquoise { get; } = System.Drawing.Color.FromArgb(175, 238, 238);
		public static System.Drawing.Color PaleVioletRed { get; } = System.Drawing.Color.FromArgb(219, 112, 147);
		public static System.Drawing.Color PapayaWhip { get; } = System.Drawing.Color.FromArgb(255, 239, 213);
		public static System.Drawing.Color PeachPuff { get; } = System.Drawing.Color.FromArgb(255, 218, 185);
		public static System.Drawing.Color Peru { get; } = System.Drawing.Color.FromArgb(205, 133, 63);
		public static System.Drawing.Color Pink { get; } = System.Drawing.Color.FromArgb(255, 192, 203);
		public static System.Drawing.Color Plum { get; } = System.Drawing.Color.FromArgb(221, 160, 221);
		public static System.Drawing.Color PowderBlue { get; } = System.Drawing.Color.FromArgb(176, 224, 230);
		public static System.Drawing.Color RosyBrown { get; } = System.Drawing.Color.FromArgb(188, 143, 143);
		public static System.Drawing.Color RoyalBlue { get; } = System.Drawing.Color.FromArgb(65, 105, 225);
		public static System.Drawing.Color SaddleBrown { get; } = System.Drawing.Color.FromArgb(139, 69, 19);
		public static System.Drawing.Color Salmon { get; } = System.Drawing.Color.FromArgb(250, 128, 114);
		public static System.Drawing.Color SandyBrown { get; } = System.Drawing.Color.FromArgb(244, 164, 96);
		public static System.Drawing.Color SeaGreen { get; } = System.Drawing.Color.FromArgb(46, 139, 87);
		public static System.Drawing.Color SeaShell { get; } = System.Drawing.Color.FromArgb(255, 245, 238);
		public static System.Drawing.Color Sienna { get; } = System.Drawing.Color.FromArgb(160, 82, 45);
		public static System.Drawing.Color SkyBlue { get; } = System.Drawing.Color.FromArgb(135, 206, 235);
		public static System.Drawing.Color SlateBlue { get; } = System.Drawing.Color.FromArgb(106, 90, 205);
		public static System.Drawing.Color SlateGray { get; } = System.Drawing.Color.FromArgb(112, 128, 144);
		public static System.Drawing.Color Snow { get; } = System.Drawing.Color.FromArgb(255, 250, 250);
		public static System.Drawing.Color SpringGreen { get; } = System.Drawing.Color.FromArgb(0, 255, 127);
		public static System.Drawing.Color SteelBlue { get; } = System.Drawing.Color.FromArgb(70, 130, 180);
		public static System.Drawing.Color Tan { get; } = System.Drawing.Color.FromArgb(210, 180, 140);
		public static System.Drawing.Color Thistle { get; } = System.Drawing.Color.FromArgb(216, 191, 216);
		public static System.Drawing.Color Tomato { get; } = System.Drawing.Color.FromArgb(255, 99, 71);
		public static System.Drawing.Color Turquoise { get; } = System.Drawing.Color.FromArgb(64, 224, 208);
		public static System.Drawing.Color Violet { get; } = System.Drawing.Color.FromArgb(238, 130, 238);
		public static System.Drawing.Color Wheat { get; } = System.Drawing.Color.FromArgb(245, 222, 179);
		public static System.Drawing.Color WhiteSmoke { get; } = System.Drawing.Color.FromArgb(245, 245, 245);
		public static System.Drawing.Color YellowGreen { get; } = System.Drawing.Color.FromArgb(154, 205, 50);
	}

}