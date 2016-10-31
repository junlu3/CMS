using System;
using System.Collections.Generic;

namespace XL.CHC.Domain.DomainModel
{
    public partial class MembershipUser : Entity
    {
        public MembershipUser()
        {
            Id = Guid.NewGuid();
            Deleted = false;
            CreatedOn = DateTime.Now;
        }
        public Guid Id { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public Nullable<DateTime> LastLoginDate { get; set; }
        public string AdminComment { get; set; }
        public string Email { get; set; }
        public string LastIpAddress { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string QQ { get; set; }
        public bool IsActived { get; set; } = false;
        public string Username { get; set; }
        public string WeiXin { get; set; }
        public bool Deleted { get; set; }
        public virtual IList<MembershipRole> MembershipRoles { get; set; } = new List<MembershipRole>();
        public virtual IList<Category> EmailTaskTypes { get; set; } = new List<Category>();
        public virtual Company Company { get; set; }
   
    }

    public class MembershipUserSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
        public IList<MembershipRole> Roles { get; set; }
        public Guid Company_Id { get; set; }

    }

    public class MembershipRoleSearchModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string KeyWord { get; set; }
        public Guid Company_Id { get; set; }
    }
}
