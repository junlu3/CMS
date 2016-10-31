using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class EmployeeWorkHistoryRepository : IEmployeeWorkHistoryRepository
    {
        private readonly CHCContext _context;

        public EmployeeWorkHistoryRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public EmployeeWorkHistory GetByIDCard(string idCard)
        {
            return _context.EmployeeWorkHistory.Where(x => x.EmployeeBaseInfo.IDCard == idCard).FirstOrDefault();
        }

        public void Add(EmployeeWorkHistory entity)
        {
            _context.EmployeeWorkHistory.Add(entity);
        }
    }
}
