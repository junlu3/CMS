using System;
using System.ComponentModel.DataAnnotations;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class LawListViewModel
    {
        public IPagedList<Law> LawList { get; set; }

        public string KeyWord { get; set; }

        public int PageIndex { get; set; } = 1;
    }

    public class LawViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "名称不能为空")]
        public string Name { get; set; }

        public string DocumentNumber { get; set; }

        public DateTime? ImplementationDate { get; set; }

        public string FilePath { get; set; }
    }
}