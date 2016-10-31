
using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class CompanyEmployeeMapping : EntityTypeConfiguration<CompanyEmployee>
    {
        public CompanyEmployeeMapping()
        {
            HasKey(x => x.Id);
            HasOptional(x => x.HealthStatus)
                .WithMany()
                .Map(x => x.MapKey("HealthStatus"));
            HasOptional(x => x.Married)
            .WithMany()
            .Map(x => x.MapKey("Married"));
            HasOptional(x => x.MigrantWorker)
               .WithMany()
               .Map(x => x.MapKey("MigrantWorker"));
            //HasRequired(x => x.HealthStatus)
            //    .WithOptional()
            //    .Map(x => x.MapKey("HealthStatus"));
            //HasRequired(x => x.Married)
            //   .WithOptional()
            //   .Map(x => x.MapKey("Married"));
            //HasRequired(x => x.MigrantWorker)
            //   .WithOptional()
            //   .Map(x => x.MapKey("MigrantWorker"));
        }
    }
}
