using System;
using Thiskord_Front.Models;

namespace Thiskord_Front.Services
{
    public class SessionService
    {
        public string CurrentUser { get; private set; }
        public string Token { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

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