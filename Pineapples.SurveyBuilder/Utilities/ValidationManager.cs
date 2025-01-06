using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace surveybuilder
{
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
		/// An array of the names of dependent fields
		/// </summary>
		public string[] Rq;

		/// <summary>
		/// Convenience function to create a test that makes a single field required, based on the value
		/// Y in the Test field. Useful for fields that depend on a Yes/No checkbox pair
		/// </summary>
		/// <param name="test">name of field to test</param>
		/// <param name="rq">name of dependent field</param>
		/// <returns></returns>
		public static ConditionalField IfYes(string test, string rq)
		{
			return new ConditionalField(test)
			{
				Value = new string[] { "Y" },
				Rq = new string[] { rq }
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
			return new ConditionalField(test)
			{
				Value = new string[] { "Y" },
				Rq = rq
			};
		}

		//values is a 0-length array when any non-null value is a match
		public static ConditionalField IfAny(string test, string rq)
		{
			return new ConditionalField(test)
			{
				Value = new string[] { },
				Rq = new string[] { rq }
			};
		}
		/// <param name="rq">Accept an array of dependent fields</param>
		public static ConditionalField IfAny(string test, params string[] rq)
		{
			return new ConditionalField(test)
			{
				Value = new string[] { },
				Rq = rq
			};
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

		public string GenerateJavaScript(PdfDocument pdfDoc)
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
				jsBuilder.AppendLine($"        value: [{string.Join(", ", ArrayToJavaScriptArray(condition.Value))}],");
				jsBuilder.AppendLine($"        rq: [{string.Join(", ", ArrayToJavaScriptArray(condition.Rq))}]");
				jsBuilder.AppendLine("    };");
			}
			jsBuilder.AppendLine("    return a;");
			jsBuilder.AppendLine("};");

			string jsObject = $@"{{
		name: '{Name}',
		tests: {functionName},
		message: '{Message}',
		type: 'C',
		filter: {(string.IsNullOrEmpty(Filter) ? "null" : Filter)}
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


		static IEnumerable<string> ArrayToJavaScriptArray(string[] array)
		{
			foreach (var item in array)
			{
				yield return $"\"{item}\"";
			}
		}
	}

	public class RequiredFields : List<string>
	{
		public RequiredFields(string name, string message)
		{
			Name = name;
			Message = message;
		}
			/// <summary>
			/// Add an array of field names to this validation
			/// </summary>
			/// <param name="fields">names to add</param>
			/// <returns>this, for fluent interface</returns>
			public RequiredFields AddFields(params string[] fields)
			{
				foreach (string fld in fields)
				{
					Add(fld);
				}
				return this;
			}
		
		public string Name { get; set; }
		public string Message { get; set; }

		/// <summary>
		/// Name of a predefined function (in completeness.js)
		/// That takes a string arg - name of the Test field, and returns true or false
		/// The test is ignored if return = false
		/// </summary>
		public string Filter { get; set; }

		public string GenerateJavaScript(PdfDocument pdfDoc)
		{
			StringBuilder jsBuilder = new StringBuilder();

			if (string.IsNullOrWhiteSpace(Name))
				throw new InvalidOperationException("Each RequiredFields instance must have a Name.");

			string functionName = $"{Name.CamelCase().Replace(" ", "_")}_Requireds";
			jsBuilder.AppendLine($"console.println('{functionName} Loading...');");
			jsBuilder.AppendLine($"var {functionName} = function() {{");
			jsBuilder.AppendLine("    var a = [];");
			jsBuilder.AppendLine("    var j = 0;");

			foreach (var name in this)
			{
				jsBuilder.AppendLine($"    a[j++] = {name.SingleQuote()};");
			}
			jsBuilder.AppendLine("    return a;");
			jsBuilder.AppendLine("};");

			string jsObject = $@"{{
		name: '{Name}',
		tests: {functionName},
		message: '{Message}',
		type: 'R',
		filter: {(string.IsNullOrEmpty(Filter) ? "null" : Filter)}
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
}

