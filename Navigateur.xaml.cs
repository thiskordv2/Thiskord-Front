using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;

namespace Thiskord_Front
{
    public sealed partial class Navigateur : Page
    {
        public static Frame NavigateurFrame { get; set; }

        private readonly ApiService _apiService = new ApiService();
        private bool _serverMenuInitialized;
        
       
        public ObservableCollection<Channel> Channels { get; set; } = new ObservableCollection<Channel>();

        public Navigateur()
        {
            InitializeComponent();
            NavigateurFrame = InnerFrame;
            InnerFrame.Navigate(typeof(ns_choice));
            
          
            BaseExample.ItemsSource = Channels;
        }

        private async void ServerMenuFlyout_Opening(object sender, object e)
        {
            if (_serverMenuInitialized)
                return;

            if (!ServerMenuFlyout.Items.Any())
                ServerMenuFlyout.Items.Add(new MenuFlyoutItem { Text = "Chargement..." });

            List<Project> projects = await _apiService.GetAllProjects();

            ServerMenuFlyout.Items.Clear();
            if (projects.Count > 0)
            {
                foreach (var project in projects)
                {
                    var item = new MenuFlyoutItem
                    {
                        Text = project.name,
                        Tag = project
                    };
                    item.Click += ServerMenuItem_Click;  
                    ServerMenuFlyout.Items.Add(item);
                }
                _serverMenuInitialized = true; 
            }
            else
            {
                ServerMenuFlyout.Items.Add(new MenuFlyoutItem { Text = "Aucun projet trouvé", IsEnabled = false });
            }

            if (ServerMenuFlyout.IsOpen)
            {
                var target = ServerMenuFlyout.Target as FrameworkElement;
                ServerMenuFlyout.Hide();
                if (target is not null)
                    ServerMenuFlyout.ShowAt(target);
            }
        }

     
        private async void ServerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item && item.Tag is Project project)
            {
                System.Diagnostics.Debug.WriteLine($"Projet cliqué: {project.name} (ID: {project.id})");
                
                Channels.Clear();
                
                List<Channel> channels = await _apiService.GetChannelsByProjectId(project.id.Value);
                
                System.Diagnostics.Debug.WriteLine($"Channels reçus: {channels.Count}");
                
                foreach (var channel in channels)
                {
                    System.Diagnostics.Debug.WriteLine($"Ajout channel: {channel.Name}");
                    Channels.Add(channel);
                }
                
                // Rouvrir le menu après le chargement
                if (!ServerMenuFlyout.IsOpen)
                {
                    var target = ServerMenuFlyout.Target as FrameworkElement;
                    if (target is not null)
                        ServerMenuFlyout.ShowAt(target);
                }
            }
        }
    }
}