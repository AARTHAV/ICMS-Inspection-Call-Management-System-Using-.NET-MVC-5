using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ICMS.Controllers;
using System.Web.Mvc;

namespace ICMS.Models
{
    public class MaintainEmailModel
    {
        public int Flag { get; set; }

        [DisplayName("Head ID")]
        public int HedID { get; set; }

        [DisplayName("Project")]
        [Required]
        public string Project { get; set; }

        [DisplayName("Mail ID")]
        [Required]
        public string EmailAddr { get; set; }

        [DisplayName("Mail Type")]
        [Required]
        public string EmailType { get; set; }

        [DisplayName("Active")]
        [Required]
        public int IsActive { get; set; }

        [DisplayName("Registered DateTime")]
        public DateTime RegDttm { get; set; }

        [DisplayName("Registered By")]
        public string RegBy { get; set; }

        public string vwMail { get; set; }
        public string tempvwMail { get; set; }
        public List<SelectListItem> listvwMail { get; set; }
        public List<SelectListItem> listvwProject { get; set; }

    }
    public class MaintainEmailViewModel
    {
        public List<MaintainEmailModel> ListEmail { get; set; }
    }
}