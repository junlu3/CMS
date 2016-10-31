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
    public class HospitalBookSearchViewModel
    {
        public Nullable<DateTime> MinSearchTime { get; set; }
        public Nullable <DateTime> MaxSearchTime { get; set; }
        public Guid? SelectedComany { get; set; }
        public List<SelectListItem> CompanyList { get; set; } = new List<SelectListItem>();
        public int? SelectedBookStatus { get; set; }
        public List<SelectListItem> BookStatusList { get; set; } = new List<SelectListItem>();
        public int PageIndex { get; set; } = 1;

        public IPagedList<CompanySubOrder> CompanySubOrders { get; set; }
    }

    public class HospitalBookProcessViewModel
    {
        public CompanySubOrder CompanySubOrder { get; set; } = new CompanySubOrder();
    }
}
