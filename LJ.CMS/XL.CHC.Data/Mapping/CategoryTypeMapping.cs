using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class CategoryTypeMapping : EntityTypeConfiguration<CategoryType>
    {
        public CategoryTypeMapping()
        {
            HasKey(x => x.Id);
            HasMany(x => x.Categories).WithRequired(x => x.CategoryType)
                .Map(x => x.MapKey("CategoryType_Id"))
                .WillCascadeOnDelete();
        }

    }
}
