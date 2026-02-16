using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;

namespace Thiskord_Front.Services
{
    public class ApiService
    {
        private static readonly HttpClient client = new HttpClient();
        
        private bool loaded = false; 

        public ApiService()
        {
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri("http://localhost:8080/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<string> CallApiAsync(string route, string method = "GET", string? jsonRequest = null)
        {
            try
            {
                HttpResponseMessage? response = null;
                switch (method.ToUpper())
                {
                    case "POST":
                        var content = new StringContent(
                            jsonRequest ?? "",
                            Encoding.UTF8,
                            "application/json"
                        );
                        response = await client.PostAsync(route, content);
                        break;
                    case "GET":
                    default:
                        response = await client.GetAsync(route);
                        break;
                }

                if (response != null && response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Api: " + (response?.StatusCode.ToString() ?? "Pas de réponse"));
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur Connection: " + ex.Message);
                return null;
            }
        }
    }
}