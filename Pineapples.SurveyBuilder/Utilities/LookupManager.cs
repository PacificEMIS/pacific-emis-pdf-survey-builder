using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System.Runtime.CompilerServices;
using surveybuilder;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Forms.Xfdf;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using Org.BouncyCastle.Crypto.Engines;
using iText.Forms.Fields.Properties;
using System.Linq.Expressions;

namespace surveybuilder
{
	/// <summary>
	/// Defines the string characters providing the standard checkbox characters
	/// In dingbats font standard encoding. 
	/// see alse cbstring which translates from itext.CheckBoxType enum
	/// </summary>
	public static class PdfDingbat
	{
		public static string Tick = "4";
		public static string Square = "n";    // Square in Zapf Dingbats
		public static string Diamond = "u";   // Diamond in Zapf Dingbats
		public static string Circle = "l";    // Circle in Zapf Dingbats
		public static string Cross = "7";
	}
	public class LookupEntry
	{
		public LookupEntry() { }
		public LookupEntry(string C, string N) 
		{ 
			this.C = C; 
			this.N = N;
		}

		/// <summary>
		/// The primary code for the entry (e.g., "C" or equivalent key).
		/// </summary>
		public string C { get; set; }

		/// <summary>
		/// The primary name for the entry (e.g., "N" or equivalent key).
		/// </summary>
		public string N { get; set; }

		/// <summary>
		/// Additional metadata associated with this entry.
		/// </summary>
		public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

		public override string ToString()
		{
			string m = string.Join(",", Metadata.Keys.ToArray());
			return $"{C}: {N} ({m})";
		}
	}

	public class LookupList : List<LookupEntry>
	{

		public LookupList() : base() { }

		public LookupList(IEnumerable<LookupEntry> entries)
		{
			AddRange(entries); // Adds all entries to this LookupList
		}

		// Add a helper method to find an entry by key
		public LookupEntry FindByCode(string code)
		{
			return this.FirstOrDefault(entry => entry.C == code);
		}

		// Add a method to print all entries
		public void PrintAll()
		{
			foreach (var entry in this)
			{
				Console.WriteLine(entry);
			}
		}
		/// <summary>
		/// Filters a list of LookupEntry objects based on a set of criteria.
		/// </summary>
		/// <param name="entries">The list of LookupEntry objects to filter.</param>
		/// <param name="criteria">A dictionary of key-value pairs representing the filter criteria.</param>
		/// <returns>A filtered list of LookupEntry objects matching all criteria.</returns>
		public LookupList FilterByMetadata(Dictionary<string, object> criteria)
		{
			return this
				.Where(entry =>
					criteria.All(criterion =>
						entry.Metadata.ContainsKey(criterion.Key) &&
						entry.Metadata[criterion.Key]?.ToString() == criterion.Value?.ToString()))
				.ToLookupList();
		}
		// simpler case - 1 criterion
		public LookupList FilterByMetadata(string key, object value)
		{
			Dictionary<string, object> criteria = new Dictionary<string, object>()
			{
				{ key, value }
			};
			return FilterByMetadata(criteria);
		}

