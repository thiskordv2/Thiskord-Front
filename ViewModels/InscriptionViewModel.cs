using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Thiskord_Back.Models.Account;
using Thiskord_Front.Services;
using Thiskord_Front.Views;

namespace Thiskord_Front.ViewModels
{
    public partial class InscriptionViewModel : ObservableObject
    {
        public AuthService authService = new();

        [ObservableProperty]
        public string name = string.Empty;
        [ObservableProperty]
        public string mail = string.Empty;
        [ObservableProperty]
        public string password1 = string.Empty;
        [ObservableProperty]
        public string password2 = string.Empty;
        [ObservableProperty]
        public string errorMessage = string.Empty;
        [ObservableProperty]
        public Brush errorMessageColor = new SolidColorBrush(Colors.Red);

        public event Action? OnRegisterSuccess;
        public event Action? OnNavigateToLogin;

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrEmpty(Name)) { ErrorMessage = "Veuillez entrer un nom d'utilisateur"; return; }
            if (string.IsNullOrEmpty(Mail)) { ErrorMessage = "Veuillez entrer un mail"; return; }
            if (string.IsNullOrEmpty(Password1) || string.IsNullOrEmpty(Password2)) { ErrorMessage = "Veuillez entrer un mot de passe";  return; }

            if (!IsMailValid(Mail)) { ErrorMessage = "Veuillez entrer un mail valide"; return; }
            if (!string.Equals(Password1,Password2)) { ErrorMessage = "Les mots de passes sont différents"; return; }       
            if (!IsPassValid(Password1)) { ErrorMessage = "Votre mot de passe doit faire au moins 8 caractère de longueur\nMettez au moins 1 majuscule, 1 minuscule et 1 chiffre"; return; }

            ErrorMessage = string.Empty;

            try
            {
                var newUser = new UserAccount()
                {
                    user_mail = Mail,
                    user_name = Name,
                    user_password = Password1
                };

                string jsonRequest = JsonSerializer.Serialize(newUser);
                var response = await authService.register(jsonRequest);
                if (response is null || response.user_id == null)
                {
                    ErrorMessage = "Échec de la création du compte";
                    return;
                }

                ErrorMessage = "Inscription réussie ! \nRedirection en cours";
                ErrorMessageColor = new SolidColorBrush(Colors.Green);
                await Task.Delay(2000);
                OnRegisterSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
        }

        [RelayCommand]
        public async Task Login()
        {
            OnNavigateToLogin?.Invoke();
        }

        private bool IsMailValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (Exception ex)
            {
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        private bool IsPassValid(string pass1)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
            return Regex.IsMatch(pass1, pattern);
        }
    }
}
