using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class HealthResultSearchViewModel
    {

        public int PageIndex { get; set; } = 1;

        public Guid? SelectedCompany { get; set; }
        public List<SelectListItem> CompanyList { get; set; } = new List<SelectListItem>();

        public string EmployeeKeyWord { get; set; }
        public string HealthResultKeyWord { get; set; }
        public string SelectedFinalResults { get; set; } = null;
        public List<string> FinalResultList { get; set; } = new List<string>();

        public IList<string> WorkTypeList { get; set; } = new List<string>();
        public string SelectedWorkType { get; set; } = null;

        public int? MinAdverseYears { get; set; } = null;
        public int? MaxAdverseYears { get; set; } = null;

        public string AdverseFactor { get; set; } = string.Empty;
        public Nullable<DateTime> MinHealthDate { get; set; }
        public Nullable<DateTime> MaxHealthDate { get; set; }

        public IPagedList<HealthResult> HealthResults { get; set; } = new PagedList<HealthResult>(new List<HealthResult>(), 1,20, 0);

        public string ActionType { get; set; } = string.Empty;
    }

    public class HealthResultViewModel
    {
        public Guid Id { get; set; }
        public string MainPositiveResult { get; set; }
        public string Result { get; set; }
        public string HealthCode { get; set; }
        public string ImageCode { get; set; }
        public string ReportCode { get; set; }

        public DateTime? HealthDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public string HealthPerson { get; set; }
        public string HealthByCompany { get; set; }

        public Guid CompanyEmployeeId { get; set; }
        public string IDCard { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public int? AdverseMonthes { get; set; }
        public string AdverseFactor { get; set; }
        public Guid CompanyId { get; set; }
        public List<SelectListItem> AvailableCompanies { get; set; } = new List<SelectListItem>();
    }
}
