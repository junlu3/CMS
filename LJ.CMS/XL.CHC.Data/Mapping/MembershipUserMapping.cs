using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MembershipUserMapping : EntityTypeConfiguration<MembershipUser>
    {
        public MembershipUserMapping()
        {
            HasKey(x => x.Id);
            HasMany(x => x.MembershipRoles)
                .WithMany(t => t.MembershipUsers)
                .Map(m=>
                {
                    m.ToTable("MembershipUser_MembershipRole_Mapping");
                    m.MapLeftKey("MembershipUser_Id");
                    m.MapRightKey("MembershipRole_Id");
                });

            HasMany(x => x.EmailTaskTypes)
                .WithMany(x => x.EmailTaskTypeMembershipUsers)
                .Map(m =>
                {
                    m.ToTable("MembershipUser_EmailTaskType_Mapping");
                    m.MapLeftKey("MembershipUser_Id");
                    m.MapRightKey("EmailTaskType_Id");
                });
        }
    }
}
