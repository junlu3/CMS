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
    public class MSDS_SpecificationCheckRepository: IMSDS_SpecificationCheckRepository
    {
        private readonly CHCContext _context;
        public MSDS_SpecificationCheckRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_SpecificationCheck entity)
        {
            _context.MSDS_SpecificationCheck.Add(entity);
        }

        public IList<MSDS_SpecificationCheck> GetAll()
        {
            return _context.MSDS_SpecificationCheck.Select(o => o).ToList();
        }

        public MSDS_SpecificationCheck Single(Guid id)
        {
            return _context.MSDS_SpecificationCheck.SingleOrDefault(o => o.Specification.Id == id);
        }

        public void Delete(MSDS_SpecificationCheck entity)
        {
            _context.MSDS_SpecificationCheck.Remove(entity);
        }
    }
}
