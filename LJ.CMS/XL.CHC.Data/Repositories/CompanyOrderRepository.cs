using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
    public class CompanyOrderRepository : ICompanyOrderRepository
    {
        private readonly CHCContext _context;
        public CompanyOrderRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(CompanyOrder order)
        {
            if (order != null)
            {
                _context.CompanyOrder.Add(order);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public CompanyOrder GetById(Guid orderId)
        {
            return _context.CompanyOrder.FirstOrDefault(x => x.Id == orderId);
        }

        public IPagedList<CompanyOrder> Search(CompanyOrderSearchModel searchModel)
        {
            var query = _context.CompanyOrder
                    .Where(x => (searchModel.CompanyId == null || searchModel.CompanyId == x.Company.Id)
                                && (searchModel.CreatedStartTime == null || x.CreatedDate >= searchModel.CreatedStartTime)
                                && (searchModel.CreatedEndTime == null || x.CreatedDate <= searchModel.CreatedEndTime)
                                && (searchModel.MinBookTime == null || x.SubOrders.Where(s => s.StartDate >= searchModel.MinBookTime).Count() > 0)
                                && (searchModel.MaxBookTime == null || x.SubOrders.Where(s => s.EndDate <= searchModel.MaxBookTime).Count() > 0)
                                )
                    .OrderByDescending(x => x.CreatedDate);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<CompanyOrder>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

        public void Delete(CompanyOrder entity)
        {
            _context.CompanyOrder.Remove(entity);
        }
    }
}
