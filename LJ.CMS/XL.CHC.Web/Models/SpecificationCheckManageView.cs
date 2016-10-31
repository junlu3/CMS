using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XL.CHC.Web.Models
{
    public class SpecificationCheckViewModel
    {
        public Guid Id { get; set; }
        public Guid Company_Id { get; set; }
        public int Status { get; set; }
        public DateTime? Create_Date { get; set; }
        public string Create_By { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Update_By { get; set; }
        public string RejectReason { get; set; }
    }
}