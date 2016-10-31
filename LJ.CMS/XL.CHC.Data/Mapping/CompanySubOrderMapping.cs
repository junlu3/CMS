using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class CompanySubOrderMapping : EntityTypeConfiguration<CompanySubOrder>
    {
        public CompanySubOrderMapping()
        {
            HasKey(x => x.Id);
            HasKey(x => x.Series);
            HasOptional(x=>x.BookStatus)
                .WithMany()
                .Map(x => x.MapKey("BookStatus"));
            HasOptional(x => x.ParentOrder)
                .WithMany(x => x.SubOrders)
                .Map(x => x.MapKey("CompanyOrder_Id"));
                //.WillCascadeOnDelete(true);
               

        }
    }
}
