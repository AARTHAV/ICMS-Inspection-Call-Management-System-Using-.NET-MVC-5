using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICMS.Models
{
    public class InspReqModel
    {
        [DisplayName("Request ID")]
        public int RqID { get; set; }
        [DisplayName("Request Number")]
        public string RqNo { get; set; }

        [DisplayName("Request Reversion No")]
        public int RqRevNo { get; set; }
        [DisplayName("Project")]
        public string PrjNo { get; set; }
        [DisplayName("Forging")]
        [Required]
        public string FrgNo { get; set; }
        [DisplayName("Stage")]
        [Required]
        public int StgID { get; set; }
        [DisplayName("Stage")]
        public string StageName { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Offer Date & Time")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyyTHH:mm}")]
        public DateTime? OfrDttm { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Remark { get; set; }
        [DisplayName("Request Status")]
        public string RqStatus { get; set; }
        [DisplayName("Request Type")]
        public int RqType { get; set; }
        public string RqTypeName { get; set; }
        [DisplayName("Location")]
        [Required]
        public string LcID { get; set; }
        [DisplayName("Location")]
        public string LcName { get; set; }

        [DisplayName("Request Date & Time")]
        public DateTime? RqRegDttm { get; set; }
        [DisplayName("Request By")]
        public string RqRegBy { get; set; }

        [DisplayName("Department")]
        public string RqDpcd { get; set; }
        //[Range(typeof(bool), "true", "true", ErrorMessage = "Please select")]
        [DisplayName("Confirmation Note")]
        public Boolean RqOts { get; set; }
        public int RqstDocID { get; set; }

        public string FilePath { get; set; }
        public HttpPostedFileBase[] UploadedFile { get; set; }
        [Required]

        public string ProjectName { get; set; }
        public string tempProjName { get; set; }
        public List<SelectListItem> listProject { get; set; }

        public List<DocumentModel> lstDocument { get; set; }

        public List<RequestPlannerDetailsModel> lstRequest { get; set; }

        public List<AssignInspectorModel> lstAssignIns { get; set; }

        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? PlanStartDateTime { get; set; }
        public DateTime? PlanEndDateTime { get; set; }

        public string AgencyName { get; set; }

        public string PlanID { get; set; }
        public string IsUpdateExternalRequest { get; set; }

        public string IsUpdateInternalReturnRequest { get; set; }
        public List<TpiMstModel> listTpiMstModel { get; set; }
        public string ResnID { get; set; }

        public IEnumerable<string> lstFrgNo { get; set; }

        public List<SelectList> lstSelectdFrgNo { get; set; }

        public string hiddenfieldSelectedFrgNo { get; set; }
    }
    public class InspReqViewModel
    {
        public List<InspReqModel> ListInspReq { get; set; }
        public Pager pager { get; set; }
    }
}