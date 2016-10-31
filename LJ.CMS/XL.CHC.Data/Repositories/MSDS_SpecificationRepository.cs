using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using System.Linq.Dynamic;

namespace XL.CHC.Data.Repositories
{
    public class MSDS_SpecificationRepository : IMSDS_SpecificationRepository
    {
        private readonly CHCContext _context;

        public MSDS_SpecificationRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_Specification entity)
        {
            _context.MSDS_Specification.Add(entity);
        }

        public IList<MSDS_Specification> GetAll()
        {
            return _context.MSDS_Specification.Select(o=>o).ToList();
        }

        public MSDS_Specification Single(Guid id,Guid company_Id)
        {
            return _context.MSDS_Specification.SingleOrDefault(o => o.Id == id && o.Company.Id == company_Id);
        }
        public MSDS_Specification Single(string product_Name,Guid company_Id,string supplier_Name)
        {
            return _context.MSDS_Specification.SingleOrDefault(o=> o.Product_Name == product_Name && o.Company.Id == company_Id && o.Supplier_Name == supplier_Name);
        }
        public MSDS_Specification Single(Guid id)
        {
            return _context.MSDS_Specification.SingleOrDefault(o => o.Id == id);
        }
        public void Delete(MSDS_Specification entity)
        {
            _context.MSDS_Specification.Remove(entity);
        }

        public IPagedList<MSDS_Specification> Search(SpecificationSearchModel searchModel)
        {

            if (searchModel.CheckStatus == 0)
            {
                var query = _context.MSDS_Specification
                            .Where(o => (searchModel.STime == null || o.Create_Date.Value >= searchModel.STime.Value)
                            && (searchModel.ETime == null || o.Create_Date.Value <= searchModel.ETime.Value)
                            && (searchModel.Product_Name == null || o.Product_Name.Contains(searchModel.Product_Name))
                            && (searchModel.CN_Name == null || o.CN_Name.Contains(searchModel.CN_Name))
                            && (searchModel.Supplier_Name == null || o.Supplier_Name.Contains(searchModel.Supplier_Name))
                            && (!searchModel.Product_UN.HasValue || o.Product_UN == searchModel.Product_UN)
                            && (searchModel.CASCode == null || o.CASCode.Contains(searchModel.CASCode))
                            && (searchModel.HS_CASCode == null || o.HazardousSubstance.Any(k => k.HS_CASCode == searchModel.HS_CASCode))
                            && (searchModel.Company_Id == null || o.Company.Id == searchModel.Company_Id)
                            && o.SpecificationCheck == null
                            ).Union(_context.MSDS_Specification.Where(o=> (searchModel.STime == null || o.Create_Date.Value >= searchModel.STime.Value)
                            && (searchModel.ETime == null || o.Create_Date.Value <= searchModel.ETime.Value)
                            && (searchModel.Product_Name == null || o.Product_Name.Contains(searchModel.Product_Name))
                            && (searchModel.CN_Name == null || o.CN_Name.Contains(searchModel.CN_Name))
                            && (searchModel.Supplier_Name == null || o.Supplier_Name.Contains(searchModel.Supplier_Name))
                            && (!searchModel.Product_UN.HasValue || o.Product_UN == searchModel.Product_UN)
                            && (searchModel.CASCode == null || o.CASCode.Contains(searchModel.CASCode))
                            && (searchModel.HS_CASCode == null || o.HazardousSubstance.Any(k => k.HS_CASCode == searchModel.HS_CASCode))
                            && (searchModel.Company_Id == null || o.Company.Id == searchModel.Company_Id)
                            && (o.SpecificationCheck.Status == 3)
                            ));

                int count = query.Count();
                string strWhere = searchModel.SortCol + " " + searchModel.SortType;
                IList<MSDS_Specification> result = query.AsQueryable().OrderBy(strWhere).Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MSDS_Specification>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }
            else if (searchModel.CheckStatus == 1 || searchModel.CheckStatus == 2)
            {
                var query = _context.MSDS_Specification
                            .Where(o => (searchModel.STime == null || o.Create_Date.Value >= searchModel.STime.Value)
                            && (searchModel.ETime == null || o.Create_Date.Value <= searchModel.ETime.Value)
                            && (searchModel.Product_Name == null || o.Product_Name.Contains(searchModel.Product_Name))
                            && (searchModel.CN_Name == null || o.CN_Name.Contains(searchModel.CN_Name))
                            && (searchModel.Supplier_Name == null || o.Supplier_Name.Contains(searchModel.Supplier_Name))
                            && (!searchModel.Product_UN.HasValue || o.Product_UN == searchModel.Product_UN)
                            && (searchModel.CASCode == null || o.CASCode.Contains(searchModel.CASCode))
                            && (searchModel.HS_CASCode == null || o.HazardousSubstance.Any(k => k.HS_CASCode == searchModel.HS_CASCode))
                            && (searchModel.Company_Id == null || o.Company.Id == searchModel.Company_Id)
                            && o.SpecificationCheck.Status == searchModel.CheckStatus
                            );

                int count = query.Count();
                string strWhere = searchModel.SortCol + " " + searchModel.SortType;
                IList<MSDS_Specification> result = query.AsQueryable().OrderBy(strWhere).Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MSDS_Specification>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }
            else
            {
                var query = _context.MSDS_Specification
                                .Where(o => (searchModel.STime == null || o.Create_Date.Value >= searchModel.STime.Value)
                                && (searchModel.ETime == null || o.Create_Date.Value <= searchModel.ETime.Value)
                                && (searchModel.Product_Name == null || o.Product_Name.Contains(searchModel.Product_Name))
                                && (searchModel.CN_Name == null || o.CN_Name.Contains(searchModel.CN_Name))
                                && (searchModel.Supplier_Name == null || o.Supplier_Name.Contains(searchModel.Supplier_Name))
                                && (!searchModel.Product_UN.HasValue || o.Product_UN == searchModel.Product_UN)
                                && (searchModel.CASCode == null || o.CASCode.Contains(searchModel.CASCode))
                                && (searchModel.HS_CASCode == null || o.HazardousSubstance.Any(k => k.HS_CASCode == searchModel.HS_CASCode))
                                && (searchModel.Company_Id == null || o.Company.Id == searchModel.Company_Id)
                                );

                int count = query.Count();
                string strWhere = searchModel.SortCol + " " + searchModel.SortType;
                IList<MSDS_Specification> result = query.AsQueryable().OrderBy(strWhere).Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
                return new PagedList<MSDS_Specification>(result, searchModel.PageIndex, searchModel.PageSize, count);
            }
        }

    }
}
