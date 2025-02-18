﻿using System;
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

using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using iText.Forms.Form.Element;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Colors;
using iText.Forms.Xfdf;
using iText.Forms.Fields.Merging;
using iText.Forms.Form;
using iText.Layout.Borders;
using System.Security.Cryptography;
using CommandLine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using System.Dynamic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using surveybuilder.Utilities;


namespace surveybuilder
{
	class Program
	{


		static void Main(string[] args)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			// Retrieve the attributes
			string title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)))?.Title ?? "No Title Defined";
			string description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)))?.Description ?? "No Description Defined";
			string version = assembly.GetName().Version?.ToString().Substring(0,5) ?? "No Version Defined";
			string copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)))?.Copyright ?? "No Copyright Defined";

			// Write them to the console
			Console.WriteLine($"{title} Version {version}");
			Console.WriteLine($"{description}");
			Console.WriteLine($"{copyright}");
			Console.WriteLine(new String('-',80));
			Console.WriteLine();



			// Find all types implementing IBuilder
			var builderTypes = assembly.GetTypes()
									   .Where(t => typeof(IBuilder).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

			// Create the dictionary
			Dictionary<string, string> builderDescriptions = new Dictionary<string, string>();

			foreach (var type in builderTypes)
			{
				// Create an instance of the class
				if (Activator.CreateInstance(type) is IBuilder instance)
				{
					builderDescriptions[type.Name] = instance.Description;
				}
			}

			// Print the dictionary
			Console.WriteLine($"Available forms:");
			foreach (var kvp in builderDescriptions)
			{
				Console.WriteLine($"{kvp.Key}: {kvp.Value}");
			}
			Console.WriteLine();

			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(RunWithOptions)
				.WithNotParsed(HandleParseError);
		}

		static void HandleParseError(IEnumerable<Error> errs)
		{
			Console.WriteLine("Error parsing arguments.Press <ENTER> to exit");
			Console.ReadLine();

		}

		static void RunWithOptions(Options opts)
		{
			opts.TestOptions();
			string dest = opts.Destination;
			opts.ToConsole();
			// deal with the toolbox
			if (opts.Toolbox != null)
			{
				new Toolbox(opts).RunTools();
				return;
			}
			if (opts.Script != null)
			{
				new Toolbox(opts).Script();
				return;
			}


			try
			{
				string retry = "";
				PdfDocument pdfDoc = null;
				do
				{

					try
					{
						retry = "";
						WriterProperties wprops = new WriterProperties()
							.SetCompressionLevel(opts.Verbose ? CompressionConstants.NO_COMPRESSION : CompressionConstants.BEST_COMPRESSION)
							.SetFullCompressionMode(!opts.Verbose);


						PdfWriter writer = new PdfWriter(dest, wprops);
						pdfDoc = new PdfDocument(writer);
					}
					catch (Exception e)
					{
						Console.WriteLine($"Unable to open {dest} for writing. Press Y to retry...");
						retry = Console.ReadLine();
						if (retry.ToLower() != "y")
						{
							throw e;
						}
					}
					
				} while (retry.ToLower() == "y");
				

				// now use form to create the class
				// Create an instance of the class
				IBuilder builder = CreateBuilderInstance(opts.Form);
				builder.Initialise(opts, pdfDoc);

				Document document = builder.Build();
				document.Close();
				Console.WriteLine($"Build complete: {dest} created");

				if (!String.IsNullOrEmpty(opts.Populate)) {
					Console.WriteLine($"Populating from school {opts.Populate} year {opts.Year}");
					dest = new Toolbox(opts).Populate();
				}

				if (opts.Xfdf)
				{
					 new Toolbox(opts).Xfdf(dest);
				}
				Console.WriteLine($"End of processing");
				if (opts.AutoOpen)
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = dest,
						UseShellExecute = true // Use the OS shell to open the file with its associated app
					});
				}
				else if (opts.Wait)
				{
					Console.ReadKey();
				}

			}
			catch (Exception e)
			{
				Console.WriteLine($"Error: {e.Message}");
				Console.WriteLine();
				Console.WriteLine(e);
				Console.ReadKey();
			}
		}

		static IBuilder CreateBuilderInstance(string className)
		{
			// Get the current assembly
			var assembly = typeof(Program).Assembly;

			string namespacename = typeof(Program).Namespace;
			// Get the full type name
			var type = assembly.GetType($"{namespacename}.{className}", true, true);
			//var type = builderTypes.Where(type => (type.Name == "KEMIS_SEC"))
			//	.First();
			// Create an instance of the type
			return (IBuilder)Activator.CreateInstance(type);
		}

	}
}