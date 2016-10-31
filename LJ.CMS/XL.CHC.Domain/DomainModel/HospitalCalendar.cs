using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class HospitalCalendar
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public bool Enabled { get; set; } = true;
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; } = DateTime.Now;
    }
}
