using System.IO;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
   public interface IEmployeeWorkHistoryService
   {
        ImportResultModel ImportEmployeeWorkHistories(Stream stream);
        void Add(EmployeeWorkHistory workHistory);
    }
}
