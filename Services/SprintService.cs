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
            var sprints = await FetchSprints("sprint/sprint/project/" + projectId.ToString());
            if (sprints.Count > 0)
                return sprints;

            // fallback si l'URL principale ne renvoie rien
            return await FetchSprints("sprint/project/" + projectId.ToString());
        }

        private async Task<List<Sprint>> FetchSprints(string route)
        {
            string jsonResult = await apiService.CallApiAsync(route, "GET");
            if (string.IsNullOrEmpty(jsonResult))
                return new List<Sprint>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                return JsonSerializer.Deserialize<List<Sprint>>(jsonResult, options) ?? new List<Sprint>();
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur Désérialisation des sprints (" + route + "): " + ex.Message);
                return new List<Sprint>();
            }
        }

        public async Task<bool> CreateSprint(int projectId, string sprintGoal, string sprintBeginDate, string sprintEndDate)
        {
            var payload = new
            {
                sprint_goal = sprintGoal,
                sprint_begin_date = sprintBeginDate,
                sprint_end_date = sprintEndDate,
                id_project_sprint = projectId
            };
            string jsonRequest = JsonSerializer.Serialize(payload);
            string jsonResult = await apiService.CallApiAsync("sprint/create/sprint", "POST", jsonRequest);
            return jsonResult != null;
        }
    }
}
