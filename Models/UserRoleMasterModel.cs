using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class UserRoleMasterModel
    {
        [DisplayName("Employee ID")]
        [Required]
        public string EmpNo { get; set; }

        [DisplayName("Role ID")]
        public string RoID { get; set; }

        [DisplayName("Role Name")]
        public string RoDesc { get; set; }

        [DisplayName("Status")]
        public int UrStatus { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime UrRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string UrRegBy { get; set; }
    }

    public class UserRoleMasterViewModel
    {
        public List<UserRoleMasterModel> ListUserRoleMst { get; set; }
        public Pager pager { get; set; }
    }
}