using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
                client.BaseAddress = new Uri("https://localhost:7250/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        private async Task<string> CallApiAsync(string route)
        {
            if (!loaded)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(route);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Erreur Api: " + response.StatusCode);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Connection: " + ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<List<Project>> GetAllProjects()
        {
            loaded = false;
            string jsonResult = await CallApiAsync("project/all");
            System.Diagnostics.Debug.WriteLine("API payload: " + (jsonResult ?? "null"));

            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var projects = JsonSerializer.Deserialize<List<Project>>(jsonResult, options) ?? new List<Project>();
                loaded = true;
                return projects;
            }
            else
            {
                loaded = false;
                return new List<Project>();
            }
        }
    }
}