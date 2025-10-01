using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class AssignInspectorModel
    {
        public int InsID { get; set; }
        public int PlnID { get; set; }
        public int InsPsNo { get; set; }
        public string InsPsName { get; set; }
        public DateTime? InsDttm { get; set; }
        public string InsBy { get; set; }
        public string InspDpcd { get; set; }

        public string TypeOfInspector { get; set; }
    }
}