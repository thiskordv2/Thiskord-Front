using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Thiskord_Front.ViewModels;
using Thiskord_Front.Models.Project;

namespace Thiskord_Front.Views
{

    public sealed partial class ProjectSettings : Page
    {
        ProjectSettingsViewModel ViewModel { get; } = new();
        public ProjectSettings()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.ProjectUpdateSuccessful += OnProjectUpdateSuccessful;
            ViewModel.ProjectUpdateCancelled += OnProjectUpdateCancelled;
            ViewModel.AskConfirmDelete += OnAskConfirmDelete;
            ViewModel.ProjectDeleteSuccessful += OnProjectDeleteSuccessful;

            if (e.Parameter is Project project)
                ViewModel.LoadProject(project);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.ProjectUpdateSuccessful -= OnProjectUpdateSuccessful; 
            ViewModel.ProjectUpdateCancelled -= OnProjectUpdateCancelled;
            ViewModel.AskConfirmDelete -= OnAskConfirmDelete;
            ViewModel.ProjectDeleteSuccessful -= OnProjectDeleteSuccessful; 
        }

        private void OnProjectUpdateSuccessful(Project project) => ReturnToNavigateur(project);
        private void OnProjectUpdateCancelled(Project project) => ReturnToNavigateur(project);
        private void OnProjectDeleteSuccessful() { this.Frame.Navigate(typeof(Navigateur)); }

        private async void OnAskConfirmDelete()
        {
            var titleConfirm = new TextBlock { Text = $"Etes vous sur de vouloir supprimé {ViewModel.ProjectName}?" };
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Confirmation de suppression",
                Content = titleConfirm,
                PrimaryButtonText = "Supprimer",
                CloseButtonText = "Annuler"
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary) return;
            ViewModel.DeleteProjectCommand.Execute(null);
        }
        private void ReturnToNavigateur(Project project)
        {
            if (Frame.CanGoBack)
                this.Frame.Navigate(typeof(Navigateur), project);
        }
    }
}
