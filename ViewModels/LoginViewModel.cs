using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Thiskord_Front.Services;
using System.Net.Security;
using Thiskord_Front.Models;

namespace Thiskord_Front.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly SessionService _sessionService;

        // Propriétés liées à la Vue (Binding)
        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        private bool isLoading;

        public bool IsNotLoading => !IsLoading;

        // Événement pour demander à la Vue d'afficher la Popup
        // (Le VM ne doit pas manipuler l'UI directement)
        public event Action<string>? OnSimulationPopupRequested;

        public LoginViewModel()
        {
            _authService = new AuthService();
            _sessionService = SessionService.Instance;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                return;

            IsLoading = true;
            try
            {

                AuthRequest requestPayload = new AuthRequest(Username,  Password);
                string jsonRequest = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions { WriteIndented = true });
                
                // 3. Appel simulé au service
                var response = await _authService.login(jsonRequest);

                // 4. Préparation de la réponse au format JSON
                string jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                string user = response.user.userName;
                string token = response.token;
                _sessionService.Login(user, token);
                // 5. Combinaison de la requête et de la réponse pour affichage
                string fullInfo = $"=== REQUÊTE ENVOYÉE ===\n{jsonRequest}\n\n=== RÉPONSE REÇUE ===\n{jsonResponse}";

                // 6. Déclencher l'événement pour afficher la popup dans la Vue
                OnSimulationPopupRequested?.Invoke(fullInfo);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
