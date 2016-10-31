using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IHospitalCalendarRepository
    {
        IList<HospitalCalendar> GetCalendarData(DateTime startDate, DateTime endDate);
        void AddCalendar(HospitalCalendar item);

        void DeleteCalendar(HospitalCalendar item);
        HospitalCalendar GetById(Guid id);
    }
}
