using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.GestionProjet;
using Thiskord_Front.Services;

namespace Thiskord_Front.ViewModels
{
    public class SprintNavigationParameter
    {
        public Sprint Sprint { get; set; }
        public int ProjectId { get; set; }
    }

    public partial class SprintPageViewModel : ObservableObject
    {
        private readonly TaskService _taskService = new();
        private readonly SessionService _sessionService = SessionService.Instance;
        private int _projectId;

        [ObservableProperty]
        private Sprint sprint;

        [ObservableProperty]
        private bool isLoadingTasks;

        public ObservableCollection<SprintTask> Tasks { get; } = new();

        public async Task LoadSprintAsync(Sprint sprint, int projectId)
        {
            Sprint = sprint;
            _projectId = projectId;
            await LoadTasksAsync();
        }

        public async Task LoadTasksAsync()
        {
            if (Sprint is null)
                return;

            IsLoadingTasks = true;
            Tasks.Clear();
            var taskList = await _taskService.GetTasksBySprint(Sprint.sprint_id);
            foreach (var task in taskList)
            {
                Tasks.Add(task);
            }
            IsLoadingTasks = false;
        }

        public async Task<bool> ConfirmCreateTaskAsync(string title, string description, string status = "IN_PROGRESS")
        {
            

            var task = new SprintTask
            {
                task_title = title,
                task_desc = description ?? string.Empty,
                is_subtask = false,
                task_status = status,
                id_creator = _sessionService.CurrentUserId ?? 0,
                id_resp = _sessionService.CurrentUserId ?? 0,
                id_project_task = _projectId,
                id_parent_task = null,
                id_sprint = Sprint.sprint_id
            };

            bool success = await _taskService.CreateTask(task);
            if (success)
            {
                await LoadTasksAsync();
            }

            return success;
        }
    }
}
