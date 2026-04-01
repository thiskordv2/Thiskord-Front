using CommunityToolkit.Mvvm.ComponentModel;
using Thiskord_Front.Services;
using Thiskord_Front.Models.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace Thiskord_Front.ViewModels
{
    public partial class ProjectSettingsViewModel : ObservableObject
    {
        ProjectService projectService = new();

        [ObservableProperty]
        private string projectName = string.Empty;

        [ObservableProperty]
        private string projectDescription = string.Empty;

        [ObservableProperty]
        private string editTitle = string.Empty;

        public event Action<Project>? ProjectUpdateSuccessful;
        public event Action? ProjectDeleteSuccessful;
        public event Action<Project>? ProjectUpdateCancelled;

        [ObservableProperty]
        private Project? projectActuelle;

        public void LoadProject(Project project)
        {
            if (project == null) return;
            ProjectActuelle = project;
            ProjectName = project.name;
            ProjectDescription = project.description;
            EditTitle = $"Modifier le Project {ProjectName}";
        }

        [RelayCommand]
        public async Task SaveProject()
        {
            ProjectActuelle.name = ProjectName;
            ProjectActuelle.description = ProjectDescription;
            try
            {
                await projectService.EditProject(ProjectActuelle);
                ProjectUpdateSuccessful?.Invoke(ProjectActuelle);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error updating project: " + ex.Message);
            }
        }

        [RelayCommand]
        public async Task DeleteProject()
        {
            try
            {
                await projectService.DeleteProject(ProjectActuelle.id);
                ProjectDeleteSuccessful?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error deleting project: " + ex.Message);
            }
        }

        [RelayCommand]
        public void CancelEdit()
        {
            ProjectUpdateCancelled?.Invoke(ProjectActuelle);
        }
    }
}
