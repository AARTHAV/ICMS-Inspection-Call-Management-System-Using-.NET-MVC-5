using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class EmployeeModel
    {
        public int EmpID { get; set; }
        public string EmpPsNo { get; set; }
        public string EmpName { get; set; }
        public string EmpMail { get; set; }
        public string EmpDpcd { get; set; }
        public Boolean EmpStatus { get; set; }
    }
}