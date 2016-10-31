using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface ICompanySubOrderRepository
    {
        IList<CompanySubOrder> GetSubOrders(DateTime startDate, DateTime endDate);
        IPagedList<CompanySubOrder> Search(CompanySubOrderSearchModel companySubOrderSearchModel);
        IList<CompanySubOrder> GetByDate(DateTime startDate, DateTime endDate);
        void Add(CompanySubOrder order);
        CompanySubOrder GetById(Guid id);
        void Delete(CompanySubOrder entity);
    }
}
