using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveybuilder
{
	public class Theme
	{
		public Palette Primary { get; set; }
		public Palette Accent { get; set; }
		public Palette Warn { get; set; }
		public Palette Background { get; set; }

		public Palette Secondary { get; set; }
		public Palette Error { get; set; }
		public Palette Surface { get; set; }


		public Theme()
		{
			Primary = Palette.Theme("Primary");
			Accent = Palette.Theme("Accent");
			Warn = Palette.Theme("Warn");
			Background = Palette.Theme("Background");
		}
	}
}
