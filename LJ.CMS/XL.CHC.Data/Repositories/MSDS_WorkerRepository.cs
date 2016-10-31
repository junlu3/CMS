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
    public class MSDS_WorkerRepository:IMSDS_WorkerRepository
    {
        private readonly CHCContext _context;
        public MSDS_WorkerRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_Worker entity)
        {
            if (_context.MSDS_Worker.Any(x=>x.Worker_ID == entity.Worker_ID && x.Company.Id == entity.Company.Id))
            {
                throw new Exception("该员工已存在！");
            }
            _context.MSDS_Worker.Add(entity);
        }

        public IList<MSDS_Worker> GetAll(Guid company_Id)
        {
            return _context.MSDS_Worker.Where(x => x.Company.Id == company_Id).ToList();
        }

        public void Delete(MSDS_Worker entity)
        {
            _context.MSDS_Worker.Remove(entity);
        }

        public MSDS_Worker Single(Guid id)
        {
            return _context.MSDS_Worker.SingleOrDefault(x => x.Id == id);
        }

        public IPagedList<MSDS_Worker> Search(WorkerSearchModel searchModel)
        {
            var query = _context.MSDS_Worker.Where(x => string.IsNullOrEmpty(searchModel.KeyWord)
            || x.Worker_Name.ToLower().Contains(searchModel.KeyWord.ToLower())
            || x.Worker_ID.Contains(searchModel.KeyWord))
            .OrderBy(x => x.Worker_Name);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<MSDS_Worker>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

    }
}
