using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICMS.Models
{
    public class ProjectMaster
    {
        public string ProjectName { get; set; }
        public List<SelectListItem> listProject { get; set; }
    }
}