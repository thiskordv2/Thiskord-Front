using System.Text.Json.Serialization;

namespace Thiskord_Front.Models
{
    public class UserAccount
    {
        public int? user_id { get; set; }

        [JsonPropertyName("user_name")]
        public string? user_name { get; set; }

        [JsonPropertyName("password")]
        public string? user_password { get; set; }

        [JsonPropertyName("user_mail")]
        public string? user_mail { get; set; }

        [JsonPropertyName("user_picture")]
        public string? user_picture { get; set; }
        public UserAccount() { }

        public UserAccount(int _user_id, string _user_name, string _user_password, string _user_mail, string _user_picture)
        {
            this.user_id = _user_id;
            this.user_name = _user_name;
            this.user_password = _user_password;
            this.user_mail = _user_mail;
            this.user_picture = _user_picture;
        }

        public UserAccount(int _user_id, string _user_name, string _user_mail, string _user_picture)
        {
            this.user_id = _user_id;
            this.user_name = _user_name;
            this.user_mail = _user_mail;
            this.user_picture = _user_picture;
        }
    }
}