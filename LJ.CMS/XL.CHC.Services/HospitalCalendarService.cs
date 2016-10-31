using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces.Repositories;


namespace XL.CHC.Services
{
    public class HospitalCalendarService : IHospitalCalendarService
    {
        private readonly IHospitalCalendarRepository _hospitalCalendarRepository;
        public HospitalCalendarService(IHospitalCalendarRepository hospitalCalendarRepository)
        {
            _hospitalCalendarRepository = hospitalCalendarRepository;
        }

        public void AddCalendar(HospitalCalendar item)
        {
            _hospitalCalendarRepository.AddCalendar(item);
        }

        public void DeleteCalendar(HospitalCalendar item)
        {
            _hospitalCalendarRepository.DeleteCalendar(item);
        }

        public HospitalCalendar GetById(Guid id)
        {
            return _hospitalCalendarRepository.GetById(id);
        }

        public IList<HospitalCalendar> GetCalendarData(DateTime startDate, DateTime endDate)
        {
            return _hospitalCalendarRepository.GetCalendarData(startDate, endDate);
        }
    }
}
