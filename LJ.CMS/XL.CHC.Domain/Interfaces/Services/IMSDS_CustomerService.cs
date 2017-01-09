using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_CustomerService
    {
        void Add(MSDS_Customer entity);
        IList<MSDS_Customer> GetAll();
        void Delete(MSDS_Customer entity);
        MSDS_Customer Single(Guid id);
        IPagedList<MSDS_Customer> Search(CustomerSearchModel searchModel);
    }
}
