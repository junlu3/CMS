using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class AutoTaskService : IAutoTaskService
    {
        private readonly IAutoTaskRepository _autoTaskRepository;

        public AutoTaskService(IAutoTaskRepository autoTaskRepository)
        {
            this._autoTaskRepository = autoTaskRepository;
        }

        public IList<AutoTask> GetAllAutoTasks()
        {
            return _autoTaskRepository.GetAllAutoTasks();
        }

        public AutoTask GetByCode(string code)
        {
            return _autoTaskRepository.GetByCode(code);
        }

        public IList<AutoTask> GetByStatus(Category status)
        {
            return _autoTaskRepository.GetByStatus(status);
        }
    }
}
