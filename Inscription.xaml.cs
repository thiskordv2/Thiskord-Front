using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Thiskord_Front.Pages
{
    public sealed partial class InscriptionPage : Page
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public InscriptionPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Inscription_Click(object sender, RoutedEventArgs e)
        {
            // Récupère les valeurs des champs
            string email = MailTextBox.Text;
            string userName = UserTextBox.Text;
            string password = PasswordBox1.Password;
            string confirmPassword = PasswordBox2.Password;

            // Vérifie que tous les champs sont remplis
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                await ShowMessageDialog("Erreur", "Tous les champs sont obligatoires.");
                return;
            }

            // Vérifie que les mots de passe correspondent
            if (password != confirmPassword)
            {
                await ShowMessageDialog("Erreur", "Les mots de passe ne correspondent pas.");
                return;
            }

            // Objet de requête
            var userRequest = new
            {
                email,
                user_name = userName,
                user_password = password,
                confirm_password = confirmPassword
            };

            try
            {
                // Convertit l'objet en JSON
                string jsonRequest = JsonSerializer.Serialize(userRequest);

                // Crée le contenu de la requête
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Remplace cette URL par celle de ton API
                var response = await _httpClient.PostAsync("http://ton-api-url/api/inscription/register", content);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Lit la réponse d'erreur
                    var errorObject = JsonSerializer.Deserialize<InscriptionErrorResponse>(responseContent);
                    await ShowMessageDialog("Erreur", errorObject.error);
                }
                else
                {
                    // Lit la réponse de succès
                    var successObject = JsonSerializer.Deserialize<InscriptionSuccessResponse>(responseContent);
                    await ShowMessageDialog("Succès", successObject.message);
                }
            }
            catch (Exception ex)
            {
                await ShowMessageDialog("Erreur", $"Une erreur est survenue : {ex.Message}");
            }
        }

        private async Task ShowMessageDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot,
            };

            await dialog.ShowAsync();
        }
    }

    // Classes pour désérialiser les réponses JSON
    public class InscriptionErrorResponse
    {
        public string error { get; set; }
    }

    public class InscriptionSuccessResponse
    {
        public int userId { get; set; }
        public string message { get; set; }
    }
}
