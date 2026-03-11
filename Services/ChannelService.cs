using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;

namespace Thiskord_Front.Services
{
    public class ChannelService
    {
        private readonly ApiService _apiService;

        public ChannelService()
        {
            _apiService = new ApiService();
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
    }
}
