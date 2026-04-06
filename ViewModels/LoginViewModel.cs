using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models;
using Thiskord_Front.Services;

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
        private string errorMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotLoading))]
        private bool isLoading;

        public bool IsNotLoading => !IsLoading;

        public event Action? OnLoginSuccess;
        public event Action? OnNavigateToRegister;

        public LoginViewModel()
        {
            _authService = new AuthService();
            _sessionService = SessionService.Instance;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Veuillez remplir tous les champs.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {

                AuthRequest requestPayload = new AuthRequest(Username,  Password);
                string jsonRequest = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions { WriteIndented = true });
                
                AuthenticatedUser response = await _authService.login(jsonRequest);

                _sessionService.Login(response.user.userName, response.token);
                OnLoginSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la connexion :\n{ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Register()
        {
            OnNavigateToRegister?.Invoke();
        }
    }
}
