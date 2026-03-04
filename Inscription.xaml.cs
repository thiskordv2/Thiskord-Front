using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI;
using ABI.Microsoft.UI;

namespace Thiskord_Front.Pages
{
    public sealed partial class InscriptionPage : Page
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public InscriptionPage()
        {
            this.InitializeComponent();
        }

        private void Button_Inscription_Click(object sender, RoutedEventArgs e)
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
                ShowMessage("Tous les champs sont obligatoires.", true);
                return;
            }

            // Vérifie que les mots de passe correspondent
            if (password != confirmPassword)
            {
                ShowMessage("Les mots de passe ne correspondent pas.", true);
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

                // Crée ce qu'il y a dans la  requête
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Utilisation de Task.Run pour éviter de bloquer l'UI
                Task.Run(async () =>
                {
                    try
                    {
                        // URL de l'API ma is je sais pas faire 
                        var response = await _httpClient.PostAsync("http://ton-api-url/api/inscription/register", content);

                        string responseContent = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            this.DispatcherQueue.TryEnqueue(() =>
                            {
                                ShowMessage(responseContent, true);
                            });
                        }
                        else
                        {
                            this.DispatcherQueue.TryEnqueue(() =>
                            {
                                ShowMessage("Inscription réussie!", false);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        this.DispatcherQueue.TryEnqueue(() =>
                        {
                            ShowMessage($"Une erreur est survenue : {ex.Message}", true);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                ShowMessage($"Une erreur est survenue : {ex.Message}", true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}
