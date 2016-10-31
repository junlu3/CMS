using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class WorkShopSearchViewModel
    {
        public Guid Id { get; set; }
        public string WorkShop_Name { get; set; }
        public IPagedList<MSDS_WorkShop> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string KeyWord { get; set; }
    }

    public class WorkShopViewModel
    {
        public Guid Id { get; set; }
        public string WorkShop_Name { get; set; }
        public Guid Company_Id { get; set; }
    }
}