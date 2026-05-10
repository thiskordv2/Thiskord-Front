using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.GestionProjet;

namespace Thiskord_Front.Services
{
    public class TaskService
    {
        private readonly ApiService _apiService = new();

        public async Task<List<SprintTask>> GetTasksBySprint(int sprintId)
        {
            string jsonResult = await _apiService.CallApiAsync("api/sprinttask/sprint/task/{sprintId}", "GET");
            if (string.IsNullOrEmpty(jsonResult))
                return new List<SprintTask>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                return JsonSerializer.Deserialize<List<SprintTask>>(jsonResult, options) ?? new List<SprintTask>();
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur Désérialisation tasks: " + ex.Message);
                return new List<SprintTask>();
            }
        }

        public async Task<List<SprintTask>> GetTasksByParentTask(int parentTaskId)
        {
            string jsonResult = await _apiService.CallApiAsync($"sprinttask/parent/task/{parentTaskId}", "GET");
            if (string.IsNullOrEmpty(jsonResult))
                return new List<SprintTask>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                return JsonSerializer.Deserialize<List<SprintTask>>(jsonResult, options) ?? new List<SprintTask>();
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur Désérialisation parent tasks: " + ex.Message);
                return new List<SprintTask>();
            }
        }

        public async Task<bool> CreateTask(SprintTask task)
        {
            if (task == null)
                return false;

            string jsonRequest = JsonSerializer.Serialize(task);
            string jsonResult = await _apiService.CallApiAsync("api/task/task", "POST", jsonRequest);
            return !string.IsNullOrEmpty(jsonResult);
        }

        public async Task<bool> UpdateTask(SprintTask task)
        {
            if (task == null || task.task_id == 0)
                return false;

            string jsonRequest = JsonSerializer.Serialize(task);
            string jsonResult = await _apiService.CallApiAsync("api/task/task", "PATCH", jsonRequest);
            return !string.IsNullOrEmpty(jsonResult);
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            if (taskId <= 0)
                return false;

            string jsonResult = await _apiService.CallApiAsync("api/task/task/{taskId}", "DELETE");
            return !string.IsNullOrEmpty(jsonResult);
        }
    }
}
