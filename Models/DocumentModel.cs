using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICMS.Models
{
    public class DocumentModel
    {
        public int DocID { get; set; }

        public int RqstID { get; set; }
        public int RqRevNo { get; set; }
        public string DocName { get; set; }
        public string DocAddr { get; set; }

        public DateTime? UpldDttm { get; set; }
        public string UpldBy { get; set; }
        public string DocDadp { get; set; }
    }
}