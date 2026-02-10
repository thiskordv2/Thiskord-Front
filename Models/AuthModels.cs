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

        public AuthRequest(string _userAuth, string _password)
        {
            this.userAuth = _userAuth;
            this.password = _password;
        }
    }
    
    public class User
    {
        [JsonPropertyName("user_name")]
        public string userName { get; set; } = "";
        [JsonPropertyName("user_mail")]
        public string userMail { get; set; } = "";
        [JsonPropertyName("user_picture")]
        public string userPicture { get; set; } = "";

        public User(string _userName, string _userMail, string _userPicture)
        {
            this.userName = _userName;
            this.userMail = _userMail;
            this.userPicture = _userPicture;
        }
    }
    public class AuthenticatedUser
    {
        [JsonPropertyName("user")]
        public User user { get; set; } = new User("", "", "");

        [JsonPropertyName("token")]
        public string token { get; set; } = "";

        [JsonConstructor]
        public AuthenticatedUser(User _user, string _token)
        {
            this.user = _user;
            this.token = _token;
        }
    }
}
