using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;

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

					// Send a GET request to the specified endpoint.
					HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;

					// Ensure the request was successful.
					response.EnsureSuccessStatusCode();

					// Read the response content as a string.
					string responseBody = response.Content.ReadAsStringAsync().Result;

					// representation of the Json returned by lookups/collection methods
					Dictionary<string, CnObject[]> tmp = JsonConvert.DeserializeObject<Dictionary<string, CnObject[]>>(responseBody);

					var dic = new Dictionary<string, List<KeyValuePair<string, string>>>();

					dic = tmp.ToDictionary(
						kvp => kvp.Key,
						kvp => kvp.Value
								.Select(cn => new KeyValuePair<string, string>(cn.C, cn.N))
								.ToList()
								);

					return dic;
				}
				catch (HttpRequestException e)
				{
					// Handle any HTTP request exceptions.
					Console.WriteLine($"Request error: {e.Message}");
					throw;
				}
				catch (Exception e)
				{
					// Handle any other exceptions.
					Console.WriteLine($"Unexpected error: {e.Message}");
					throw;
				}
			}
		}
	}
}
