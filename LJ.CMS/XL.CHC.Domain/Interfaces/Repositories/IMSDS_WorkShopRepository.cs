using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_WorkShopRepository
    {
        void Add(MSDS_WorkShop entity);
        void Delete(MSDS_WorkShop entity);
        IList<MSDS_WorkShop> GetAll(Guid company_Id);
        MSDS_WorkShop Single(Guid id);
        IPagedList<MSDS_WorkShop> Search(WorkShopSearchModel searchModel);
    }
}
