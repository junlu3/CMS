using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class CompanyOrderSearchViewModel
    {
        public Nullable<DateTime> MinSearchTime { get; set; }
        public Nullable<DateTime> MaxSearchTime { get; set; }

        public Guid? SelectedCompanyId { get; set; }
        public IList<SelectListItem> CompanyList { get; set; } = new List<SelectListItem>();
        //public int? SelectedBookStatus { get; set; } = null;
        //public List<SelectListItem> BookStatusList { get; set; } = new List<SelectListItem>();
        public IPagedList<CompanyOrder> CompanyOrders { get; set; }
        public int PageIndex
        {
            get; set;
        } = 1;
    }

    public class CompanyOrderCreateViewModel
    {
        public Guid? OrderId { get; set; } = null;
        public string Code { get; set; }
        public string Comment { get; set; }

        /// <summary>
        /// split by ,
        /// </summary>
        public string AssignedSubOrderString { get; set; }
        public IList<CompanySubOrder> AssignedSubOrders { get; set; } = new List<CompanySubOrder>();
        public IList<CompanySubOrder> NotAssignedSubOrders { get; set; } = new List<CompanySubOrder>();

        /// <summary>
        /// split by ,
        /// </summary>
        /// 
        public string AssignedEmployeesString { get; set; }
        public IList<CompanyEmployee> AssignedEmployees { get; set; } = new List<CompanyEmployee>();
        public IList<CompanyEmployee> NotAssignedEmployees { get; set; } = new List<CompanyEmployee>();
 

        public int? HealthYear { get; set; } = null;

        public bool IsLocked { get; set; } = false;



    }

    public class CompanyOrderDownLoadViewModel
    {
        public CompanyOrder Order { get; set; }

        public bool IsHospitalRole { get; set; } = false;

    }

}
