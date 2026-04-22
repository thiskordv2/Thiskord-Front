using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Thiskord_Front.Models.Project;
using Thiskord_Front.ViewModels;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;

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
        }
        // Evitez les fuites de mémoire :
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.RequestEditProject += NavigateToProjectSettings;
            ViewModel.OnInviteTokenReceived += ShowInviteGenerationDialog;
            ViewModel.OnLogoutSuccess += OnLogoutSuccess;
            ViewModel.RequestEditChannel += OnRequestEditChannel;
            ViewModel.OnProjectCreate += CreateProjectTask;
            ViewModel.OnJoinProject += JoinProjectTask;

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
            ViewModel.RequestEditChannel -= OnRequestEditChannel;
            ViewModel.OnProjectCreate -= CreateProjectTask;
            ViewModel.OnInviteTokenReceived -= ShowInviteGenerationDialog;
            ViewModel.OnJoinProject -= JoinProjectTask;
        }

        private void NavigateToProjectSettings(Project project) { this.Frame.Navigate(typeof(ProjectSettings), project); }
        private void OnLogoutSuccess() { this.Frame.Navigate(typeof(Login)); }
        private void OnRequestEditChannel(Channel channel) { _ = EditChannelAsync(channel); }

        private async void ShowInviteGenerationDialog()
        {
            string? generatedToken = null;

            ContentDialog dialog = null;

            var generateText = new TextBlock
            {
                Text = "Création d'un lien d'invitation pour votre projet",
                TextWrapping = TextWrapping.Wrap
            };

            var expiresAtPicker = new DatePicker
            {
                Header = "Date d'expiration du token",
            };

            var generateButton = new Button
            {
                Content = "Générer l'invitation",
            };

            var errorMessageBlock = new TextBlock
            {
                Visibility = Visibility.Collapsed,
                Text = ""
            };

            generateButton.Click += async (s, e) =>
            {
                if (expiresAtPicker.SelectedDate == null)
                {
                    errorMessageBlock.Text = "Choisissez une date d'expiration valide.";
                    errorMessageBlock.Visibility = Visibility.Visible;
                    return;
                }

                errorMessageBlock.Visibility = Visibility.Collapsed;

                string expiresAt = expiresAtPicker.SelectedDate!.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

                generatedToken = await ViewModel.GenerateInvitationToken(expiresAt);

                if (string.IsNullOrEmpty(generatedToken))
                {
                    errorMessageBlock.Text = "Une erreur est survenue lors de la génération du lien d'invitation.";
                    errorMessageBlock.Visibility = Visibility.Visible;
                    return; 
                }

                bool isValidUrl = Uri.TryCreate(generatedToken, UriKind.Absolute, out Uri? uriResult)
                                  && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (isValidUrl)
                {
                    dialog.Hide();
                }
                else
                {
                    errorMessageBlock.Text = generatedToken; 
                    errorMessageBlock.Visibility = Visibility.Visible;
                    generatedToken = null;
                }

            };  

            var container = new StackPanel
            {
                Spacing = 16,
                Children = { generateText, expiresAtPicker, generateButton, errorMessageBlock }
            };

            dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Lien d'invitation",
                Content = container,
                PrimaryButtonText = "Fermer",
                DefaultButton = ContentDialogButton.None,
                MinWidth = 420
            };

            await dialog.ShowAsync();

            if (!string.IsNullOrEmpty(generatedToken))
            {
                ShowInviteDialog(generatedToken);
            }
        }

        private async void ShowInviteDialog(string token)
        {
            var tokenCard = new Border
            {
                Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(0, 16, 0, 0)
            };

            var tokenText = new TextBlock
            {
                Text = token,
                TextWrapping = TextWrapping.Wrap,
                FontFamily = new FontFamily("Cascadia Code, Consolas, monospace"),
                FontSize = 13,
                Foreground = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"],
                IsTextSelectionEnabled = true,
                LineHeight = 20
            };

            tokenCard.Child = tokenText;

            var helperText = new TextBlock
            {
                Text = "Partagez ce lien pour inviter quelqu'un à rejoindre votre projet.",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
                Margin = new Thickness(0, 0, 0, 0)
            };

            var copyIcon = new FontIcon { Glyph = "\uE8C8", FontSize = 14 };
            var copyLabel = new TextBlock { Text = "Copier le lien" };

            var copyButtonContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children = { copyIcon, copyLabel }
            };

            var copyButton = new Button
            {
                Content = copyButtonContent,
                Style = (Style)Application.Current.Resources["AccentButtonStyle"],
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 12, 0, 0),
                Padding = new Thickness(0, 10, 0, 10)
            };

            var feedbackText = new TextBlock
            {
                Text = "✓  Copié dans le presse-papiers",
                FontSize = 12,
                Foreground = (Brush)Application.Current.Resources["SystemFillColorSuccessBrush"],
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 8, 0, 0),
                Opacity = 0,
                Visibility = Visibility.Collapsed
            };

            copyButton.Click += async (s, e) =>
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(token);
                Clipboard.SetContent(dataPackage);

                copyIcon.Glyph = "\uE8FB";
                copyLabel.Text = "Copié !";
                copyButton.IsEnabled = false;
                feedbackText.Visibility = Visibility.Visible;
                feedbackText.Opacity = 1;

                await Task.Delay(2000);

                copyIcon.Glyph = "\uE8C8";
                copyLabel.Text = "Copier le lien";
                copyButton.IsEnabled = true;
                feedbackText.Opacity = 0;
                feedbackText.Visibility = Visibility.Collapsed;
            };

            var container = new StackPanel
            {
                Spacing = 0,
                Children = { helperText, tokenCard, copyButton, feedbackText }
            };

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Lien d'invitation",
                Content = container,
                PrimaryButtonText = "Fermer",
                DefaultButton = ContentDialogButton.None,
                MinWidth = 420
            };

            await dialog.ShowAsync();
        }

        private async void CreateProjectTask()
        {
            await CreateProject();
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

        private void SprintListing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private async void JoinProjectTask()
        {
            await JoinProject();
        }
        private async Task JoinProject()
        {
            var title = new TextBlock
            {
                Text = "Entrez l'url reçu"
            };

            var urlBox = new TextBox
            {
                PlaceholderText = "https://api.emre-ak.fr/api/invite/examplelink",
                AcceptsReturn = false,
            };

            var container = new StackPanel
            {
                Spacing = 10,
                Children = { title, urlBox }
            };

            var dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "Rejoindre un projet",
                Content = container,
                PrimaryButtonText = "Rejoindre",
                SecondaryButtonText = "Annuler",
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string url = urlBox.Text.Trim();

                if (!string.IsNullOrWhiteSpace(url))
                {
                    await ViewModel.JoinProjectBtn(url);
                }
            }
        }
    }
}