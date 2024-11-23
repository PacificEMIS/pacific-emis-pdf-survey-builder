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

namespace surveybuilder
{
	public class LookupManager
	{
		PdfDocument pdfDoc;
		string dataHost;
		private Dictionary<string, List<LookupEntry>> lookups;
		private Dictionary<string, PdfArray> opts;
		private Dictionary<string, PdfObject> aps;

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
			lookups = new Dictionary<string, List<LookupEntry>>();
			AddLookups("core");
		}

		#region Lookup Lists

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
					if (!lookups.ContainsKey(kvp.Key))
					{
						lookups.Add(kvp.Key, kvp.Value);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error retrieving XML: {ex.Message}");

			}
		}

		
		/// <summary>
		///  Support the syntax lookups["lookupname"] on LookupManager class
		/// </summary>
		/// <param name="key">name of lookuop list</param>
		/// <returns>the lookup list</returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public List<LookupEntry> this[string key]
		{
			get
			{
				if (lookups.TryGetValue(key, out var value))
				{
					return value;
				}
				throw new KeyNotFoundException($"Key '{key}' not found in lookups.");
			}
			set
			{
				lookups[key] = value;
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
			if (!lookups.ContainsKey(key))
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
			List<LookupEntry> lkps = lookups[key];

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


		public PdfObject Ap(string key)
		{
			if (aps == null)
			{
				aps = new Dictionary<string, PdfObject>();

			}
			if (aps.ContainsKey(key))
			{
				return aps[key];
			}
			// to do
			return null;
		}

		#endregion


	}
}