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
                client.BaseAddress = new Uri("https://localhost:7250/api/");
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

        public async Task<List<Project>> GetAllProjects()
        {
            loaded = false;
            string jsonResult = await CallApiAsync("project/all", "GET");
            
            System.Diagnostics.Debug.WriteLine("API payload: " + (jsonResult ?? "null"));

            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try 
                {
                    var projects = JsonSerializer.Deserialize<List<Project>>(jsonResult, options) ?? new List<Project>();
                    loaded = true;
                    return projects;
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Désérialisation: " + ex.Message);
                    return new List<Project>();
                }
            }
            else
            {
                return new List<Project>();
            }
        }

        public async Task<List<Channel>> GetChannelsByProjectId(int projectId)
        {
            string jsonResult = await CallApiAsync($"channel/project/{projectId}", "GET");
            
            System.Diagnostics.Debug.WriteLine("Channels API payload: " + (jsonResult ?? "null"));

            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try 
                {
                    var channels = JsonSerializer.Deserialize<List<Channel>>(jsonResult, options) ?? new List<Channel>();
                    return channels;
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Désérialisation Channels: " + ex.Message);
                    return new List<Channel>();
                }
            }
            else
            {
                return new List<Channel>();
            }
        }
    }
}