using System.Data.Entity.ModelConfiguration;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MSDS_WorkShopMapping : EntityTypeConfiguration<MSDS_WorkShop>
    {
        public MSDS_WorkShopMapping()
        {

            HasKey(x=>x.Id);
            HasMany(x => x.WorkStaions)
                .WithRequired(x=>x.WorkShop)
                .Map(x=>x.MapKey("WorkShop_Id"));
        }
    }
}
