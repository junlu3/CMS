using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
    public class CompanySubOrderRepository : ICompanySubOrderRepository
    {
        private readonly CHCContext _context;
        public CompanySubOrderRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(CompanySubOrder order)
        {
            _context.CompanySubOrder.Add(order);
        }

        public IList<CompanySubOrder> GetByDate(DateTime startDate, DateTime endDate)
        {
            return _context.CompanySubOrder.Where(x => x.StartDate >= startDate && x.EndDate <= endDate)
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }

        public CompanySubOrder GetById(Guid id)
        {
            return _context.CompanySubOrder.FirstOrDefault(x => x.Id == id);
        }

        public IList<CompanySubOrder> GetSubOrders(DateTime startDate, DateTime endDate)
        {
            return _context.CompanySubOrder.Where
               (x => x.BookStatus.Name != "拒绝" && x.StartDate <= startDate && x.EndDate >= endDate)

               .ToList();
        }

        public IPagedList<CompanySubOrder> Search(CompanySubOrderSearchModel companySubOrderSearchModel)
        {
            var query = _context.CompanySubOrder
                .Where(x => (companySubOrderSearchModel.CompanyId == null || x.Company.Id == companySubOrderSearchModel.CompanyId)
                   && (companySubOrderSearchModel.SelectedBookStatus == null || x.BookStatus.Id == companySubOrderSearchModel.SelectedBookStatus)
                  && (companySubOrderSearchModel.MaxSearchTime == null || x.EndDate <= companySubOrderSearchModel.MaxSearchTime)
                  && (companySubOrderSearchModel.MinSearchTime == null || x.StartDate >= companySubOrderSearchModel.MinSearchTime)
                ).OrderByDescending(x => x.StartDate)
                .ThenByDescending(x => x.UpdatedDate);
            var count = query.Count();
            var result = query.Skip((companySubOrderSearchModel.PageIndex - 1) * companySubOrderSearchModel.PageSize).Take(companySubOrderSearchModel.PageSize).ToList();
            return new PagedList<CompanySubOrder>(result, companySubOrderSearchModel.PageIndex, companySubOrderSearchModel.PageSize, count);
        }

        public void Delete(CompanySubOrder entity)
        {
            _context.CompanySubOrder.Remove(entity);
        }
    }
}
