using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace surveybuilder
{
	public class CnObject
	{
		public string C { get; set; }
		public string N { get; set; }
	}
	public class RestApi
	{
		public Dictionary<string, List<KeyValuePair<string, string>>> GetFromRestService(string url)
		{
			// Define the Fiddler proxy
			var proxy = new System.Net.WebProxy("http://127.0.0.1:8888", false);

			// Configure HttpClientHandler to use the proxy
			var httpClientHandler = new HttpClientHandler
			{
				Proxy = proxy,
				UseProxy = false
			};
			using (HttpClient client = new HttpClient(httpClientHandler))
			{



				// Ensure the URL is valid and can be reached over HTTPS.
				client.BaseAddress = new Uri(url);

				try
				{
					// Set the Accept header to request XML
					//client.DefaultRequestHeaders.Accept.Clear();
					//client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

					// Send a GET request to the specified endpoint
					HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

					// Ensure the request was successful
					response.EnsureSuccessStatusCode();

					// Read the response content as a string
					string responseBody = response.Content.ReadAsStringAsync().Result;

					// Parse the JSON response dynamically
					var tmp = JsonConvert.DeserializeObject<JObject>(responseBody);

					// Dictionary to store the parsed results
					var dic = new Dictionary<string, List<KeyValuePair<string, string>>>();

					// Iterate through each property in the JSON object
					foreach (var property in tmp.Properties())
					{
						string key = property.Name; // e.g., "schoolTypes", "authorities"
						JArray items = (JArray)property.Value;

						var keyValueList = new List<KeyValuePair<string, string>>();

						foreach (JObject item in items)
						{
							// Try to retrieve "C" and "N" or use fallback keys
							string code = item.ContainsKey("C") ? item["C"]?.ToString() :
										  item.ContainsKey("QualCode") ? item["QualCode"]?.ToString() :
										  item.ContainsKey("mresName") ? item["mresName"]?.ToString() :
										  item.ContainsKey("ToiletType") ? item["ToiletType"]?.ToString() : null;

							string name = item.ContainsKey("N") ? item["N"]?.ToString() :
										  item.ContainsKey("QualName") ? item["QualName"]?.ToString() :
										  item.ContainsKey("mresName") ? item["mresName"]?.ToString() :
										  item.ContainsKey("ToiletType") ? item["ToiletType"]?.ToString() : null;

							// Add to the list if at least one value exists
							if (!string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(name))
							{
								keyValueList.Add(new KeyValuePair<string, string>(code ?? string.Empty, name ?? string.Empty));
							}
						}

						// Add the processed list to the dictionary
						dic[key] = keyValueList;
					}

					return dic;
				}
				catch (HttpRequestException e)
				{
					// Handle any HTTP request exceptions
					Console.WriteLine($"Request error: {e.Message}");
					throw;
				}
				catch (Exception e)
				{
					// Handle any other exceptions
					Console.WriteLine($"Unexpected error: {e.Message}");
					throw;
				}

			}
		}
	}
}
