using System;
using System.Collections.Generic;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
namespace XL.CHC.Web.Models
{
    public class PermissionViewModel
    {
        public IList<MembershipRole> MembershipRoles { get; set; } = new List<MembershipRole>();
        public IList<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }

    public class RolePermissionViewModel
    {
        public MembershipRole MembershipRole { get; set; }
        public IList<MenuItem> MenuItems { get; set; }
        public int[] CheckedMenus { get; set; }
        public Guid Id { get; set; }
    }

    public class MemberShipRoleSearchViewModel
    {
        public IPagedList<MembershipRole> ViewList { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string KeyWord { get; set; }
        public IList<SelectListItem> Companys { get; set; }
        public string Company_Id { get; set; }
        public bool isAdmin { get; set; }
    }

    public class MemberShipRoleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Company Company { get; set; }
        public IList<SelectListItem> Companys { get; set; }
        public string Company_Id { get; set; }
        public bool isAdmin { get; set; }
    }
}
