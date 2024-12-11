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
		string filesPath = ConfigurationManager.AppSettings["filesPath"];
		int year = 2025;

		string toolbox = null;

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
				filesPath = value;
			}
		}
		[Option('n', "open", Required = false, HelpText = "Url pointing to the Pacific EMIS Rest Api. Used to source data (especially lookup tables) from the target implementation of Pacific EMIS")]
		public bool AutoOpen { get; set; }

		[Option('y', "year", Required = false, HelpText = "Year for the document")]
		public int Year 
		{ 
			get 
			{ 
				return year; 
			} 
			set 
			{
				year = value;
			}
		}


		#region Toolbox options
		/// <summary>
		/// Toolbox mode does not generate the document. It applies a 'tool' to the 
		/// </summary>
		[Option('x', "toolbox", Required = false, HelpText = "Pdf File for tool to act on - no file is exported")]
		public string Toolbox		{
			get
			{
				return toolbox;
			}
			set
			{
				toolbox = value;
			}
		}

		[Option("pushjs", Required = false, HelpText = "Push current Javascript files into the toolbox target")]
		public bool PushJs { get; set; }

		[Option("clearjs", Required = false, HelpText = "Remove all Javascript files from the toolbox target")]
		public bool ClearJs { get; set; }

		[Option("openaction", Required = false, HelpText = "Set OnOpen Javascript on the toolbox target")]
		public string OpenActionJs { get; set; }

		[Option("checkaction", Required = false, HelpText = "Set javascript on 'CheckBtn' on the toolbox target")]
		public string CheckActionJs { get; set; }


		[Option("dump", Required = false, HelpText = "Dump all field names to output window")]
		public bool Dump { get; set; }

		[Option("populate", Required = false, HelpText = "Create a populated survey - value is the school no")]
		public string Populate { get; set; }


		#endregion
	}
}
