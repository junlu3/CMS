using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
   public  class EmployeeWorkHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<DateTime> LeaveDate { get; set; }
        public string CompanyName { get; set; }
        public string Department { get; set; }
        public string WorkType { get; set; }
        public string AdverseFactor { get; set; }
        public string ProtectType { get; set; }
        public bool? Deleted { get; set; } = false;
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public virtual EmployeeBaseInfo EmployeeBaseInfo { get; set; }
    }
}
