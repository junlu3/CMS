using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IAutoTaskService
    {
        IList<AutoTask> GetAllAutoTasks();

        IList<AutoTask> GetByStatus(Category status);

        AutoTask GetByCode(string code);
    }
}
