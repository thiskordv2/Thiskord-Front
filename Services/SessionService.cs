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
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public void Login(string user, string token)
        {
            CurrentUser = user;
            Token = token;
        }

        public void Logout()
        {
            CurrentUser = "";
            Token = "";
        }

    }
}