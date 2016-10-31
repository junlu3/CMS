using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public partial class Company
    {
        public bool Deleted { get; set; } = false;

        public Guid Id { get; set; } = Guid.NewGuid();
        public string CompanyAddress { get; set; }
        public string CompanyName { get; set; }
        public string Telephone { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; } = DateTime.Now;
        public string Fax { get; set; }
        public string LegalPerson { get; set; }
        public string RestDay { get; set; }
        public string ZipCode { get; set; }
        public Nullable<DateTime> RegisterDate { get; set; }
        public virtual Category CompanyType { get; set; }
        public virtual Category CompanyRegisterType { get; set; }
        public virtual IList<CompanyEmployee> CompanyEmployees { get; set; }
        //public virtual MembershipUser MembershipUser { get; set; }
        public virtual IList<MembershipUser> MembershipUsers { get; set; }
        public virtual IList<CompanyOrder> CompanyOrders { get; set; }

        public virtual IList<CompanySubOrder> CompanySubOrders { get; set; }
        public virtual IList<MSDS_Specification> Specifications { get; set; }
        public virtual IList<MembershipRole> MembershipRoles { get; set; }
        public virtual IList<MSDS_WorkShop> WorkShops { get; set; }
        public virtual IList<MSDS_Worker> Workers { get; set; }
        public virtual bool IsDefault { get; set; }

        public string CompanyStamp { get; set; }
    }

    public class CompanySearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
    }
}
