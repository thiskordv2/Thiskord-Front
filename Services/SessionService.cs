using System;
using Thiskord_Front.Models;

namespace Thiskord_Front.Services
{
    public class SessionService
    {
        private static readonly Lazy<SessionService> _instance = new(() => new SessionService());
        public static SessionService Instance => _instance.Value;
        public string CurrentUser { get; private set; } = string.Empty;
        public string Token { get; private set; } = string.Empty;
        public UserAccount? CurrentUserAccount { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public void Login(UserAccount user, string token)
        {
            CurrentUserAccount = user;
            CurrentUser = user.user_name ?? string.Empty;
            Token = token;
        }

        public void Logout()
        {
            CurrentUser = string.Empty;
            Token = string.Empty;
            CurrentUserAccount = null;
        }

    }
}