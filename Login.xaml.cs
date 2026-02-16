using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Thiskord_Front.ViewModels;
using Thiskord_Front.Pages;

namespace Thiskord_Front;

public sealed partial class Login : Page
{
    public LoginViewModel ViewModel { get; }

    public Login()
    {
        this.InitializeComponent();
        ViewModel = new LoginViewModel();
        ViewModel.OnSimulationPopupRequested += ShowJsonPopup;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
        {
            ViewModel.Password = pb.Password;
        }
    }

    private async void ShowJsonPopup(string jsonContent)
    {
        // On vérifie que le XamlRoot est accessible (nécessaire pour WinUI 3)
        if (Content is FrameworkElement fe && fe.XamlRoot != null)
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = fe.XamlRoot,
                Title = "Simulation Communication API",
                Content = new ScrollViewer
                {
                    MaxHeight = 600,
                    Content = new TextBlock
                    {
                        Text = jsonContent,
                        TextWrapping = TextWrapping.Wrap,
                        FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                        Padding = new Thickness(10)
                    }
                },
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary
            };

            // --- C'est ici que ça se passe ---

            // 1. On stocke le résultat du clic (l'exécution s'arrête ici tant que la pop-up est ouverte)
            ContentDialogResult result = await dialog.ShowAsync();

            // 2. On vérifie si l'utilisateur a cliqué sur le bouton "OK" (Primary)
            if (result == ContentDialogResult.Primary)
            {
                // 3. On remplace le contenu de la fenêtre par ta nouvelle page
                this.Content = new Navigateur();
            }
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        this.Content = new InscriptionPage();
    }
}