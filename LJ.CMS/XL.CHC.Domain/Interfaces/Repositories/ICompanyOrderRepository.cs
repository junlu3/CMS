using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface ICompanyOrderRepository
    {
        IPagedList<CompanyOrder> Search(CompanyOrderSearchModel searchModel);
        CompanyOrder GetById(Guid orderId);
        void Add(CompanyOrder order);
        void Delete(CompanyOrder entity);
    }
}
