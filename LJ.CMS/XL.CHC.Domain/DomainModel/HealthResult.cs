
using System;

namespace XL.CHC.Domain.DomainModel
{
    public class HealthResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string MainPositiveResult { get; set; }
        public string Result { get; set; }
        public string HealthCode { get; set; }
        public string ImageCode { get; set; }
        public string ReportCode { get; set; }
        public Nullable<DateTime> HealthDate { get; set; }
        public string ReportDate { get; set; }
        public string HealthPerson { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual CompanyEmployee CompanyEmployee { get; set; }

        public bool Deleted { get; set; } = false;

        public string HealthByCompany { get; set; }
    }

    public class HealthResultSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public Guid? SelectedCompany { get; set; }
        public string EmployeeKeyWord { get; set; }
        public string HealthResultKeyWord { get; set; }
        public string SelectedFinalResults { get; set; } = null;
        public string SelectedWorkType { get; set; } = null;

        public int? MinAdverseYears { get; set; } = null;
        public int? MaxAdverseYears { get; set; } = null;

        public string AdverseFactor { get; set; } = null;
        public Nullable<DateTime> MinHealthDate { get; set; } = null;
        public Nullable<DateTime> MaxHealthDate { get; set; } = null;


    }
}
