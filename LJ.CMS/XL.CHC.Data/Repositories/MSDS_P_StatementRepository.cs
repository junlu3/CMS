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
    public class MSDS_P_StatementRepository: IMSDS_P_StatementRepository
    {
        private readonly CHCContext _context;

        public MSDS_P_StatementRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public IList<MSDS_P_Statement> GetAll()
        {
            return _context.MSDS_P_Statement.Select(o => o).ToList<MSDS_P_Statement>();
        }

        public MSDS_P_Statement Single(string code)
        {
            return _context.MSDS_P_Statement.SingleOrDefault(o => o.Code == code);
        }

        public void Add(MSDS_P_Statement entity)
        {
            _context.MSDS_P_Statement.Add(entity);
        }

        public void Delete(MSDS_P_Statement entity)
        {
            _context.MSDS_P_Statement.Remove(entity);
        }

        public IList<MSDS_P_Statement> GetListByNames(string[] names)
        {
            return _context.MSDS_P_Statement.Where(o => names.Contains(o.Code)).ToList();
        }
    }
}
