using System;

namespace XL.CHC.Domain.DomainModel
{
    public class MembershipUser_Company_Mapping
    {
        public Guid Id { get; set; }
        public virtual MembershipUser MembershipUser { get; set; }
        public virtual Company Company { get; set; }
        public bool IsCurrent { get; set; }
    }
}
