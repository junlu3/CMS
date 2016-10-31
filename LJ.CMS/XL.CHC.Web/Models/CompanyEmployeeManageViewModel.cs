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
    public class CompanyEmployeeSearchViewModel
    {

        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 离职/在岗
        /// </summary>
        public List<SelectListItem> PostStatusList { get; set; } = new List<SelectListItem>();
        public int? SelectedPostStatus { get; set; }

        public List<SelectListItem> HealthStatusList { get; set; } = new List<SelectListItem>();
        public int? SelectedHealthStatus { get; set; }
        public IList<string> AdverseFactorList { get; set; } = new List<string>();
        public string SelectedAdverseFactor { get; set; } = null;
        public string KeyWords { get; set; } = string.Empty;

        public IList<string> DepartmentList { get; set; } = new List<string>();
        public string SelectedDepartment { get; set; } = null;

        public IList<string> WorkTypeList { get; set; } = new List<string>();
        public string SelectedWorkType { get; set; } = null;

        public IPagedList<CompanyEmployee> Employees { get; set; } = new PagedList<CompanyEmployee>(new List<CompanyEmployee>(), 1, 20, 0);
    }

    public class CompanyEmployeeViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "姓名不能为空")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "身份证不能为空")]
        public string IDCard { get; set; }
        public string Sex { get; set; }
        public int? AdverseMonthes { get; set; }
        public int? TotalWorkMonthes { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        [Required(ErrorMessage ="入职时间不能为空")]
        public DateTime StartPostDate { get; set; }
        public DateTime? EndPostDate { get; set; }
        public string AdverseFactor { get; set; }
        public string Comment { get; set; }
        public string ContactPhone { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string ProtectType { get; set; }
        public string WorkNumber { get; set; }
        public string WorkType { get; set; }
        public Guid CompanyId { get; set; }
        public int HealthStatusId { get; set; }
        public int MarriedId { get; set; }
        public int MigrantWorkerId { get; set; }
        public List<SelectListItem> AvailableCompanies { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableHealthStatus { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableMarried { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableMigrantWorker { get; set; } = new List<SelectListItem>();
    }
}
