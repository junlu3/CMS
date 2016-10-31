using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class CategoryMapping: EntityTypeConfiguration<Category>
    {
        public CategoryMapping()
        {
            HasKey(x => x.Id);
            //HasMany(x => x.CompaniesForType)
            //     .WithOptional(x => x.CompanyType)
            //     .Map(x=>x.MapKey("CompanyType"));
           
            //HasMany(x => x.CompaniesForRegisterType)
            //    .WithOptional(x => x.CompanyRegisterType)
                //.Map(x=>x.MapKey("CompanyRegisterType"));
        }
    }
}
