using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Windows.Services.Maps;

namespace Thiskord_Front.Services
{
    public class ChannelService
    {
        private readonly ApiService _apiService = new();

        public async Task<List<Channel>> GetChannelsForProject(int projectId)
        {
            string jsonResult = await _apiService.CallApiAsync($"channel/project/{projectId}", "GET");
            if (!string.IsNullOrEmpty(jsonResult))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                try
                {
                    var channels = JsonSerializer.Deserialize<List<Channel>>(jsonResult, options) ?? new List<Channel>();
                    return channels;
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur Désérialisation des channels: " + ex.Message);
                    return new List<Channel>();
                }
            }
            else
            {
                return new List<Channel>();
            }
        }

        public async Task<bool> DeleteChannel(int channelId)
        {
            // CallApiAsync retourne null en cas d'erreur, string (vide ou JSON) en cas de succès
            string? result = await _apiService.CallApiAsync($"channel/{channelId}", "DELETE");
            return result != null;
        }
        public async Task<bool> EditChannel(int channelId, string jsonRequest)
        {
            string? result = await _apiService.CallApiAsync($"channel/{channelId}", "PUT", jsonRequest);
            return result != null;
        }
        public async Task<bool> EditChannel(int channelId, string name, string description)
        {
            var payload = new
            {
                name = name,
                description = description
            };

            string json = JsonSerializer.Serialize(payload);
            string? result = await _apiService.CallApiAsync($"channel/{channelId}", "PUT", json);
            return result != null;
        }
        public async Task<bool> CreateChannel(string name, string description, int projectId)
        {
            var payload = new
            {
                name = name,
                description = description,
                projectId = projectId
            };

            string json = JsonSerializer.Serialize(payload);
            string? result = await _apiService.CallApiAsync("channel/create", "POST", json);
            return result != null;
        }
    }
}
