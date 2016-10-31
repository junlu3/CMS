using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public partial class MembershipRole : Entity
    {
        public MembershipRole()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        //public string Alias { get; set; }
        public string Name { get; set; }
        public string Create_By { get; set; }
        public DateTime? Create_Date { get; set; }
        public string Update_By { get; set; }
        public DateTime? Update_Date { get; set; }
        public bool Deleted { get; set; }
        public virtual Company Company { get; set; }
        public virtual IList<MembershipUser> MembershipUsers { get; set; }
        public virtual IList<MenuItem> MenuItems { get; set; }


    }
}
