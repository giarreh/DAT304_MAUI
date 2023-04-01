﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using MauiApp8.Model2;

namespace MauiApp8.Services.BackgroundServices
{
    // All the code in this file is included in all platforms.



    public class Nightscout
    {
        

        


        public static JsonSerializerOptions JsonSerOpt()
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return _serializerOptions;
        }

        public static HttpClient RestService()
        {
            HttpClient _client;
            _client = new HttpClient();

            return _client;
        }

        public static async Task<List<GlucoseAPI>> GetGlucose(string RestUrl, string StartDate, string EndDate)
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            List<GlucoseAPI> Items;
            Items = new List<GlucoseAPI>();
            string Order = $"api/v1/entries/sgv.json?find[dateString][$gte]={StartDate}&find[dateString][$lte]={EndDate}&count=all";
            Uri uri = new Uri(string.Format(RestUrl + Order));

            try
            {
                using (HttpClient _client = new HttpClient())
                {
                    Console.WriteLine("Getting Response...");
                    HttpResponseMessage response = await _client.GetAsync(uri);
                    if (response.Content.Headers.ContentLength == 0)
                    {
                        Debug.WriteLine("The response content is empty.");
                    }
                    else if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Items = JsonSerializer.Deserialize<List<GlucoseAPI>>(content, _serializerOptions);
                        Console.WriteLine("Finnished request...");
                    }

                    response.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not a success code (2xx)
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return Items;
            }

            return Items;
        }

        // GetInsulin does not remove exersie entries from return.
        public static async Task<List<TreatmentAPI>> GetInsulin(string RestUrl, string StartDate, string EndDate)
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            List<TreatmentAPI> Items;
            Items = new List<TreatmentAPI>();
            string Order = $"api/v1/treatments.json?find[created_at][$gte]={StartDate}&find[created_at][$lte]={EndDate}&count=all";
            Console.WriteLine($"{RestUrl}{Order}");
            Uri uri = new Uri(string.Format(RestUrl + Order));

            try
            {
                using (HttpClient _client = new HttpClient())
                {
                    Console.WriteLine("Getting Response...");
                    HttpResponseMessage response = await _client.GetAsync(uri);

                    if (response.Content.Headers.ContentLength == 0)
                    {
                        Console.WriteLine("The response content is empty.");
                    }
                    else if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("The JSON is being serialized...");
                        string content = await response.Content.ReadAsStringAsync();
                        Items = JsonSerializer.Deserialize<List<TreatmentAPI>>(content, _serializerOptions);
                    }

                    response.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not a success code (2xx)
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return Items;
            }

            return Items;
        }
    }
}

