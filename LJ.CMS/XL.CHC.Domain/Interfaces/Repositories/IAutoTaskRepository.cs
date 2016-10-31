using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IAutoTaskRepository
    {
        IList<AutoTask> GetAllAutoTasks();

        IList<AutoTask> GetByStatus(Category status);

        AutoTask GetByCode(string code);
    }
}
