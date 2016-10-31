using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MSDS_SpecificationMapping : EntityTypeConfiguration<MSDS_Specification>
    {
        public MSDS_SpecificationMapping()
        {
            HasKey(x=>x.Id);
            HasMany(x => x.HazardousSubstance)
                .WithMany(x => x.Specifications)
                .Map(m=> {
                    m.MapLeftKey("Sid");
                    m.MapRightKey("HSid");
                    m.ToTable("MSDS_Specification_HS_Mapping");
                });

            HasOptional(x => x.SpecificationCheck)
                .WithRequired(x=>x.Specification)
                .Map(x=>x.MapKey("Specification_Id"));

            HasMany(x => x.WorkStations)
                .WithMany(x => x.Specifications)
                .Map(m=> {
                    m.MapLeftKey("Specification_Id");
                    m.MapRightKey("WorkStation_Id");
                    m.ToTable("MSDS_Specification_WorkStation_Mapping");
                });
        }
    }
}
