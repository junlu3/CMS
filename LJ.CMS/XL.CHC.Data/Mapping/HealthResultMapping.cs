using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class HealthResultMapping : EntityTypeConfiguration<HealthResult>
    {
        public HealthResultMapping()
        {
            HasKey(x => x.Id);
            HasOptional(x => x.CompanyEmployee)
               .WithMany()
               .Map(x => x.MapKey("CompanyEmployee_Id"));
        }
    }
}
