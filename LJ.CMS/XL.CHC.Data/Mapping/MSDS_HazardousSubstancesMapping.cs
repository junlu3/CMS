using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MSDS_HazardousSubstancesMapping : EntityTypeConfiguration<MSDS_HazardousSubstances>
    {
        public MSDS_HazardousSubstancesMapping()
        {
            HasKey(x=>x.HS_Id);
            HasMany(x => x.HS_HStatements)
                .WithMany(x => x.HazardousSubstances)
                .Map(m=> {
                    m.MapLeftKey("HS_Id");
                    m.MapRightKey("HS_H_Statement_Id");
                    m.ToTable("MSDS_HS_Statement_Mapping");
                });
        }
    }
}
