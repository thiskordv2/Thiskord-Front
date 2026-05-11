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
            string jsonResult = await _apiService.CallApiAsync($"sprinttask/sprint/task/{sprintId}", "GET");
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

            var payload = new
            {
                task_title = task.task_title,
                task_desc = task.task_desc,
                is_subtask = task.is_subtask,
                task_status = task.task_status,
                id_creator = task.id_creator,
                id_resp = task.id_resp,
                id_project_task = task.id_project_task,
                id_sprint = task.id_sprint
            };

            string jsonRequest = JsonSerializer.Serialize(payload);
            string jsonResult = await _apiService.CallApiAsync("sprinttask/task", "POST", jsonRequest);
            return !string.IsNullOrEmpty(jsonResult);
        }

        public async Task<bool> UpdateTask(SprintTask task)
        {
            if (task == null || task.task_id == 0)
                return false;

            string jsonRequest = JsonSerializer.Serialize(task);
            string jsonResult = await _apiService.CallApiAsync($"sprinttask/task/{task.task_id}", "PATCH", jsonRequest);
            return !string.IsNullOrEmpty(jsonResult);
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            if (taskId <= 0)
                return false;

            string jsonResult = await _apiService.CallApiAsync($"sprinttask/task/{taskId}", "DELETE");
            return !string.IsNullOrEmpty(jsonResult);
        }
    }
}
