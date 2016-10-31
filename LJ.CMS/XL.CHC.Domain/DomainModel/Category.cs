using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public class Category
    {
       
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public virtual CategoryType CategoryType { get; set; }

        //public virtual IList<Company> CompaniesForType { get; set; }
        //public virtual IList<Company> CompaniesForRegisterType { get; set; }

        public virtual IList<MembershipUser> EmailTaskTypeMembershipUsers { get; set; } = new List<MembershipUser>();
    }

}
