using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class MSDS_H_StatementRepository: IMSDS_H_StatementRepository
    {
        private readonly CHCContext _context;

        public MSDS_H_StatementRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public IList<MSDS_H_Statement> GetAll()
        {
            return _context.MSDS_H_Statement.Select(o=>o).ToList<MSDS_H_Statement>();
        }

        public MSDS_H_Statement Single(string code)
        {
            return _context.MSDS_H_Statement.SingleOrDefault(o => o.Code == code);
        }

        public void Add(MSDS_H_Statement entity)
        {
            _context.MSDS_H_Statement.Add(entity);
        }

        public void Delete(MSDS_H_Statement entity)
        {
            _context.MSDS_H_Statement.Remove(entity);
        }

        public IList<MSDS_H_Statement> GetListByNames(string[] names)
        {
            return _context.MSDS_H_Statement.Where(o => names.Contains(o.Code)).ToList();
        }
    }
}
