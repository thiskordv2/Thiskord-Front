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
        private ApiService apiService;
        public AuthService()
        {
             apiService = new ApiService();
        }
        // Simule un appel réseau
        public async Task<AuthenticatedUser> login(string jsonRequest)
        {
            AuthenticatedUser? res;
            string jsonResult = await this.apiService.CallApiAsync("auth/auth", "POST", jsonRequest);
            System.Diagnostics.Debug.WriteLine("API payload: " + (jsonResult ?? "null"));
            System.Diagnostics.Debug.WriteLine(jsonResult);
            System.Diagnostics.Debug.WriteLine("connard");
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                res = JsonSerializer.Deserialize<AuthenticatedUser>(jsonResult, options);
            } else
            {
                res = null;
            }
            System.Diagnostics.Debug.Write(res);

            return res ?? new AuthenticatedUser();
           
        }
        
    }
}
