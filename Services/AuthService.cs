using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thiskord_Front.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Thiskord_Front.Services;
using Windows.Security.Cryptography.Core;
using System.Diagnostics;
    
namespace Thiskord_Front.Services
{
    public class AuthService
    {
        private ApiService apiService = new();
        public async Task<AuthenticatedUser?> login(string jsonRequest)
        {
            AuthenticatedUser? res;
            string jsonResult = await this.apiService.CallApiAsync("auth/auth", "POST", jsonRequest);
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                res = JsonSerializer.Deserialize<AuthenticatedUser>(jsonResult, options);
            } else
            {
                res = null;
            }
            return res ?? new AuthenticatedUser();
       
        }
      
        public async Task<UserAccount?> register(string jsonRequest)
        {
            UserAccount? res;
            string jsonResult = await this.apiService.CallApiAsync("inscription/register", "POST", jsonRequest);
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                res = JsonSerializer.Deserialize<UserAccount>(jsonResult, options);
            }
            else
            {
                res = null;
            }
            return res;
        }
    }
}