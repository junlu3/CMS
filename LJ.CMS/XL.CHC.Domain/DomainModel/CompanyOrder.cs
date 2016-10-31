using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class CompanyOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Code { get; set; }
        public string Comment { get; set; }
        public bool? Deleted { get; set; } = false;
        public bool? Locked { get; set; } = false;
        public bool? IsBuildCompleted { get; set; } = false;
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }

        public virtual Company Company { get; set; }
        public virtual IList<CompanyEmployee> CompanyEmployees { get; set; } = new List<CompanyEmployee>();
        public virtual IList<CompanySubOrder> SubOrders { get; set; } = new List<CompanySubOrder>();
    }

    public class CompanyOrderSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public Guid? CompanyId { get; set; } = null;
        public DateTime? CreatedStartTime { get; set; }
        public DateTime? CreatedEndTime { get; set; }

        public DateTime? MinBookTime { get; set; }
        public DateTime? MaxBookTime { get; set; }
    }

}
