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

        public event Action? OnLoginSuccess;
        public event Action<string>? OnLoginFailed;

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
                
                AuthenticatedUser response = await _authService.login(jsonRequest);

                if (!string.IsNullOrEmpty(response.token))
                {
                    _sessionService.Login(response.user.userName, response.token);
                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    OnLoginFailed?.Invoke("Identifiants incorrects.");
                }
            }
            catch (Exception ex)
            {
                OnLoginFailed?.Invoke($"Erreur de connexion : {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
