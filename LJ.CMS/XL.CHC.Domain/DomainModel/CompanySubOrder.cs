using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class CompanySubOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int? Series { get; set; }

        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public bool? Deleted { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual CompanyOrder ParentOrder { get; set; } 
        public virtual Category BookStatus { get; set; }

        public virtual Company Company { get; set; }
    }

    public class CompanySubOrderSearchModel
    {
        public Guid? CompanyId { get; set; }
        public Nullable<DateTime> MinSearchTime { get; set; }
        public Nullable<DateTime> MaxSearchTime { get; set; }
        public int? SelectedBookStatus { get; set; } = null;

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
