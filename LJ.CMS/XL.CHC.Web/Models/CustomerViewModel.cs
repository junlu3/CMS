using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class CustomerSearchViewModel
    {
        public IPagedList<MSDS_Customer> ViewList { get; set; } 
        public string Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;

    }
}