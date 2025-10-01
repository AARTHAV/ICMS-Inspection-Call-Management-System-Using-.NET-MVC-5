using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class TpiMstModel
    {
        public int TotalRecords { get; set; }
        [DisplayName("Agency Employee ID")]
        public int TpID { get; set; }
        [DisplayName("Agency Name")]
        public int AgID { get; set; }
        [DisplayName("Agency Name")]
        public string AgName { get; set; }
        [DisplayName("Agency Employee Name")]
        [Required]
        public string TpName { get; set; }
        [DisplayName("Agency Employee Contact")]
        [Required]
        public string TpMob { get; set; }
        [DisplayName("Agency Employee Email")]
        [Required]
        public string TpMail { get; set; }
        [DisplayName("Tpi Prm")]
        [Required]
        public int TpPrm { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime TpRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string TpRegBy { get; set; }
        [DisplayName("Agency Employee Status")]
        public int TpStatus { get; set; }


    }
    public class TpiMstViewModel
    {
        public List<TpiMstModel> ListTpiMst { get; set; }
        public Pager pager { get; set; }
    }
}