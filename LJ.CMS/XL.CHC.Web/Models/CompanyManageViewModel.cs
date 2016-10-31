using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class CompanyManageViewModel
    {
        public IPagedList<Company> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public DateTime? STime { get; set; }
        public DateTime? ETime { get; set; }
        public string KeyWord { get; set; }
    }


}