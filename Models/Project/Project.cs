using System.Text.Json.Serialization;

namespace Thiskord_Front.Models.Project
{
    public class Project
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }
        public Project()
        {
        }

        public Project(int id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public class ProjectRequest
        {
            [JsonPropertyName("name")]
            public string name { get; set; }

            [JsonPropertyName("description")]
            public string description { get; set; }
        }
    }


}

