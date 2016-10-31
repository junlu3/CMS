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
    public class MSDS_HazardousSubstancesRepository: IMSDS_HazardousSubstancesRepository
    {
        private readonly CHCContext _context;

        public MSDS_HazardousSubstancesRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void Add(MSDS_HazardousSubstances entity)
        {
            _context.MSDS_HazardousSubstances.Add(entity);
        }

        public IList<MSDS_HazardousSubstances> GetAll()
        {
            return _context.MSDS_HazardousSubstances.Select(o=>o).ToList();
        }

        public MSDS_HazardousSubstances Single(Guid id)
        {
            return _context.MSDS_HazardousSubstances.SingleOrDefault(o=>o.HS_Id == id);
        }

        public void Delete(MSDS_HazardousSubstances entity)
        {
            _context.MSDS_HazardousSubstances.Remove(entity);
        }

        public void DeleteList(IList<MSDS_HazardousSubstances> entities)
        {
            _context.MSDS_HazardousSubstances.RemoveRange(entities);
        }
    }
}
