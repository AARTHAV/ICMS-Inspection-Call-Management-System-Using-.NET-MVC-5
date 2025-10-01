using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class UserMstModel
    {
        [DisplayName("Employee ID")]
        [Required]
        public string EmpNo { get; set; }
        [DisplayName("Status")]
        public int UsrStatus { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime UsrRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string UsrRegBy { get; set; }

        public string Password { get; set; }
        public string RoleName { get; set; }
        public string EmpName { get; set; }
        public string EmpPsNo { get; set; }

        public string EmployeeDeparment { get; set; }
        public string EmployeeEmail { get; set; }
    }
    public class UserMstViewModel
    {
        public List<UserMstModel> ListUserMst { get; set; }
        public Pager pager { get; set; }
    }
}