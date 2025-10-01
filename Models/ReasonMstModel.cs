using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class ReasonMstModel
    {
        [DisplayName("Reason ID")]
        public int RsnID { get; set; }
      
        [DisplayName("Reason Description")]
        [Required]
        public string RsnDesc { get; set; }
        [DisplayName("Reason DateTime")]
        public DateTime RsnRegDttm { get; set; }

        public Boolean IsReqiredFileUpload { get; set; }
        [DisplayName("Reason By")]
        public string RsnRegBy { get; set; }
        [DisplayName("Reason Status")]
        public int RsnStatus { get; set; }
    }
    public class ReasonMstViewModel
    {
        public List<ReasonMstModel> ListResnMaster { get; set; }
        public Pager pager { get; set; }
    }
}