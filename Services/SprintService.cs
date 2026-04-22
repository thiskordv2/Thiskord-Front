using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.GestionProjet;

namespace Thiskord_Front.Services
{
    public class SprintService
    {
        private ApiService apiService = new();

        public async Task<List<Sprint>> GetSprint(int projectId)
        {
            string jsonResult = await apiService.CallApiAsync("sprint/"+projectId.ToString(), "GET");
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    var sprints = JsonSerializer.Deserialize<List<Sprint>>(jsonResult, options) ?? new List<Sprint>();
                    return sprints;

                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Désérialisation: " + ex.Message);
                    return new List<Sprint>();
                }
            }
            else
            {
                return new List<Sprint>();
            }
        }

        public async Task<bool> CreateSprint(string name, string description)
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
