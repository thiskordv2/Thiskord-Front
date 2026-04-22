using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thiskord_Front.Models.Project
{
    public class MessageDto
    {
        public int Id { get; set; } = 0 ;
        public string Username { get; set; } = "";
        public string Content { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
