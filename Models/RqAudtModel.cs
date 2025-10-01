using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class RqAudtModel
    {
        public int AudtID { get; set; }
        public string RqstId { get; set; }
        public string Rqrevno { get; set; }
        public string AudtPsNo { get; set; }
        public string Audtdpcd { get; set; }
        public DateTime Audtdttm { get; set; }
        public string AudtstName { get; set; }
        public string Audtxt { get; set; }
    }
}