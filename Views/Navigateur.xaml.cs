using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Thiskord_Front.Models.Project;
using Thiskord_Front.ViewModels;

namespace Thiskord_Front.Views
{
    public sealed partial class Navigateur : Page
    {
        public static Frame? NavigateurFrame { get; private set; }

        public NavigateurViewModel ViewModel { get; }

        public static ICommand EditChannelFromXamlCommand { get; private set; }
        public static ICommand DeleteChannelFromXamlCommand { get; private set; }

        public Navigateur()
        {
            ViewModel = new NavigateurViewModel();

            InitializeComponent();
            NavigateurFrame = InnerFrame;
            InnerFrame.Navigate(typeof(ProjectChoice));

            EditChannelFromXamlCommand = ViewModel.EditChannelCommand;
            DeleteChannelFromXamlCommand = ViewModel.DeleteChannelCommand;
        }
        // Evitez les fuites de mémoire :
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.RequestEditProject += NavigateToProjectSettings;
            ViewModel.OnLogoutSuccess += OnLogoutSuccess;
            ViewModel.RequestEditChannel += OnRequesEditChannel;

            if (e.Parameter is Project project)
            {
                await ViewModel.SelectProjectCommand.ExecuteAsync(project);
                await ViewModel.LoadUsers();
                RightPanel.Visibility = Visibility.Visible;
                if (ViewModel.SelectedChannel is not null) InnerFrame.Navigate(typeof(ChannelPage), ViewModel.SelectedChannel);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.RequestEditProject -= NavigateToProjectSettings;
            ViewModel.OnLogoutSuccess -= OnLogoutSuccess;
            ViewModel.RequestEditChannel -= OnRequesEditChannel;
        }

        private void NavigateToProjectSettings(Project project) { this.Frame.Navigate(typeof(ProjectSettings), project); }
        private void OnLogoutSuccess() { this.Frame.Navigate(typeof(Login)); }
        private void OnRequesEditChannel(Channel channel) { _ = EditChannelAsync(channel); }


        private async void ServerMenuFlyout_Opening(object sender, object e)
        {
            await ViewModel.LoadProjectsCommand.ExecuteAsync(null);

            ServerMenuFlyout.Items.Clear();

            if (ViewModel.Projects.Count == 0)
            {
                ServerMenuFlyout.Items.Add(new MenuFlyoutItem
                {
                    Text = "Aucun projet trouvé",
                    IsEnabled = false
                });
                return;
            }

            foreach (var project in ViewModel.Projects)
            {
                var item = new MenuFlyoutItem
                {
                    Text = project.name,
                    Tag = project
                };
                item.Click += OnOpenProject_Click;
                ServerMenuFlyout.Items.Add(item);
            }
        }

        private async void OnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem { Tag: Project project })
                await ViewModel.SelectProjectCommand.ExecuteAsync(project);
            ViewModel.LoadUsers();
            ViewModel.SelectedChannel = null;
            RightPanel.Visibility = Visibility.Visible;
        }

        private void ChannelListing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (channelListing.SelectedItem is Channel selectedChannel)
            {
                ViewModel.SelectedChannel = selectedChannel;
                InnerFrame.Navigate(typeof(ChannelPage), selectedChannel);
                channelListing.SelectedItem = null;
            }
        }

        private async Task EditChannelAsync(Channel channel)
        {
            if (channel.Id is null) return;

            // Créer les TextBox pour l'édition
            var nameTextBox = new TextBox
            {
                Text = channel.Name ?? "",
                PlaceholderText = "Nom du channel",
                Margin = new Thickness(0, 0, 0, 10)
            };

            var descriptionTextBox = new TextBox
            {
                Text = channel.Description ?? "",
                PlaceholderText = "Description (optionnelle)",
                AcceptsReturn = true,
                Height = 100,
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Créer un StackPanel pour contenir les contrôles
            var contentPanel = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    new TextBlock { Text = "Nom:", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    nameTextBox,
                    new TextBlock { Text = "Description:", FontWeight = Microsoft.UI.Text.FontWeights.Bold },
                    descriptionTextBox
                }
            };

            // Créer et afficher le ContentDialog
            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = $"Modifier le channel « {channel.Name} »",
                Content = contentPanel,
                PrimaryButtonText = "Enregistrer",
                SecondaryButtonText = "Annuler"
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string newName = nameTextBox.Text.Trim();
                string newDescription = descriptionTextBox.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(newName))
                {
                    var errorDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Erreur",
                        Content = "Le nom du channel ne peut pas être vide.",
                        PrimaryButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                    return;
                }

                bool success = await ViewModel.ConfirmEditChannelAsync(
                    channel,
                    newName,
                    descriptionTextBox.Text?.Trim() ?? "");

                if (!success)
                {
                    await new ContentDialog
                    {
                        XamlRoot = XamlRoot,
                        Title = "Erreur",
                        Content = $"Impossible de modifier le channel « {channel.Name} ».",
                        PrimaryButtonText = "OK"
                    }.ShowAsync();
                }
            }
        }
    }
}