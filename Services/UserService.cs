using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models;

namespace Thiskord_Front.Services
{
    public class UserService
    {
        ApiService apiService = new();
        public async Task<List<UserAccount?>> GetAllUsers()
        {
            string jsonResult = await this.apiService.CallApiAsync("user/all", "GET", null);
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    var users = JsonSerializer.Deserialize<List<UserAccount>>(jsonResult, options) ?? new List<UserAccount>();
                    return users;
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Désérialisation: " + ex.Message);
                    return new List<UserAccount?>();
                }
            }
            else
            {
                return new List<UserAccount?>();
            }
        }

        public async Task<List<UserAccount?>> GetAllUsersForProject(int projectId)
        {
            string jsonResult = await this.apiService.CallApiAsync($"user/project/{projectId}", "GET", null);
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    var users = JsonSerializer.Deserialize<List<UserAccount>>(jsonResult, options) ?? new List<UserAccount>();
                    return users;
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Désérialisation: " + ex.Message);
                    return new List<UserAccount?>();
                }
            }
            else
            {
                return new List<UserAccount?>();
            }
        }
    }
}
