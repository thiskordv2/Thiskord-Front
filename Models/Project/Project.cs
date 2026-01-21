using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Thiskord_Front.Models.Project
{
    public class Project
    {
        [JsonPropertyName("project_id")]
        public int? id { get; set; }
        [JsonPropertyName("project_name")]
        public string? name { get; set; }
        [JsonPropertyName("project_desc")]
        public string? description { get; set; }
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
            [JsonPropertyName("project_name")]
            public string name { get; set; }

            [JsonPropertyName("project_desc")]
            public string description { get; set; }
        }
    }


}

