using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_CustomerRepository
    {
        void Add(MSDS_Customer entity);
        IList<MSDS_Customer> GetAll();
        void Delete(MSDS_Customer entity);
        MSDS_Customer Single(Guid id);
        IPagedList<MSDS_Customer> Search(CustomerSearchModel searchModel);
    }
}
