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
    public class CompanyBookSearchViewModel
    {
        public Nullable<DateTime> MinSearchTime { get; set; }
        public Nullable<DateTime> MaxSearchTime { get; set; }
        public int? SelectedBookStatus { get; set; } = null;
        public List<SelectListItem> BookStatusList { get; set; } = new List<SelectListItem>();
        public IPagedList<CompanySubOrder> CompanySubOrders { get; set; }
        public int PageIndex
        {
            get; set;
        } = 1;

    }

    public class CompanyCalendarViewModel
    {
        public Guid? Id { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public string StartDateString { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string EndDateString { get; set; }
        public bool Enabled { get; set; } = true;
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 预定的状态 0 未被预定 1 预定未确认 2 确认
        /// </summary>
        public int OrderStatus { get; set; } = 0;
        public bool BelongToCompany { get; set; } = false;
    }
}
