using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Thiskord_Front.Models.GestionProjet;
using Thiskord_Front.Models;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;
using Thiskord_Front.Views;

namespace Thiskord_Front.ViewModels
{
    public partial class NavigateurViewModel : ObservableObject
    {
        private readonly SessionService _sessionService = SessionService.Instance;
        private readonly ProjectService _projectService = new();
        private readonly ChannelService _channelService = new();
        private readonly SprintService _sprintService = new();
        private readonly TaskService _taskService = new();
        private readonly UserService _userService = new();

        [ObservableProperty]
        private Project? selectedProject;

        public Visibility SprintSelectorVisibility => SelectedProject is null ? Visibility.Collapsed : Visibility.Visible;

        [ObservableProperty] private string joinMessage = null;
        [ObservableProperty]
        private Channel? selectedChannel;

        [ObservableProperty]
        private Sprint? selectedSprint;

        [ObservableProperty]
        private bool isLoadingProjects;
        public ObservableCollection<Channel> Channels { get; } = new();
        public ObservableCollection<Project> Projects { get; } = new();
        public ObservableCollection<UserAccount> Users { get;  } = new();
        public ObservableCollection<SprintTask> Tasks { get; } = new();

        public ObservableCollection<Sprint> Sprints { get; } = new();

        public event Action? OnLogoutSuccess;
        public event Action<Channel>? RequestEditChannel;
        public event Action<Project>? RequestEditProject;
        public event Action<Sprint>? RequestOpenSprint;
        public event Action? OnJoinProject;

        public event Action? OnProjectCreate;
        public event Action? OnCreateSprint;
        public event Action? OnInviteTokenReceived; 

        [RelayCommand]
        public async Task LoadProjects()
        {
            if (Projects.Any()) return;
            isLoadingProjects = true;
            try
            {
                var projects = await _projectService.GetAllProjects();
                Projects.Clear();
                foreach (var p in projects) Projects.Add(p);
            }
            finally { isLoadingProjects = false; }
        }

        [RelayCommand]
        public async Task SelectProject(Project project)
        {
            if (project == null) return;
            SelectedProject = project;
            SelectedSprint = null;
            Tasks.Clear();

            var channels = await _channelService.GetChannelsForProject(project.id);
            var sprints = await _sprintService.GetSprint(project.id);
            Channels.Clear();
            Sprints.Clear();
            SelectedChannel = null;

            if (channels != null)
            {
                foreach (var c in channels) Channels.Add(c);
            }

            foreach (var s in sprints) Sprints.Add(s);
        }

        partial void OnSelectedProjectChanged(Project? value)
        {
            OnPropertyChanged(nameof(SprintSelectorVisibility));
        }

        [RelayCommand]
        public async Task SelectSprint(Sprint sprint)
        {
            if (sprint == null) return;
            SelectedSprint = sprint;
            RequestOpenSprint?.Invoke(sprint);

            Tasks.Clear();
            var sprintTasks = await _taskService.GetTasksBySprint(sprint.sprint_id);
            foreach (var task in sprintTasks)
                Tasks.Add(task);
        }

        [RelayCommand]
        public async Task DeleteChannel(Channel channel)
        {
            if (channel?.Id == null) return;
            if (await _channelService.DeleteChannel(channel.Id.Value)) Channels.Remove(channel);
        }

        [RelayCommand]
        private async Task Logout()
        {
            _sessionService.Logout();
            OnLogoutSuccess?.Invoke();
        }
        [RelayCommand]
        private void EditChannel(Channel channel){ RequestEditChannel?.Invoke(channel); }

        public async Task<bool> ConfirmEditChannelAsync(Channel channel, string newName, string newDesc)
        {
            if (channel.Id == null) return false;
            bool success = await _channelService.EditChannel(channel.Id.Value, newName, newDesc);

            if (success)
            {
                channel.Name = newName;
                channel.Description = newDesc;

                int index = Channels.IndexOf(channel);
                if (index >= 0)
                    Channels[index] = channel; 
            }

            return success;
        }

        [RelayCommand]
        public void JoinProject()
        {
            OnJoinProject?.Invoke();
        }

        [RelayCommand]
        public async Task JoinProjectBtn(string url)
        {
            try
            {
                var uri = new Uri(url);
                string token = uri.Segments.Last();
                string? result = await _projectService.JoinProject(token);
                JoinMessage = result ?? "Erreur lors de la tentative de rejoindre le projet.";
            }
            catch (UriFormatException)
            {
                JoinMessage = "L'URL fournie est invalide.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur JoinProjectBtn: " + ex.Message);
                JoinMessage = "Une erreur inattendue est survenue.";
            }
        }

        [RelayCommand]
        private void CreateProjectBtn() { OnProjectCreate?.Invoke(); }

        [RelayCommand]
        private void CreateSprintBtn() { OnCreateSprint?.Invoke(); }

        public async Task<bool> ConfirmCreateProject(string projectName, string projectDesc)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return false;

            bool success = await _projectService.CreateProject(projectName, projectDesc);
            if (!success)
                return false;

            var projects = await _projectService.GetAllProjects();
            Projects.Clear();
            foreach (var p in projects)
                Projects.Add(p);

            return true;
        }

        public async Task<bool> ConfirmCreateSprint(string sprintGoal, string sprintBeginDate, string sprintEndDate)
        {
            if (SelectedProject == null || string.IsNullOrWhiteSpace(sprintGoal))
                return false;

            bool success = await _sprintService.CreateSprint(SelectedProject.id, sprintGoal, sprintBeginDate, sprintEndDate);
            if (!success)
                return false;

            var sprints = await _sprintService.GetSprint(SelectedProject.id);
            Sprints.Clear();
            foreach (var sprint in sprints)
                Sprints.Add(sprint);

            return true;
        }
        public async Task LoadUsers()
        {
            Users.Clear();
            var result = await _userService.GetAllUsersForProject(SelectedProject.id);
            foreach (var user in result)
            {
                Users.Add(user);
            }    
        }

        [RelayCommand]
        private void ProjectSettings()
        {
            Project project = SelectedProject;
            RequestEditProject?.Invoke(project);
        }

        public void UpdateProject(Project project)
        {
            SelectedProject = project;
        }

        [RelayCommand]
        private async Task Invite()
        {
            if (SelectedProject is null) return;
            OnInviteTokenReceived?.Invoke();
        }
        public async Task<string?> GenerateInvitationToken(string expiresAt)
        {
            var token = await _projectService.InviteToProject(SelectedProject.id, expiresAt);
            return token;
        }
    }
}
