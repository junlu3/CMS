
using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MembershipUser_Company_Mapping_Mapping : EntityTypeConfiguration<MembershipUser_Company_Mapping>
    {
        public MembershipUser_Company_Mapping_Mapping()
        {
            HasKey(x => x.Id);
            
           
        }
    }
}