		/// <summary>
		/// Many grid shaped objects use lookup lists to generate the row and column
		/// headings of a grid. These values are associated to the row or column number.
		/// When the form is processed into the database this metadata must be available to the 
		/// upload procedure, inorder to know what codes are associated to each data item.
		/// To accomplish this the values are stored as fields.
		/// Common names look like:
		/// Tag.R.nn.K Tag.R.nn.V Tag.C.nn.K Tag.C.nn.V
		/// Tag - the particular table or data context
		/// R/C row or column
		/// nn index to the row or column
		/// K/V key/value ( ie from Code/ Name of the lookup
		/// 
		/// Note that it is critically important that List<T> in C# guarantees 
		/// that elements are returned in the order they were added during iteration.
		/// In particular they are always iiterated the same, so the rder they appear when populating rows
		/// is the same as the order they appear in this routine.
		/// </summary>
		/// <param name="pdfDoc">PdfDocumemt in which the fields will be created</param>
		/// <param name="codeFormatter">function converting the row/column number to a string
		/// which is the name of the field to hold the lookup code
		/// typically this will use string interpolation: e.g.
		/// (i) => $"Enrol.R.{i:00}.C"
		/// </param>
		/// <param name="nameFormatter">Same as codeFormatter but for the value (loolup name)
		/// </param>
		public void AsFields(PdfDocument pdfDoc
			, Func<int, string> codeFormatter, Func<int, string> nameFormatter)
		{
			var form = PdfAcroForm.GetAcroForm(pdfDoc, true);

			int i = 0;
			foreach (var lkp in this)
			{
				string codeFldName = codeFormatter(i);
				string nameFldName = nameFormatter(i);

				var codefld = new TextFormFieldBuilder(pdfDoc, codeFldName)
					.CreateText()
					.SetReadOnly(true)
					.SetValue(lkp.C);
				var namefld = new TextFormFieldBuilder(pdfDoc, nameFldName)
					.CreateText()
					.SetReadOnly(true)
					.SetValue(lkp.N);
				form.AddField(codefld);
				form.AddField(namefld);
				i++;
			}
		}
		/// <summary>
		/// Export the codes of a lookup set as fields. This is used in preference to AsFields if the values are 
		/// already used on the form,
		/// </summary>
		/// <param name="pdfDoc">PdfDocument to add fields to</param>
		/// <param name="codeFormatter">function to get field name from array position of lookup in List</param>
		public void CodesAsFields(PdfDocument pdfDoc
			, Func<int, string> codeFormatter)
		{
			var form = PdfAcroForm.GetAcroForm(pdfDoc, true);
			int i = 0;
			foreach (var lkp in this)
			{
				string codeFldName = codeFormatter(i);

				var codefld = new TextFormFieldBuilder(pdfDoc, codeFldName)
					.CreateText()
					.SetReadOnly(true)
					.SetValue(lkp.C);
				form.AddField(codefld);
				i++;
			}
		}
	}
	public class LookupManager:Dictionary<string, LookupList>
	{
		PdfDocument pdfDoc;
		string dataHost;
		
		private Dictionary<string, PdfArray> opts;
		private Dictionary<(string, string, float, float, float, string), PdfDictionary> aps;

		/// <summary>
		/// Constructor for the LookupManager
		/// This class allows retrieval of lookups through in 'indexer' (lookups["schoolTypes"])
		/// As well as efficient management of Option lists for dropdowns derived from lookupList
		/// </summary>
		/// <param name="pdfDoc">The host PdfDocument</param>
		/// <param name="dataHost">url to the Pacific Emis REST implementation</param>
		public LookupManager(PdfDocument pdfDoc, string dataHost)
		{
			this.pdfDoc = pdfDoc;
			this.dataHost = dataHost;
			AddLookups("core");
		}

