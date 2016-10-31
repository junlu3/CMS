using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MembershipRoleMapping : EntityTypeConfiguration<MembershipRole>
    {
        public MembershipRoleMapping()
        {
            HasKey(x => x.Id);
            HasMany(x => x.MenuItems)
                .WithMany(m => m.MembershipRoles)
                .Map(m =>
                {
                    m.ToTable("MenuItem_MembershipRole_Mapping");
                    m.MapLeftKey("MembershipRole_Id");
                    m.MapRightKey("MenuItem_Id");
                });

        }
    }
}
