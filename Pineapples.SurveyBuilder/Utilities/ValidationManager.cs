using iText.Kernel.Pdf;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveybuilder
{
	public static class ValidationManager
	{
		static int sequence = 0;
		/// <summary>
		/// Adds a list of required fields to the document
		/// </summary>
		/// <param name="pdfDoc">The document to add the fields to</param>
		/// <param name="fields">The fields to add</param>
		/// <returns>The JavaScript that was added to the document</returns>
		public static string AddRequiredFields(PdfDocument pdfDoc, RequiredFields fields)
		{
			string script =  fields.GenerateJavaScript(pdfDoc, sequence++);
			// write the script to a debug file in the current directory
			// this is useful for debugging the script
			// and for checking that the script is being generated correctly
			// the script can be copied from the file and pasted into the console
			// for testing
			
			//string folder = System.IO.Directory.GetCurrentDirectory();
			//string debugFile = $"{folder}\\{fields.Name}{sequence:00}.js";
			//System.IO.File.WriteAllText(debugFile, script);
			return script;
		}
		/// <summary>
		/// Adds a list of conditional fields to the document
		/// </summary>
		/// <param name="pdfDoc">The document to add the fields to</param>
		/// <param name="fields">The fields to add</param>
		/// <returns>The JavaScript that was added to the document</returns>
		public static string AddConditionalFields(PdfDocument pdfDoc, ConditionalFields fields)
		{
			string script =  fields.GenerateJavaScript(pdfDoc, sequence++);
			string folder = System.IO.Directory.GetCurrentDirectory();
			string debugFile = $"{folder}\\{fields.Name}{sequence:00}.js";
			System.IO.File.WriteAllText(debugFile, script);
			return script;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdfDoc"></param>
		/// <param name="fields"></param>
		/// <returns></returns>
		private static string AddRequiredFields(PdfDocument pdfDoc, List<RequiredFields> fields)
		{
			StringBuilder jsBuilder = new StringBuilder();
			foreach (var field in fields)
			{
				jsBuilder.AppendLine(AddRequiredFields(pdfDoc, field));
			}
			return jsBuilder.ToString();
		}

		private static string AddConditionalFields(PdfDocument pdfDoc, List<ConditionalFields> fields)
		{
			StringBuilder jsBuilder = new StringBuilder();
			foreach (var field in fields)
			{
				jsBuilder.AppendLine(AddConditionalFields(pdfDoc, field));
			}
			return jsBuilder.ToString();
		}

		/// <summary>
		/// Convert an array of strings into an array of string arrays
		/// this is used for preparing arguments of this form from simple strings
		public static string[][] ToArrayArray(params string[] array)
		{
			string[][] result = new string[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				result[i] = new string[] { array[i] };
			}
			return result;
		}
		/// <summary>
		/// Convert an array of strings into a list of string arrays
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static List<string[]> ToArrayList(params string[] array)
		{
			List<string[]> result = new List<string[]>();
			for (int i = 0; i < array.Length; i++)
			{
				result.Add(new string[] { array[i] });
			}
			return result;
		}

		public static string ArrayToJson(string[] array)
		{
			return $"[{string.Join(", ", array.Select(item => $"\"{item}\""))}]";
		}

		public static string ArrayArrayToJson(string[][] arrayArray)
		{
			return $"[{string.Join(", ", arrayArray.Select(array => ArrayToJson(array)))}]";
		}
	}
	/// <summary>
	/// Describes a test of fields that are required (or not) based on the value of another field
	/// </summary>
	public class ConditionalField
	{
		public ConditionalField() { }
		public ConditionalField(string test)
		{
			Test = test;
		}
		/// <summary>
		/// The name of the field on which others are dependent
		/// </summary>
		public string Test;
		/// <summary>
		/// The possible values of Test that trigger the depency
		/// </summary>
		public string[] Value;

		/// <summary>
		/// An list of arrays of the names of dependent fields
		/// </summary>
		public List<string[]> Rq = new List<string[]>();

		/// <summary>
		/// Convenience function to create a test that makes a single field required, based on the value
		/// Y in the Test field. Useful for fields that depend on a Yes/No checkbox pair
		/// </summary>
		/// <param name="test">name of field to test</param>
		/// <param name="rq">name of dependent field</param>
		/// <returns></returns>
		public static ConditionalField IfYes(string test)
		{
			return new ConditionalField(test)
			{
				Value = new string[] { "Y" },
			};
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="test"></param>
		/// <param name="rq">Accept an array of dependent fields</param>
		/// <returns></returns>
		public static ConditionalField IfYes(string test, params string[] rq)
		{
			var cf =  new ConditionalField(test)
			{
				Value = new string[] { "Y" }
			};
			cf.AddAll(rq);
			return cf;
		}
		public static ConditionalField IfYesAlternatives(string test, params string[] rq)
		{
			var cf = new ConditionalField(test)
			{
				Value = new string[] { "Y" }
			};
			cf.AddAlternatives(rq);
			return cf;
		}

		//values is a 0-length array when any non-null value is a match
		public static ConditionalField IfAny(string test, string rq)
		{
			var cf = new ConditionalField(test)
			{
				Value = new string[] { }
			};
			cf.AddAll(rq);
			return cf;
		}
		/// <param name="rq">Accept an array of dependent fields</param>
		public static ConditionalField IfAny(string test, params string[] rq)
		{
			var cf = new ConditionalField(test)
			{
				Value = new string[] { }
			};
			cf.AddAll(rq);
			return cf;
		}

		public static ConditionalField IfAnyAlternatives(string test, params string[] rq)
		{
			var cf = new ConditionalField(test)
			{
				Value = new string[] { }
			};
			cf.AddAlternatives(rq);
			return cf;
		}
		// manipulating the dependent fields
		/// <summary>
		/// Add a single field to the list of dependent fields
		/// </summary>
		/// <param name="rq"></param>
		/// <returns></returns>
		private ConditionalField Add(string rq)
		{
			Rq.Add(new string[] { rq });
			return this;
		}

		/// <summary>
		/// Add a list of fields to the list of dependent fields
		/// Each field is required if the test field matches the Value
		/// </summary>
		/// <param name="rq"></param>
		/// <returns></returns>
		public ConditionalField AddAll(params string[] rq)
		{
			foreach (string s in rq)
			{
				Rq.Add(new string[] { s });
			}
			return this;
		}

		/// <summary>
		/// Add a set of alternative fields to the list of dependent fields
		/// At least one of these values is required if the test field matches the Value
		/// </summary>
		/// <param name="rq"></param>
		/// <returns>the Conditional</returns>
		public ConditionalField AddAlternatives(params string[] rq)
		{
			Rq.Add(rq);
			return this;
		}

	};

	public class ConditionalFields : List<ConditionalField>
	{
		public ConditionalFields(string name, string message)
		{
			Name = name;
			Message = message;
		}
		/// <summary>
		/// Name of this Conditional Fields object exported as element - in array
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Message to display in the event of any error
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Name of a predefined function (in completeness.js)
		/// That takes a string arg - name of the Test field, and returns true or false
		/// The test is ignored if return = false
		/// </summary>
		public string Filter { get; set; }

		public string GenerateJavaScript(PdfDocument pdfDoc, int sequence)
		{
			StringBuilder jsBuilder = new StringBuilder();

			if (string.IsNullOrWhiteSpace(Name))
				throw new InvalidOperationException("Each ConditionFields instance must have a Name.");

			string functionName = $"{Name.CamelCase().Replace(" ", "_")}_Conditionals";
			jsBuilder.AppendLine($"console.println('{functionName} Loading...');");
			jsBuilder.AppendLine($"var {functionName} = function() {{");
			jsBuilder.AppendLine("    var a = [];");
			jsBuilder.AppendLine("    var j = 0;");

			foreach (var condition in this)
			{
				jsBuilder.AppendLine("    a[j++] = {");
				jsBuilder.AppendLine($"        test: \"{condition.Test}\",");
				jsBuilder.AppendLine($"        value: {ValidationManager.ArrayToJson(condition.Value)},");
				jsBuilder.AppendLine($"        rq: {ValidationManager.ArrayArrayToJson(condition.Rq.ToArray())}");
				jsBuilder.AppendLine("    };");
			}
			jsBuilder.AppendLine("    return a;");
			jsBuilder.AppendLine("};");

			string jsObject = $@"{{
		name: '{Name}',
		tests: {functionName},
		message: '{Message}',
		type: 'C',
		filter: {(string.IsNullOrEmpty(Filter) ? "null" : Filter)},
		sort: '{sequence:00}'    
}}";

			jsBuilder.AppendLine($"requiredsTable[requiredsTable.length] = {jsObject}");
			jsBuilder.AppendLine($"console.println('{functionName} Loaded');");
			string jscriptText = jsBuilder.ToString();
			var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);

			PdfDictionary jscript = iText.Kernel.Pdf.Action.PdfAction
					.CreateJavaScript(jscriptText).GetPdfObject();

			// extension doesn;t actually matter here
			// so use .djs to distinguish between dynamic and static js
			// allows toolbox pushjs mode to preserve these
			Console.WriteLine($"Installing dynamic javascript: {functionName}.djs");
			javaScriptNameTree.AddEntry($"{functionName}.djs", jscript);
			return jscriptText;
		}
	}

	public class RequiredFields
	{
		List<string[]> fields = new List<string[]>();
		public RequiredFields(string name, string message)
		{

			Name = name;
			Message = message;
		}
		/// <summary>
		/// Adds single field names to the validation list.
		/// Each name is added as a separate array of one element.
		/// </summary>
		/// <param name="fields">Field names to add.</param>
		/// <returns>The current <see cref="RequiredFields"/> instance, for fluent chaining.</returns>
		public RequiredFields Add(params string[] fields)
		{
			foreach (string fld in fields)
			{
				if (string.IsNullOrWhiteSpace(fld))
				{
					throw new ArgumentException("Field names cannot be null or empty", nameof(fields));
				}
				this.fields.Add(new[] { fld });
			}
			return this;
		}

		/// <summary>
		/// Adds an array of alternative field names as a single entry in the validation list.
		/// </summary>
		/// <param name="alternatives">Alternative field names.</param>
		/// <returns>The current <see cref="RequiredFields"/> instance, for fluent chaining.</returns>
		public RequiredFields AddAlternatives(params string[] alternatives)
		{
			if (alternatives == null || alternatives.Length == 0)
			{
				throw new ArgumentException("Alternatives cannot be null or empty", nameof(alternatives));
			}

			fields.Add(alternatives);
			return this;
		}

		/// <summary>
		/// Retrieves all the fields added to the validation list.
		/// </summary>
		/// <returns>A list of field name arrays.</returns>
		public IReadOnlyList<string[]> GetFields()
		{
			return fields.AsReadOnly();
		}

		public string Name { get; set; }
		public string Message { get; set; }

		/// <summary>
		/// Name of a predefined function (in completeness.js)
		/// That takes a string arg - name of the Test field, and returns true or false
		/// The test is ignored if return = false
		/// </summary>
		public string Filter { get; set; }

		public string GenerateJavaScript(PdfDocument pdfDoc, int sequence)
		{
			StringBuilder jsBuilder = new StringBuilder();

			if (string.IsNullOrWhiteSpace(Name))
				throw new InvalidOperationException("Each RequiredFields instance must have a Name.");

			string functionName = $"{Name.CamelCase().Replace(" ", "_")}_Requireds";
			jsBuilder.AppendLine($"console.println('{functionName} Loading...');");
			jsBuilder.AppendLine($"var {functionName} = function() {{");
			jsBuilder.AppendLine("    var a = [];");
			jsBuilder.AppendLine("    var j = 0;");

			foreach (var nameArray in GetFields())
			{
				jsBuilder.AppendLine($"    a[j++] = {ValidationManager.ArrayToJson(nameArray)};");
			}
			jsBuilder.AppendLine("    return a;");
			jsBuilder.AppendLine("};");

			string jsObject = $@"{{
		name: '{Name}',
		tests: {functionName},
		message: '{Message}',
		type: 'R',
		filter: {(string.IsNullOrEmpty(Filter) ? "null" : Filter)},
		sort: '{sequence:00}'
}}";

			jsBuilder.AppendLine($"requiredsTable[requiredsTable.length] = {jsObject}");
			jsBuilder.AppendLine($"console.println('{functionName} Loaded');");
			string jscriptText = jsBuilder.ToString();
			var javaScriptNameTree = pdfDoc.GetCatalog().GetNameTree(PdfName.JavaScript);

			PdfDictionary jscript = iText.Kernel.Pdf.Action.PdfAction
					.CreateJavaScript(jscriptText).GetPdfObject();

			// extension doesn't actually matter here
			// so use .djs to distinguish between dynamic and static js
			// allows toolbox pushjs mode to preserve these
			Console.WriteLine($"Installing dynamic javascript: {functionName}.djs");
			javaScriptNameTree.AddEntry($"{functionName}.djs", jscript);
			return jscriptText;
		}


	}
}

