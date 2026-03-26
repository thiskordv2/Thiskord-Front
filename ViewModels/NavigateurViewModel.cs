using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models;
using Thiskord_Front.Services;
using Thiskord_Front.Views;

namespace Thiskord_Front.ViewModels
{
    public partial class NavigateurViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly SessionService _sessionService;
        
        public event Action OnLogoutSuccess;

        public NavigateurViewModel()
        {
            _authService = new AuthService();
            _sessionService = SessionService.Instance;
        }

        [RelayCommand]
        private async Task Logout()
        {
            _sessionService.Logout();
            OnLogoutSuccess.Invoke();
        }    
    }
}
