using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.DomainModel;
using System.Web.Mvc;

namespace XL.CHC.Web.Models
{

    public class CompanyUserSearchViewModel
    {
        public IPagedList<MembershipUser> Users { get; set; } = new PagedList<MembershipUser>(new List<MembershipUser>(), 1, int.MaxValue, 0);
        public string KeyWord { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
    }

    public class CompanyUserCreateViewModel
    {
        public Guid? Id { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "名称长度不少于4位，不大于20位")]
        [MaxLength(20, ErrorMessage = "名称长度不少于4位，不大于20位")]
        public string AccountName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } = "123456";
        public IList<SelectListItem> MembershipRoles { get; set; }
        public string Role_Id { get; set; }
    }
}