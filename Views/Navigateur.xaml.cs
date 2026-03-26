using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;
using Thiskord_Front.ViewModels;

namespace Thiskord_Front.Views
{
    public sealed partial class Navigateur : Page
    {
        public static Frame NavigateurFrame { get; set; }

        public NavigateurViewModel ViewModel { get; }

        private readonly ProjectService _projectService = new ProjectService();
        private readonly ChannelService _channelService = new ChannelService();
        private bool _serverMenuInitialized;
        private Channel? _contextMessageMenu;

        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

        public Navigateur()
        {
            InitializeComponent();
            NavigateurFrame = InnerFrame;
            InnerFrame.Navigate(typeof(ns_choice));
            channelListing.ItemsSource = Channels;

            ViewModel = new NavigateurViewModel();
            ViewModel.OnLogoutSuccess += () =>
            {
                this.Frame.Navigate(typeof(Login));
            };
        }

        private async void ServerMenuFlyout_Opening(object sender, object e)
        {
            if (_serverMenuInitialized)
                return;

            if (!ServerMenuFlyout.Items.Any())
                ServerMenuFlyout.Items.Add(new MenuFlyoutItem { Text = "Chargement..." });

            List<Project> projects = await _projectService.GetAllProjects();

            ServerMenuFlyout.Items.Clear();
            if (projects.Count > 0)
            {
                foreach (var project in projects)
                {
                    var menuItem = (new MenuFlyoutItem
                    {
                        Text = project.name,
                        Tag = project,

                    });
                    menuItem.Click += OnOpenProject_Click;
                    ServerMenuFlyout.Items.Add(menuItem);
                }
                _serverMenuInitialized = true;
            }
            else
            {
                ServerMenuFlyout.Items.Add(new MenuFlyoutItem { Text = "Aucun projet trouvé", IsEnabled = false });
            }
        }

        private async void OnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(sender);
            if (sender is MenuFlyoutItem itemCliked)
            {
                if (itemCliked.Tag is Project project)
                {
                    openedProjectTitle.Content = project.name ?? "erreur d'affichage";
                    Channels.Clear();

                    List<Channel> channels = await _projectService.GetChannelsForProject(project.id.Value);

                    foreach (var channel in channels)
                    {
                        Channels.Add(channel);
                    }
                }
            }
        }

        private void channelListing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (channelListing.SelectedItem is Channel selectedChannel)
            {
                InnerFrame.Navigate(typeof(ChannelPage), selectedChannel);
                // Clear selection so the same channel can be clicked again to re-navigate.
                channelListing.SelectedItem = null;
            }
        }

        private void ChannelContextMenu_Opening(object sender, object e)
        {
            if (sender is MenuFlyout flyout &&
                flyout.Target is TextBlock tb &&
                tb.DataContext is Channel channel)
            {
                _contextMessageMenu = channel;
            }
        }

        private async void ChannelContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMessageMenu is null || sender is not MenuFlyoutItem item)
                return;

            switch (item.Tag as string)
            {
                case "edit":
                    await EditChannelAsync(_contextMessageMenu);
                    break;

                case "delete":
                    await DeleteChannelAsync(_contextMessageMenu);
                    break;
            }

            _contextMessageMenu = null;
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
                string newName = nameTextBox.Text?.Trim() ?? channel.Name;
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

                bool success = await _channelService.EditChannel(channel.Id.Value, newName, newDescription);

                if (success)
                {
                    // Mettre à jour le channel dans la collection
                    channel.Name = newName;
                    channel.Description = newDescription;

                    // Forcer le refresh de l'UI
                    var index = Channels.IndexOf(channel);
                    if (index >= 0)
                    {
                        Channels[index] = channel;
                    }
                }
                else
                {
                    var errorDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Erreur",
                        Content = $"Impossible de modifier le channel « {channel.Name} ».",
                        PrimaryButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async Task DeleteChannelAsync(Channel channel)
        {
            if (channel.Id is null) return;

            bool success = await _channelService.DeleteChannel(channel.Id.Value);

            if (success)
            {
                // On retire de l'UI seulement si l'API confirme la suppression
                Channels.Remove(channel);
            }
            else
            {
                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Erreur",
                    Content = $"Impossible de supprimer le channel « {channel.Name} ».",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }
    }
}