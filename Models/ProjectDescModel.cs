using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class ProjectDescModel
    {
        public int TotalRecords { get; set; }
        public Int64 ID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectDesc { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ProjectDescViewModel
    {
        public List<ProjectDescModel> ListProjectDesc { get; set; }
        public Pager pager { get; set; }
    }
}