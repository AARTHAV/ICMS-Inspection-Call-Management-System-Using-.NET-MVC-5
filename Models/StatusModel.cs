using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class StatusModel
    {
        public int TotalRecords { get; set; }

        [DisplayName("Status ID")]
        public int StID { get; set; }
        [Required]
        [DisplayName("Status Desc")]
        public string StDesc { get; set; }
        [Required]
        [DisplayName("Status")]
        public int StStatus { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime StRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string StRegBy { get; set; }

    }
    public class StatusViewModel
    {
        public List<StatusModel> ListStatus { get; set; }
        public Pager pager { get; set; }
    }
}