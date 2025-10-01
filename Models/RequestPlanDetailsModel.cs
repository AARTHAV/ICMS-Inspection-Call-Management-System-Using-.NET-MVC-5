using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ICMS.Models
{
    public class RequestPlannerDetailsModel
    {
        public int ID { get; set; }
        //RqstPlnDtl
        [DisplayName("Plan ID")]
        public int PlnID { get; set; }
        [DisplayName("Request ID")]
        public int RqID { get; set; }
        [DisplayName("Request Type")]
        public string RqstType { get; set; }
        [DisplayName("Agency")]
        public int AgcyID { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayName("Start Date & Time")]
        public DateTime? PlnStart { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayName("End Date & Time")]
        public DateTime? PlnEnd { get; set; }
        [DisplayName("Planned On")]
        public DateTime? PlnDttm { get; set; }
        [DisplayName("Planned By")]
        public string PlnBy { get; set; }
        [DisplayName("Department")]
        public string PlnDpcd { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Remark")]
        public string PlnRemark { get; set; }
        [DisplayName("Start Date & Time")]
        public DateTime? ActulStart { get; set; }

        public Boolean IsClosed { get; set; }
        public Boolean IsReturn { get; set; }
        [DisplayName("End Date & Time")]
        public DateTime? ActulEnd { get; set; }
        [DisplayName("Reason")]
        public int ResnID { get; set; }

        [DisplayName("Closed Date & Time")]
        public DateTime? InsDttm { get; set; }

        [DisplayName("Inspected By")]
        public string InsBy { get; set; }
        [DisplayName("Department")]
        public string InspDpcd { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        [DisplayName("Remark")]
        public string InspRemark { get; set; }

        public string RqNo { get; set; }
        public string RqRevNo { get; set; }
        public string FilePath { get; set; }
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlsx|.doc|.docx|.pdf|.png|.jpg|.gif)$", ErrorMessage = "Only document and image files allowed.")]
        public HttpPostedFileBase[] UploadedFile { get; set; }
        public string RqRegBy { get; set; }

        public List<InspReqModel> objInspReqModel { get; set; }
        public List<DocumentModel> lstDocument { get; set; }

        [DisplayName("Inspector")]
        public List<EmployeeModel> lstEmployee { get; set; }
        public List<SelectListItem> Name_of_Employees { get; set; }
        public List<AssignInspectorModel> listAssignInspector { get; set; }

        public List<TpiMstModel> lstTpiMaster { get; set; }
        public IEnumerable<string> EmpPsNo { get; set; }

        public string EmployeeName { get; set; }

        public string AgencyName { get; set; }

        [DisplayName("Reason")]
        public string ReasonName { get; set; }

        public string tempValue { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Start Date & Time")]
        public DateTime? ExtenalRquestPlnStart { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayName("End Date & Time")]
        public DateTime? ExtenalRquestPlnEnd { get; set; }
        [DataType(DataType.MultilineText)]
        [DisplayName("Remark")]
        public string ExternalPlnRemark { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Planned on")]
        public DateTime? ExtenalPlannedOn { get; set; }

        public int IsParallel { get; set; }

        public IEnumerable<string> ExternalEmpPsNo { get; set; }

        public List<SelectListItem> External_Name_of_Employees { get; set; }

        public string ExternaltempValue { get; set; }

        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.xls|.xlsx|.doc|.docx|.pdf|.png|.jpg|.gif)$", ErrorMessage = "Only document and image files allowed.")]
        public HttpPostedFileBase[] UploadedExternalFile { get; set; }

        public int RqType { get; set; }


        [DisplayName("Start Date & Time")]
        public DateTime? ActulExternalStart { get; set; }
        [DisplayName("End Date & Time")]
        public DateTime? ActulExternalEnd { get; set; }
        [DisplayName("Reason")]
        public int ExternalResnID { get; set; }

        [DisplayName("Closed Date & Time")]
        public DateTime? InsExternalDttm { get; set; }

        [DisplayName("Inspected By")]
        public string InsExternalBy { get; set; }
        [DisplayName("Department")]
        public string InspExternalDpcd { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        [DisplayName("Remark")]
        public string InspExternalRemark { get; set; }

        [DisplayName("Reason")]
        public string ReasonExternalName { get; set; }
        [DisplayName("Please select")]
        public string InspectionQuestion { get; set; }
        public string ProjectNo { get; set; }
        public string ForgingNo { get; set; }
    }
    public class RequestPlannerDetailsViewModel
    {
        public List<RequestPlannerDetailsModel> ListRequestPlannerDetails { get; set; }
        public Pager pager { get; set; }
    }
}