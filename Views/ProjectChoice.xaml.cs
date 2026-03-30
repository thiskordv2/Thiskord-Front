using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Thiskord_Front.ViewModels;
using Thiskord_Front.Models.Project;



// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Thiskord_Front.Views
{

    public sealed partial class ProjectChoice : Page
    {
        public ProjectChoiceViewModel ViewModel { get; } = new();
        public ProjectChoice()
        {
            InitializeComponent();

            ViewModel.OnProjectCreate += async () => await CreateProject();
        }

        private async Task CreateProject()
        {
            var newProjectName = new TextBox
            {
                Text = "",
                PlaceholderText = "Nom du projet",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var newProjectDesc = new TextBox
            {
                Text = "",
                PlaceholderText = "Description (optionnelle)",
                AcceptsReturn = true,
                Height = 100,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var contentPanel = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = "Nom:", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    newProjectName,
                    new TextBlock { Text = "Description:", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    newProjectDesc
                }
            };

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Crée un projet",
                Content = contentPanel,
                PrimaryButtonText = "Créer",
                SecondaryButtonText = "Annuler"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string newName = newProjectName.Text.Trim();
                string newDescription = newProjectDesc.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(newName))
                {
                    var errorDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Erreur",
                        Content = "Le nom du projet ne peut pas être vide.",
                        PrimaryButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                    return;
                }

                bool success = await ViewModel.ConfirmCreateProject(
                    newName,
                    newDescription);

                if (!success)
                {
                    await new ContentDialog
                    {
                        XamlRoot = XamlRoot,
                        Title = "Erreur",
                        Content = $"Impossible de crée le project « {newName} ».",
                        PrimaryButtonText = "OK"
                    }.ShowAsync();
                }
            }
        }
    }
}
