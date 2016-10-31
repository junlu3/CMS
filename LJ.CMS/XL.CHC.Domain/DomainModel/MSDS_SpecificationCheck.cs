using System;

namespace XL.CHC.Domain.DomainModel
{
    public class MSDS_SpecificationCheck
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public DateTime? Create_Date { get; set; }
        public string Create_By { get; set; }
        public DateTime? Update_Date { get; set; }
        public string Update_By { get; set; }
        public string RejectReason { get; set; }
        //public virtual Company Company { get; set; }
        public virtual MSDS_Specification Specification { get; set; }
    }
}
