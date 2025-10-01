using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class EmailTempModel
    {
        public int TotalRecords { get; set; }
        [DisplayName("Template ID")]
        
        public int EtempID { get; set; }
        [DisplayName("Template Type")]
        [Required]
        public string EtempType { get; set; }
        [DisplayName("Template Subject")]
        [Required]
        public string EtempSub { get; set; }
        [DisplayName("Template Cont")]
        [Required]
        public string EmtpCont { get; set; }
        [DisplayName("Template To")]
        [Required]
        public string EmtpTo { get; set; }
        [DisplayName("Template CC")]
        [Required]
        public string EmtpCc { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime RegDttm { get; set; }
        [DisplayName("Reg By")]
        public string RegBy { get; set; }
        [DisplayName("Email Template Status")]
        public int Status { get; set; }
    }

    public class EmailTempViewModel
    {
        public List<EmailTempModel> ListEmailTemplate { get; set; }
        public Pager pager { get; set; }
    }
}