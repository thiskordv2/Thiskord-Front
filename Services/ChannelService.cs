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
    }
}
