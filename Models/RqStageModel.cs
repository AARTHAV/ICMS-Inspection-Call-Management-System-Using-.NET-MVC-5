using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class StageModel
    {
        public int TotalRecords { get; set; }
        [DisplayName("Inspection Stage ID")]
        public int StgID { get; set; }
        [DisplayName("Inspection Stage Description")]
        [Required]
        public string StgDesc { get; set; }
        [DisplayName("Reg DateTime")]
        public DateTime StgRegDttm { get; set; }
        [DisplayName("Reg By")]
        public string StgRegBy { get; set; }
        [DisplayName("Inspection Stage Status")]
        public int StgStatus { get; set; }
    }

    public class RqStageViewModel
    {
        public List<StageModel> ListStage { get; set; }
        public Pager pager { get; set; }
    }
}