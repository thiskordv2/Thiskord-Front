using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly UserService _userService = new();

        [ObservableProperty]
        private Project? selectedProject;

        [ObservableProperty]
        private Channel? selectedChannel;

        [ObservableProperty]
        private bool isLoadingProjects;
        public ObservableCollection<Channel> Channels { get; } = new();
        public ObservableCollection<Project> Projects { get; } = new();
        public ObservableCollection<UserAccount> Users { get;  } = new();

        public event Action? OnLogoutSuccess;
        public event Action<Channel>? RequestEditChannel;
        public event Action<Project>? RequestEditProject;

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

            var channels = await _channelService.GetChannelsForProject(project.id);
            Channels.Clear();
            if (channels == null) { selectedChannel = null; return; }
            foreach (var c in channels) Channels.Add(c);
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
        public async Task LoadUsers()
        {
            Users.Clear();
            var result = await _userService.GetAllUsers();
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
    }
}
