using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Thiskord_Front.Models;
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
            //InnerFrame.Navigate(typeof(#NomFrameAfficher));

            EditChannelFromXamlCommand = ViewModel.EditChannelCommand;
            DeleteChannelFromXamlCommand = ViewModel.DeleteChannelCommand;

            ViewModel.OnLogoutSuccess += () => { this.Frame.Navigate(typeof(Login)); };
            ViewModel.RequestEditChannel += channel => _ = EditChannelAsync(channel);
            ViewModel.RequestEditUser += () => _ = OpenUserSettingsAsync();

            ViewModel.OnProjectCreate += async () => await CreateProject();
            ViewModel.LoadUsers();
        }

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
        }

        private void ChannelListing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (channelListing.SelectedItem is Channel selectedChannel)
            {
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
                Title = "Créer un projet",
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
                        Content = $"Impossible de créer le projet « {newName} ».",
                        PrimaryButtonText = "OK"
                    }.ShowAsync();
                }
            }
        }

        private async Task OpenUserSettingsAsync()
        {
            UserAccount? user = ViewModel.ConnectedUser;

            string currentUserName = user?.user_name ?? "Non défini";
            string currentUserMail = user?.user_mail ?? "Non défini";

            const int nameRowIndex = 2;
            const int mailRowIndex = 3;

            var editProfileButton = new Button
            {
                Content = "Modifier mon profil",
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var cancelEditButton = new Button
            {
                Content = "Annuler",
                HorizontalAlignment = HorizontalAlignment.Left,
                Visibility = Visibility.Collapsed
            };

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children =
                {
                    editProfileButton,
                    cancelEditButton
                }
            };

            var profileContent = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Mon profil",
                        Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"]
                    },
                    new PersonPicture
                    {
                        Width = 64,
                        Height = 64,
                    },
                    new TextBlock { Text = $"Nom : {currentUserName}" },
                    new TextBlock { Text = $"Mail : {currentUserMail}" },
                    buttonsPanel
                }
            };

            var sideBar = new StackPanel
            {
                Width = 125,
                Spacing = 4,
                Children =
                {
                    new Button
                    {
                        Content = "Mon profil",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Background = null,
                        BorderThickness = new Thickness(0)
                    },
                    new Button
                    {
                        Content = "Sécurité",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Background = null,
                        BorderThickness = new Thickness(0)
                    },
                    new Button
                    {
                        Content = "Préférences",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Background = null,
                        BorderThickness = new Thickness(0)
                    }
                }
            };

            var layout = new Grid
            {
                ColumnSpacing = 24
            };
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(125) });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });

            Grid.SetColumn(sideBar, 0);
            Grid.SetColumn(profileContent, 1);

            layout.Children.Add(sideBar);
            layout.Children.Add(profileContent);

            editProfileButton.Click += (_, _) =>
            {
                var nameInput = new TextBox
                {
                    Header = "Nom",
                    Text = currentUserName
                };

                var mailInput = new TextBox
                {
                    Header = "Mail",
                    Text = currentUserMail
                };

                profileContent.Children[nameRowIndex] = nameInput;
                profileContent.Children[mailRowIndex] = mailInput;

                editProfileButton.IsEnabled = false;
                cancelEditButton.Visibility = Visibility.Visible;
            };

            cancelEditButton.Click += (_, _) =>
            {
                profileContent.Children[nameRowIndex] = new TextBlock { Text = $"Nom : {currentUserName}" };
                profileContent.Children[mailRowIndex] = new TextBlock { Text = $"Mail : {currentUserMail}" };

                editProfileButton.IsEnabled = true;
                cancelEditButton.Visibility = Visibility.Collapsed;
            };

            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Paramètres utilisateur",
                Content = layout,
                PrimaryButtonText = "Fermer",
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }
    }
}