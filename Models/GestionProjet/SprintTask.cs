using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thiskord_Front.Models.GestionProjet
{
    public class SprintTask
    {
        public int task_id { get; set; }

        public string task_title { get; set; }

        public string task_desc { get; set; }

        public Boolean is_subtask { get; set; }

        public string task_status { get; set; }

        public int id_creator { get; set; }
        public int id_resp { get; set; }
        public int id_project_task { get; set; }
        public int? id_parent_task { get; set; }

        public int id_sprint { get; set; }

        public string created_at { get; set; }
        public string updated_at { get; set; }

        public SprintTask() { }

        public SprintTask(int _task_id, string _task_title, string _task_desc, bool _is_subtask, string _task_status, int _id_sprint)
        {
            this.task_id = _task_id;
            this.task_title = _task_title;
            this.task_desc = _task_desc;
            this.is_subtask = _is_subtask;
            this.task_status = _task_status;
            this.id_sprint = _id_sprint;
        }
    }
}
