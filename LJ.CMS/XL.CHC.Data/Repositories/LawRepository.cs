using System;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class LawRepository : ILawRepository
    {
        private readonly CHCContext _context;

        public LawRepository(ICHCContext context)
        {
            this._context = context as CHCContext;
        }

        public void Add(Law entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.Laws.Add(entity);
        }

        public IPagedList<Law> Search(LawSearchModel searchModel)
        {
            var query = _context.Laws.Where(x => (x.Deleted == false)
                  && (string.IsNullOrEmpty(searchModel.KeyWord) || x.Name.ToLower().Contains(searchModel.KeyWord)))
                      .OrderBy(x => x.Name);
            var count = query.Count();
            var result = query.Skip((searchModel.PageIndex - 1) * searchModel.PageSize).Take(searchModel.PageSize).ToList();
            return new PagedList<Law>(result, searchModel.PageIndex, searchModel.PageSize, count);
        }

        public Law GetById(Guid id)
        {
            return _context.Laws.FirstOrDefault(e => e.Deleted == false && e.Id == id);
        }
    }
}
