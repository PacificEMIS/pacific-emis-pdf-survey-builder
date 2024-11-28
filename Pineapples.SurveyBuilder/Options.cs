using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Configuration;

namespace surveybuilder
{
	public class Options

	{
		string emisUrl = ConfigurationManager.AppSettings["emisUrl"];
		string pineapplesPath = ConfigurationManager.AppSettings["pineapplesPath"];
		string filesPath = ConfigurationManager.AppSettings["filesPath"];

		[Option('f', "form", Required = true, HelpText = "Specifies the form to use.")]
		public string Form { get; set; }

		[Option('v', "verbose", Required = false, HelpText = "Generate uncompressed PDF output to allow debugging in a text editor.")]
		public bool Verbose { get; set; }

		[Option('u', "url", Required = false, HelpText = "Url pointing to the Pacific EMIS Rest Api. Used to source data (especially lookup tables) from the target implementation of Pacific EMIS")]
		public string EmisUrl
		{
			get
			{
				return emisUrl;
			}
			set
			{
				ConfigurationManager.AppSettings["emisUrl"] = value;
				emisUrl = value;
			}
		}

		[Option('p', "pineapples", Required = false, HelpText = "Url pointing to the Pacific EMIS Rest Api. Used to source data (especially lookup tables) from the target implementation of Pacific EMIS")]
		public string PineapplesPath
		{
			get
			{
				return pineapplesPath;
			}
			set
			{
				ConfigurationManager.AppSettings["pineapplesPath"] = value;
				pineapplesPath = value;
			}
		}

		[Option('o', "output", Required = false, HelpText = "Url pointing to the Pacific EMIS Rest Api. Used to source data (especially lookup tables) from the target implementation of Pacific EMIS")]
		public string OutputPath
		{
			get
			{
				return filesPath;
			}
			set
			{
				ConfigurationManager.AppSettings["filesPath"] = value;
				OutputPath = value;
			}
		}

	}
}
