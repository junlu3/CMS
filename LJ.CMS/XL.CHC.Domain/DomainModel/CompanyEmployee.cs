using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class CompanyEmployee
    {
        public bool? Deleted { get; set; } = false;

        public Guid Id { get; set; } = Guid.NewGuid();
        public int? AdverseMonthes { get; set; }

        public int? TotalWorkMonthes { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;
        public Nullable<DateTime> EndPostDate { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<DateTime> LeaveDate { get; set; }
        public Nullable<DateTime> StartPostDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string AdverseFactor { get; set; }
        public string Comment { get; set; }
        public string ContactPhone { get; set; }
        public string CreatedBy { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }

        public string ProtectType { get; set; }
        public string UpdatedBy { get; set; }
        public string WorkNumber { get; set; }
        public string WorkType { get; set; }

        public virtual EmployeeBaseInfo EmployeeBaseInfo { get; set; }
        public virtual Company Company { get; set; }

        public virtual Category HealthStatus { get; set; }
        public virtual Category Married { get; set; }
        public virtual Category MigrantWorker { get; set; }

        public virtual IList<CompanyOrder> CompanyOrders { get; set; }
    }

    public enum CompanyEmployeePostStatus
    {
        Stay = 1,
        Leave = 0
    }
    public class CompanyEmployeeSearchModel
    {
        public Guid? CompanyId { get; set; }
        public int? SelectedPostStatus { get; set; }

        public int? SelectedHealthStatus { get; set; }

        public string SelectedAdverseFactor { get; set; } = null;
        public string SelectedDepartment { get; set; } = null;

        public string SeletedWorkType { get; set; } = null;
        public string KeyWords { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
