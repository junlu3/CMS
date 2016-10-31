using System.Data.Entity.ModelConfiguration;

using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class CompanyOrderMapping : EntityTypeConfiguration<CompanyOrder>
    {
        public CompanyOrderMapping()
        {
            HasKey(x => x.Id);

            //HasMany(x => x.SubOrders)
            //    .WithRequired(x => x.ParentOrder)
            //    .Map(x => x.MapKey("CompanyOrder_Id"));

            HasMany(x => x.CompanyEmployees)
                .WithMany(x => x.CompanyOrders)
                .Map(m =>
                {
                    m.ToTable("CompanyOrder_Employee_Mapping");
                    m.MapLeftKey("CompanyOrder_Id");
                    m.MapRightKey("CompanyEmployee_Id");
                });
        }
    }
}
