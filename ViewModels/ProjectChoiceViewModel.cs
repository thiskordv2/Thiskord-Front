using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;

namespace Thiskord_Front.ViewModels
{
    public partial class ProjectChoiceViewModel : ObservableObject
    {
        private readonly ProjectService _projectService = new();

        public ObservableCollection<Project> Projects { get; } = new();
        public event Action? OnProjectCreate;

        //[RelayCommand]
        //public void JoinProject()
        //{
        //    // Logique pour rejoindre un serveur
        //}

        //[RelayCommand]
        //private void CreateProjectBtn() { OnProjectCreate?.Invoke(); }

        //public async Task<bool> ConfirmCreateProject(string projectName, string projectDesc)
        //{
        //    if (projectName == null) return false;
        //    bool success = await _projectService.CreateProject(projectName, projectDesc);
        //    return success;
        //}
    }
}
