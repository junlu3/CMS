using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IHospitalCalendarService
    {
        IList<HospitalCalendar> GetCalendarData(DateTime startDate, DateTime endDate);
        void AddCalendar(HospitalCalendar item);

        void DeleteCalendar(HospitalCalendar item);
        HospitalCalendar GetById(Guid id);
    }
}
