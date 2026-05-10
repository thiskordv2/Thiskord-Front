using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using Thiskord_Front.Models.GestionProjet;
using Thiskord_Front.ViewModels;

namespace Thiskord_Front.Views
{
    public sealed partial class SprintPage : Page
    {
        public SprintPageViewModel ViewModel { get; }

        public SprintPage()
        {
            ViewModel = new SprintPageViewModel();
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SprintNavigationParameter parameter)
            {
                await ViewModel.LoadSprintAsync(parameter.Sprint, parameter.ProjectId);
            }
            else if (e.Parameter is Sprint sprint)
            {
                await ViewModel.LoadSprintAsync(sprint, 0);
            }
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var titleBox = new TextBox
            {
                PlaceholderText = "Titre de la tâche",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var descriptionBox = new TextBox
            {
                PlaceholderText = "Description de la tâche",
                AcceptsReturn = true,
                Height = 100,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var errorText = new TextBlock
            {
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemFillColorAttentionBrush"],
                Visibility = Visibility.Collapsed,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var content = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = "Créer une nouvelle tâche", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold },
                    titleBox,
                    descriptionBox,
                    errorText
                }
            };

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Nouvelle tâche",
                Content = content,
                PrimaryButtonText = "Créer",
                CloseButtonText = "Annuler",
                DefaultButton = ContentDialogButton.Primary,
                MinWidth = 420
            };

            dialog.PrimaryButtonClick += async (s, args) =>
            {
                errorText.Visibility = Visibility.Collapsed;
                if (string.IsNullOrWhiteSpace(titleBox.Text))
                {
                    errorText.Text = "Le titre est requis.";
                    errorText.Visibility = Visibility.Visible;
                    args.Cancel = true;
                    return;
                }

                bool success = await ViewModel.ConfirmCreateTaskAsync(titleBox.Text.Trim(), descriptionBox.Text?.Trim());
                if (!success)
                {
                    errorText.Text = "Impossible de créer la tâche. Vérifiez les informations et réessayez.";
                    errorText.Visibility = Visibility.Visible;
                    args.Cancel = true;
                }
            };

            await dialog.ShowAsync();
        }
    }
}
