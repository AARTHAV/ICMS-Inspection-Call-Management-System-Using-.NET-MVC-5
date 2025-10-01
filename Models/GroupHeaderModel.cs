using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class GroupHeaderModel
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public DateTime GroupDttm { get; set; }
        public string GroupBy { get; set; }
        public string GroupDpcd { get; set; }

        
        [DataType(DataType.MultilineText)]
        [DisplayName("Remark")]
        public string Remark { get; set; }

        public List<GroupLineModel> listGroupLine { get; set; }
        public List<InspReqModel> listInspReqModel { get; set; }

        public List<AssignInspectorModel> listAssignInspectorModel { get; set; }
        public List<RequestPlannerDetailsModel> listRequestPlannerDetailsModels { get; set; }

        public string Reason { get; set; }

        public string ResnID { get; set; }
    }
    public class GroupHeaderViewModel
    {
        public List<GroupHeaderModel> ListGroup { get; set; }
        public Pager pager { get; set; }
    }

    public class GroupLineModel
    {
        public int GroupLineID { get; set; }
        public int GroupID { get; set; }

        public int ReqID { get; set; }

        public string Status { get; set; }
        
    }
}