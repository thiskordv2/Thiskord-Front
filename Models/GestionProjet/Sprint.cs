using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thiskord_Front.Models.GestionProjet
{
    public class Sprint
    {
        public int sprint_id { get; set; }
        public string sprint_goal { get; set; }
        public string sprint_begin_date { get; set; }
        public string sprint_end_date { get; set; }

        public Sprint() { }

        public Sprint(int sprint_id, string sprint_goal, string sprint_begin_date, string sprint_end_date)
        {
            this.sprint_id = sprint_id;
            this.sprint_goal = sprint_goal;
            this.sprint_begin_date = sprint_begin_date;
            this.sprint_end_date = sprint_end_date;
        }
    }
}
