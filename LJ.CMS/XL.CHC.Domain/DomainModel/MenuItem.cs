using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XL.CHC.Domain.DomainModel
{
    public partial class MenuItem:Entity
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public int? MenuOrder { get; set; }
        public int ParentId { get; set; }
        public string Controllor { get; set; }
        public string MenuText { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
        public virtual IList<MembershipRole> MembershipRoles { get; set; }
    }
}
