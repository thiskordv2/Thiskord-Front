using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;

namespace Thiskord_Front.ViewModels
{
    public partial class NavigateurViewModel : ObservableObject
    {
        private readonly SessionService _sessionService = SessionService.Instance;
        private readonly ProjectService _projectService = new();
        private readonly ChannelService _channelService = new();
       
        [ObservableProperty]
        private string selectedProjectName = "Mes serveurs";
        [ObservableProperty]
        private bool isLoadingProjects;
        private int? _currentProjectId;
        public ObservableCollection<Channel> Channels { get; } = new();
        public ObservableCollection<Project> Projects { get; } = new();

        public event Action? OnLogoutSuccess;
        public event Action<Channel>? RequestEditChannel;
        public event Action? RequestCreateChannel;

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
            SelectedProjectName = project.name ?? "Projet sans nom";
            _currentProjectId = project.id;

            var channels = await _channelService.GetChannelsForProject(project.id);
            Channels.Clear();
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
        [RelayCommand]
        private void CreateChannel() => RequestCreateChannel?.Invoke();

        public async Task<bool> ConfirmCreateChannel(string channelName, string channelDesc)
        {
            if (string.IsNullOrWhiteSpace(channelName) || _currentProjectId is null)
                return false;

            bool success = await _channelService.CreateChannel(channelName, channelDesc, _currentProjectId.Value);
            if (!success)
                return false;

            var channels = await _channelService.GetChannelsForProject(_currentProjectId.Value);
            Channels.Clear();
            foreach (var c in channels)
                Channels.Add(c);

            return true;
        }
    }
}
