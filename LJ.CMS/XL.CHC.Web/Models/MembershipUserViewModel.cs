using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Models
{
    public class MembershipUserLogonViewModel
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = true;

        public string ReturnUrl { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class MembershipUserManageViewModel
    {
        public string KeyWord { get; set; }
        public int PageIndex { get; set; } = 1;
        public IPagedList<MembershipUser> MembershipUsers { get; set; } = new PagedList<MembershipUser>(new List<MembershipUser>(), 1, int.MaxValue, int.MaxValue);

    }

    public class MembershipUserEditViewModel
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "请输入用户名")]
        [MinLength(4, ErrorMessage = "名称长度不少于4位，不大于20位")]
        [MaxLength(20, ErrorMessage = "名称长度不少于4位，不大于20位")]
        public string UserName { get; set; }
        public string Password { get; set; }
        //[Required(ErrorMessage = "请输入电子邮箱")]
        [EmailAddress(ErrorMessage = "请输入正确格式的邮箱地址")]
        public string Email { get; set; }
        public List<string> SelectedMembershipRoleIds { get; set; } = new List<string>();

        public IList<MembershipRole> AvailableRoles { get; set; } = new List<MembershipRole>();
        public List<SelectListItem> Companys { get; set; }
        public string Company_Id { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public string Role_Id { get; set; }
    }

    public class MembershipUserPersonalInfoViewModel
    {
        public Guid Id { get; set; } = new Guid();

        public Guid? DefaultCompanyId { get; set; }

        [EmailAddress(ErrorMessage = "请输入正确格式的邮箱地址")]
        public string Email { get; set; } = string.Empty;

        public IList<int> SelectedEmailTaskTypeIds { get; set; } = new List<int>();

        public IList<Company> AvailableCompanies { get; set; } = new List<Company>();

        public IList<Category> AvailableEmailTaskTypes { get; set; } = new List<Category>();
    }

    public class ActiveEnterpriseUserViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="请输入公司名称")]
        public string CompanyName { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string RestDay { get; set; } = "双休日";
        public string LegalPerson { get; set; }
        public Nullable<DateTime> RegisterDate { get; set; }
        public int CompanyType { get; set; }
        public int CompanyRegisterType { get; set; }

        public string StampFilePath { get; set; }

        public IList<SelectListItem> CompanyTypeList { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> CompanyRegisterTypeList { get; set; } = new List<SelectListItem>();
    }

    public class CompanyViewModel
    {
        public Guid MembershipUserId { get; set; }
        public Guid Id { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyName { get; set; }
        public string Telephone { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string Fax { get; set; }
        public string LegalPerson { get; set; }
        public string RestDay { get; set; }
        public string ZipCode { get; set; }
        public DateTime? RegisterDate { get; set; }
        public int CompanyTypeId { get; set; }
        public int CompanyRegisterTypeId { get; set; }
        public string CompanyStamp { get; set; }
        public List<SelectListItem> AvailableCompanyTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailableCompanyRegisterTypes { get; set; } = new List<SelectListItem>();
    }

    public class EditPasswordViewModel
    {
        public string Username { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string RepeatNewPassword { get; set; }
    }
}
