using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Thiskord_Front.Models
{

    public class AuthRequest
    {
        [JsonPropertyName("user_auth")]
        public string userAuth { get; set; } = "";
        [JsonPropertyName("password")]
        public string password { get; set; } = "";

        public AuthRequest()
        {
        }
        public AuthRequest(string _userAuth, string _password)
        {
            this.userAuth = _userAuth;
            this.password = _password;
        }
    }
    
    public class User
    {
        [JsonPropertyName("user_id")]
        public int? userId { get; set; }

        [JsonPropertyName("user_name")]
        public string userName { get; set; } = "";
        [JsonPropertyName("user_mail")]
        public string userMail { get; set; } = "";
        [JsonPropertyName("user_picture")]
        public string userPicture { get; set; } = "";
    }
    public class AuthenticatedUser
    {
        [JsonPropertyName("user")]
        public User user { get; set; } = new User();

        [JsonPropertyName("token")]
        public string token { get; set; } = "";
        
    }
}
