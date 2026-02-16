using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.Generic;
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

        public Navigateur()
        {
            InitializeComponent();
            NavigateurFrame = InnerFrame;
            InnerFrame.Navigate(typeof(ns_choice));
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

            if (ServerMenuFlyout.IsOpen)
            {
                var target = ServerMenuFlyout.Target as FrameworkElement;
                ServerMenuFlyout.Hide();
                if (target is not null)
                    ServerMenuFlyout.ShowAt(target);
            }
        }

        private async void OnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(sender);
            if (sender is MenuFlyoutItem itemCliked)
            {
                if (itemCliked.Tag is Project project)
                {
                    openedProjectTitle.Text = project.name ?? "erreur d'affichage";
                }
            }
        }
    }
}