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

		public Theme()
		{
			Primary = new Palette();
			Accent = new Palette();
			Warn = new Palette();
			Background = new Palette();
		}
	}
}
