using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
   public interface IEmployeeWorkHistoryRepository
   {
        EmployeeWorkHistory GetByIDCard(string idCard);

        void Add(EmployeeWorkHistory entity);
   }
}
