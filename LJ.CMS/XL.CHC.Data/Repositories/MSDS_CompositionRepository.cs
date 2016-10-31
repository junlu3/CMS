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
    public class MSDS_CompositionRepository: IMSDS_CompositionRepository
    {
        private readonly CHCContext _context;
        public MSDS_CompositionRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_Composition entity)
        {
            if (_context.MSDS_Composition.Any(x=> x.CASCode == entity.CASCode || x.Composition_Name == entity.Composition_Name))
            {
                throw new Exception("该组分已经存在！");
            }
            _context.MSDS_Composition.Add(entity);
        }

        public IList<MSDS_Composition> GetAll()
        {
            return _context.MSDS_Composition.Select(x => x).ToList();
        }
    }
}
