using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class MSDS_WorkShopRepository:IMSDS_WorkShopRepository
    {
        private readonly CHCContext _context;
        public MSDS_WorkShopRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_WorkShop entity)
        {
            if (_context.MSDS_WorkShop.Any(x=>x.WorkShop_Name == entity.WorkShop_Name))
            {
                throw new Exception("车间名称已经存在");
            }
            _context.MSDS_WorkShop.Add(entity);
        }

        public void Delete(MSDS_WorkShop entity)
        {
            _context.MSDS_WorkShop.Remove(entity);
        }

        public IList<MSDS_WorkShop> GetAll(Guid company_Id)
        {
            return _context.MSDS_WorkShop.Where(x=>x.Company.Id == company_Id).ToList<MSDS_WorkShop>();
        }

        public MSDS_WorkShop Single(Guid id)
        {
            return _context.MSDS_WorkShop.SingleOrDefault<MSDS_WorkShop>(x=>x.Id == id);
        }

        public IPagedList<MSDS_WorkShop> Search(WorkShopSearchModel searchModel)
        {
            var query = _context.MSDS_WorkShop.Where(x=> string.IsNullOrEmpty(searchModel.KeyWord) 
            || x.WorkShop_Name.ToLower().Contains(searchModel.KeyWord.ToLower()))
            .OrderBy(x=>x.WorkShop_Name);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<MSDS_WorkShop>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }
    }
}
