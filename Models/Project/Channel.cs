using System.Text.Json.Serialization;

namespace Thiskord_Front.Models.Project
{
    public class Channel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("projectId")]
        public int ProjectId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public string DisplayName => $"# {Name}";
    }
}
