using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace surveybuilder
{
	public class Options
	{
		[Option('f', "form", Required = true, HelpText = "Specifies the form to use.")]
		public string Form { get; set; }

		[Option('v', "verbose", Required = false, HelpText = "Generate uncompressed PDF output to allow debugging in a text editor.")]
		public bool Verbose { get; set; }
	}
}
