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
        public string User { get; set; } = "";
        public string Text { get; set; } = "";
        public string DateTime { get; set; } = "";
    }
}
