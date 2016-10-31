 
using System.Data.Entity.ModelConfiguration;
 
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Data.Mapping
{
    public class MenuItemMapping : EntityTypeConfiguration<MenuItem>
    {
        public MenuItemMapping()
        {
            HasKey(x => x.Id);
        }
    }
}
