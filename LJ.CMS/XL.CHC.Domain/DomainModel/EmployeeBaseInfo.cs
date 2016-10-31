using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public  class EmployeeBaseInfo
    {
        public bool? Deleted { get; set; } = false;
        public Guid Id { get; set; } = Guid.NewGuid();
        public Nullable<DateTime> CreatetDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public string IDCard { get; set; }
        public string Sex { get; set; }
        public string UserName { get; set; }

        public virtual IList<CompanyEmployee> CompanyEmployees { get; set; } = new List<CompanyEmployee>();
        public virtual IList<EmployeeWorkHistory> WorkHistories { get; set; } = new List<EmployeeWorkHistory>();
       
         
    }
}
