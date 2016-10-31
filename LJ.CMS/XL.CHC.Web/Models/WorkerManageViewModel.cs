using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class WorkerSearchViewModel
    {
        public Guid Id { get; set; }
        public string WorkStation_Name { get; set; }
        public IPagedList<MSDS_Worker> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string KeyWord { get; set; }
        //public IList<SelectListItem> WorkStations { get; set; }
        //public string WorkStation_Id { get; set; }
    }

    public class WorkerViewModel
    {
        public Guid Id { get; set; }
        public string Worker_Name { get; set; }
        public string Worker_ID { get; set; }
        public IList<SelectListItem> WorkShops { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> WorkStations { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> WorkStations_Seleted { get; set; } = new List<SelectListItem>();
        public string[] WorkStations_Seleted_Value { get; set; }
    }
}