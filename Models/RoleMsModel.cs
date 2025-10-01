using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class RoleMsModel
    {
        public int TotalRecords { get; set; }
        [DisplayName("Role ID")]
        public int RoID { get; set; }
        [DisplayName("Role Description")]
        [Required]
        public string RoDesc { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime RoRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string RoRegBy { get; set; }
        [DisplayName("Role Status")]
        public int RoStatus { get; set; }
    }

    public class RoleMsViewModel
    {
        public List<RoleMsModel> ListRoleMaster { get; set; }
        public Pager pager { get; set; }
    }
}