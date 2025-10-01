using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class LocMstModel
    {
        public int TotalRecords { get; set; }
        [DisplayName("Location ID")]
        public int LcID { get; set; }
        [DisplayName("Location Name")]
        [Required]
        public string LcName { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime LcRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string LcRegBy { get; set; }
        [DisplayName("Location Status")]
        public int LcStatus { get; set; }
    }

    public class LocationMstViewModel
    {
        public List<LocMstModel> ListLocationMaster { get; set; }
        public Pager pager { get; set; }
    }
}