		#region Lookup Lists
		/// <summary>
		/// Adds a set of loookup tables to the manager, using a Rest call
		/// </summary>
		/// <param name="lookupCollection">Corresponds to the collection endpoint in api/lookups/collection</param>
		public void AddLookups(string lookupCollection)
		{
			string endpoint = $"{dataHost}/api/lookups/collection/{lookupCollection}";

			Console.WriteLine($"Reading lookups from {endpoint}");
			try
			{
				RestApi api = new RestApi();
				var tmp = api.GetFromRestService(endpoint);
				foreach (var kvp in tmp)
				{
					if (!this.ContainsKey(kvp.Key))
					{
						base.Add(kvp.Key, kvp.Value);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error retrieving lookups {lookupCollection}: {ex.Message}");
				throw ex;

			}
		}
		public new void Add(string listName, LookupList lookupList)
		{
			Console.WriteLine($"Adding lookup list '{listName}'");
			try
			{
				base.Add(listName, lookupList);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error Adding list '{lookupList}': {ex.Message}");

			}
		}

		public void PrintAll()
		{
			foreach (string key in this.Keys)
			{
				Console.WriteLine(key);
			}
		}
		#endregion

		#region Opts
		/// <summary>
		/// return a lookup list as a 2-dimensional PdfArray, that may be used as /Opt on a dropdown
		/// </summary>
		/// <param name="key">name of lookup list</param>
		/// <returns>the PdfArray</returns>
		public PdfArray Opt(string key)
		{
			if (opts == null)
			{
				opts = new Dictionary<string, PdfArray>();

			}
			if (opts.ContainsKey(key))
			{
				return opts[key];
			}
			if (!this.ContainsKey(key))
			{
				throw new KeyNotFoundException($"Key '{key}' not found in lookups.");
			}
			return OptFromLookup(key);

		}

		/// <summary>
		/// Create Indirect PdfArray from Lookup list, and cache
		/// </summary>
		/// <param name="key">name of loookup list</param>
		/// <returns>PdfArray</returns>
		private PdfArray OptFromLookup(string key)
		{
			LookupList lkps = this[key];

			PdfArray outerArray = new PdfArray();
			// empty option - without this you cannot clear the dropdown
			PdfArray innerArray = new PdfArray();
			innerArray.Add(new PdfString("00"));
			innerArray.Add(new PdfString(""));
			outerArray.Add(innerArray);

			foreach (var item in lkps)
			{
				// Create a PdfArray for each item (C, N)
				innerArray = new PdfArray();
				innerArray.Add(new PdfString(item.C));
				innerArray.Add(new PdfString(item.N));

				// Add the inner array to the outer array
				outerArray.Add(innerArray);
			}
			outerArray.MakeIndirect(pdfDoc);
			opts[key] = outerArray;
			return outerArray;
		}

		#endregion

		#region Aps
		/// <summary>
		/// Retrieves an Appearance Dictionary for a checkbox. These are cached in the 
		/// dictionary for reuse and are implemented as 'indirect' objects in the output PDF.
		/// Appearance components are produced 'just-in-time' and are created hierarchically
		/// W - a synthetic object that holds an /AP and corresponding /MK
		/// AP - an AP object , has 2 parts /N and /D
		/// N a normal appearance obect /N
		/// D a 'downstate' appearance obect /D
		/// /N has /Off and /<export> keys These contain actual rendering streams stored as 'n' 
		/// /D has /Off and /<export> keys These contain actual rendering streams representing 
		/// the mouse-down state stored as 'd' 
		/// A top-level call to Ap (to create a W object) will return that object if already stored
		/// in the aps dictionary.
		/// If found found it is created by call Ap to get each consituent part. This process 
		/// decends recursively creating and storing, or retrieving, each required component
		/// The components are all created as indirect PdfDictionary objects
		/// So in the output PDF, each value in the aps becomes a numbered object in the PDF, 
		/// that is referenced by all the objects of which it is a component.
		/// This appraoch is flexible, and eliminates any duplication in rendering code for checkboxes.
		/// The /MK dictionary provides support for rendering when the user elects to use the
		/// application-generated field highlighting. It ensures that the symbol in the checkbox 
		/// remains constitent when focused, mouedown, or normal.
		/// </summary>
		/// <param name="typ">the CheckboxType determines the symbol in the checkbox when selected</param>
		/// <param name="forecolor">color of the symbol</param>
		/// <param name="export">export value for the checkbox. The checkbox may be part of a group
		/// tied to the same field; each such checkbox uses a different export value</param>
		/// <returns>A PdfDictionary with 2 properties: /AP the Appearance Directory, /MK the 'markup' node</returns>
		public PdfDictionary Ap(CheckBoxType typ, iText.Kernel.Colors.Color forecolor, string export)
		{
			// if requesting a square, provided BLOCK implementation

			string symbol = (typ == CheckBoxType.SQUARE ? "BLOCK" : CbString(typ));
			return Ap(symbol, forecolor, export);
		}

		public PdfDictionary Ap(string symbol, iText.Kernel.Colors.Color forecolor, string export)
		{
			if (aps == null)
			{
				aps = new Dictionary<(string, string, float, float, float, string), PdfDictionary>();
			}
			return Ap("W", symbol, forecolor, export); // W stands for Widget

		}
		private PdfDictionary Ap(string mode, string symbol, iText.Kernel.Colors.Color forecolor, string export)
		{
			var apkey = (Mode: mode, Symbol: symbol,
				Red: forecolor.GetColorValue()[0],
				Green: forecolor.GetColorValue()[1],
				Blue: forecolor.GetColorValue()[2], Export: export);

			if (aps.TryGetValue(apkey, out var ap))
			{
//				Console.WriteLine($"Found {apkey}");
				return ap;
			}
			// now we have to make it
			// make the empty Normal and Down representations for Off (ie empty)

			PdfFont dingbatFont = PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS);
			PdfDictionary result = new PdfDictionary();
			const string OFF = "Off";

			if (mode == "n")
			{
				// return the drawing part ie will be the value of /Off key, or /Export key
				switch (symbol)
				{
					case "BLOCK":
						// 
						result = PdfForm.CheckBlock(pdfDoc, forecolor).GetPdfObject();
						break;
					case "":
						// normal value for Off
						result = PdfForm.CheckEmpty(pdfDoc).GetPdfObject();
						break;
					default:
						result = PdfForm.CheckSymbol(pdfDoc, dingbatFont, symbol, forecolor).GetPdfObject();
						break;

				}
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				return result;

			}
			if (mode == "d")
			{
				// return the drawing part for Down appearance
				// ie will be the value of /Off key, or /Export key in the /D appearance
				switch (symbol)
				{
					case "BLOCK":
						// 
						result = PdfForm.CheckBlockDown(pdfDoc).GetPdfObject();
						break;
					case "":
						// normal value for Off
						result = PdfForm.CheckEmptyDown(pdfDoc).GetPdfObject();
						break;
					default:
						result = PdfForm.CheckSymbolDown(pdfDoc, dingbatFont, symbol).GetPdfObject();
						break;

				}
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				return result;

			}

			if (mode == "N")
			{
				// get the normal view, with keys for both Off and export

				// block key for export, Normal
				var exportApp = Ap("n", symbol, forecolor, "");     // export not needed
				var OffApp = Ap("n", "", ColorConstants.WHITE, "");		// white is just a placeholder
				result.Put(new PdfName(export), exportApp);
				result.Put(new PdfName(OFF), OffApp);
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				return result;
			}
			if (mode == "D")
			{
				// get the normal view, with keys for both Off and export

				// block key for export, Normal
				var exportApp = Ap("d", symbol, ColorConstants.WHITE, "");     
				var OffApp = Ap("d", "", ColorConstants.WHITE, "");				// export and forecolor not needed
				result.Put(new PdfName(export), exportApp);
				result.Put(new PdfName(OFF), OffApp);
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				return result;
			}
			if (mode == "MK")
			{
				iText.Kernel.Colors.Color borderColor = ColorConstants.GRAY;
				PdfDictionary mk = new PdfDictionary();
				result.Put(PdfName.CA, new PdfString(symbol));
				result.Put(PdfName.BC, new PdfArray(borderColor.GetColorValue()));
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				return result;
			}

			if (mode == "AP")
			{
				
				// get the /N part
				PdfObject NApp = Ap("N", symbol, forecolor, export);
				// coming down this path we really need the actual dingbat character to put in MK
				PdfObject DApp = Ap("D", symbol, ColorConstants.WHITE, export); 
				result.Put(PdfName.N, NApp);
				result.Put(PdfName.D, DApp);
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				
				return result;
			}
			if (mode == "W")  // w for Widget
			{
				// this top level call returns a synthetic object including both the AP and MK 
				// dictionaries of a check widget. They are separated and put in the right places
				// the the Renderer
				PdfObject ApDic = Ap("AP", symbol, forecolor, export);
				PdfObject MkDic = Ap("MK", (symbol == "BLOCK" ? CbString(CheckBoxType.SQUARE) : symbol), forecolor, "");
				result.Put(PdfName.AP, ApDic);
				result.Put(PdfName.MK, MkDic);
				result.MakeIndirect(pdfDoc);
				aps.Add(apkey, result);
				//System.Console.WriteLine($"{aps.Count}");
				return result;

			}
			return null;
		}


		// returnthe dingbat character for each checkbox type
		private string CbString(CheckBoxType type)
		{
			switch (type)
			{
				case CheckBoxType.CHECK:
					return "4";
				case CheckBoxType.SQUARE:
					return "n";
				case CheckBoxType.CIRCLE:
					return "l";
				case CheckBoxType.DIAMOND:
					return "u";
				case CheckBoxType.STAR:
					return "H";
				case CheckBoxType.CROSS:
					return "6";
				default:
					return "n";
			}
		}
		#endregion


	}

	public static class LookupExtensions
	{
		public static LookupList ToLookupList(this IEnumerable<LookupEntry> entries)
		{
			return new LookupList(entries);
		}
	}

}
