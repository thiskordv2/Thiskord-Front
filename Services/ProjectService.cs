using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;
using Windows.Services.Maps;

namespace Thiskord_Front.Services
{
    public class ProjectService
    {
        private ApiService apiService = new();

        public async Task<List<Project>> GetAllProjects()
        {
            string jsonResult = await apiService.CallApiAsync("project/all", "GET");
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    var projects = JsonSerializer.Deserialize<List<Project>>(jsonResult, options) ?? new List<Project>();
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

        public async Task<bool> CreateProject(string name, string description)
        {
            var payload = new { name = name, description = description };
            string jsonRequest = JsonSerializer.Serialize(payload);
            string jsonResult = await apiService.CallApiAsync("project/create", "POST", jsonRequest);
            return !string.IsNullOrEmpty(jsonResult);
        }

        public async Task<bool> EditProject(int projectId, string name, string description)
        {
            var payload = new
            {
                name = name,
                description = description
            };

            string json = JsonSerializer.Serialize(payload);
            string? result = await apiService.CallApiAsync($"project/{projectId}", "PUT", json);
            return result != null;
        }
    }
}
