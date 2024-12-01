﻿using System;
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


	public class RestApi
	{
		public Dictionary<string, LookupList> GetFromRestService(string url)
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
					response.EnsureSuccessStatusCode();

					// Read the response content as a string
					string responseBody = response.Content.ReadAsStringAsync().Result;

					// Parse the JSON response dynamically
					var tmp = JsonConvert.DeserializeObject<JObject>(responseBody);

					// Dictionary to store the parsed results with metadata
					var dic = new Dictionary<string, LookupList>();

					// Iterate through each property in the JSON object
					foreach (var property in tmp.Properties())
					{
						string key = property.Name; // e.g., "schoolTypes", "authorities"
						JArray items = (JArray)property.Value;

						var entryList = new LookupList();

						foreach (JObject item in items)
						{
							// Extract the main "Code" and "Name" values
							string code = item.ContainsKey("C") ? item["C"]?.ToString() :
										  item.ContainsKey("QualCode") ? item["QualCode"]?.ToString() :
										  item.ContainsKey("mresName") ? item["mresName"]?.ToString() :
										  item.ContainsKey("ToiletType") ? item["ToiletType"]?.ToString() : null;

							string name = item.ContainsKey("N") ? item["N"]?.ToString() :
										  item.ContainsKey("QualName") ? item["QualName"]?.ToString() :
										  item.ContainsKey("mresName") ? item["mresName"]?.ToString() :
										  item.ContainsKey("ToiletType") ? item["ToiletType"]?.ToString() : null;

							// Create a new LookupEntry
							var entry = new LookupEntry
							{
								C = code,
								N = name
							};

							// Extract additional metadata
							foreach (var additionalField in item.Properties())
							{
								string fieldName = additionalField.Name;

								// Skip the primary "Code" and "Name" keys
								if (fieldName == "C" || fieldName == "N" ||
									fieldName == "QualCode" || fieldName == "QualName" ||
									fieldName == "mresName" || fieldName == "ToiletType")
								{
									continue;
								}

								// Add other fields to Metadata
								entry.Metadata[fieldName] = additionalField.Value?.ToObject<object>();
							}

							// Add the entry to the list
							entryList.Add(entry);
						}

						// Add the processed list to the dictionary
						dic[key] = entryList;
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
