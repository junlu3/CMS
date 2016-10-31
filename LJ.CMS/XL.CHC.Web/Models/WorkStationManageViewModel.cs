using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class WorkStationSearchViewModel
    {
        public Guid Id { get; set; }
        public string WorkStation_Name { get; set; }
        public IPagedList<MSDS_WorkStation> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string KeyWord { get; set; }
        public IList<SelectListItem> WorkShops { get; set; }
        public string WorkShop_Id { get; set; } = "00000000-0000-0000-0000-000000000000";
    }

    public class WorkStationViewModel
    {
        public Guid Id { get; set; }
        public string WorkStation_Name { get; set; }
        public IList<SelectListItem> WorkShops { get; set; }
        public string WorkShop_Id { get; set; }
    }
}