using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class AgencyMstModel
    {
        public int TotalRecords { get; set; }
        [DisplayName("Agency ID")]
        public int AgID { get; set; }
        [DisplayName("Agency Name")]
        [Required]
        public string AgName { get; set; }
      
        [DisplayName("Reg DateTime")]
        public DateTime AgRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string AgRegBy { get; set; }
        [DisplayName("Agency Status")]
        public int AgStatus { get; set; }

    }
    public class AgencyMstViewModel
    {
        public List<AgencyMstModel> ListAgencyMst { get; set; }
        public Pager pager { get; set; }
    }
}