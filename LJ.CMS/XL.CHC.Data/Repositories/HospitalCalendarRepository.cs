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
    public class HospitalCalendarRepository:IHospitalCalendarRepository
    {
        private readonly CHCContext _context;
        public HospitalCalendarRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public void AddCalendar(HospitalCalendar item)
        {
            _context.HospitoalCalendar.Add(item);
        }

        public void DeleteCalendar(HospitalCalendar item)
        {
            throw new NotImplementedException();
        }

        public HospitalCalendar GetById(Guid id)
        {
            return _context.HospitoalCalendar.Where(x => x.Id == id).FirstOrDefault();
        }

        public IList<HospitalCalendar> GetCalendarData(DateTime startDate, DateTime endDate)
        {
            return _context.HospitoalCalendar
               .Where(x => x.StartDate >= startDate
               && x.EndDate <= endDate)
               .OrderBy(x=>x.StartDate)
               .ToList();
        }
    }
}
