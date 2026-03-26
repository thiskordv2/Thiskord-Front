using ABI.Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Thiskord_Back.Models.Account;
using Thiskord_Front.Models;
using Thiskord_Front.Services;
using Windows.UI;

namespace Thiskord_Front.Views
{
    public sealed partial class InscriptionPage : Page
    {
        private readonly AuthService authService = new AuthService();

        public InscriptionPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Inscription_Click(object sender, RoutedEventArgs e)
        {
            // Récupère les valeurs des champs
            string email = MailTextBox.Text?.Trim() ?? string.Empty;
            string userName = UserTextBox.Text.Trim();
            string password = PasswordBox1.Password.Trim();
            string confirmPassword = PasswordBox2.Password.Trim();

            // Vérifie que tous les champs sont remplis
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowMessage("Tous les champs sont obligatoires.", "error");
                return;
            }

            if (!IsValidEmail(email))
            {
                ShowMessage("L'adresse e-mail n'est pas valide.", "error");
                return;
            }

            // Vérifie que les mots de passe correspondent
            if (password != confirmPassword)
            {
                ShowMessage("Les mots de passe ne correspondent pas.", "error");
                return;
            }

            
            ErrorTextBlock.Visibility = Visibility.Collapsed;
            // Objet de requête
            var userRequest = new UserAccount()
            {
                user_mail = email,
                user_name = userName,
                user_password = password
            };

            try
            {
                string jsonRequest = JsonSerializer.Serialize(userRequest);
                var response = await authService.register(jsonRequest);

                if (response is null)
                {
                    ShowMessage("Échec de la création du compte", "error");
                    return;
                }

                ShowMessage("Inscription réussie ! \nRedirection en cours", "good");
                await Task.Delay(2000);
                this.Content = new Login(); 
            }
            catch (Exception ex)
            {
                ShowMessage($"Une erreur est survenue : {ex.Message}", "error");
            }

        }

        private void ShowMessage(string message, string opt)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
            if (opt == "error") { ErrorTextBlock.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red); }
            else if (opt == "good") { ErrorTextBlock.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green); }
            
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            try
            {
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private void Button_Connexion_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(InscriptionPage));
        }
    }
}
