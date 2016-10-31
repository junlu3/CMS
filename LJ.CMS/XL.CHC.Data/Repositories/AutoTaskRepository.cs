using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;

namespace XL.CHC.Data.Repositories
{
    public class AutoTaskRepository : IAutoTaskRepository
    {
        private readonly CHCContext _context;

        public AutoTaskRepository(ICHCContext context)
        {
            this._context = context as CHCContext;
        }

        public IList<AutoTask> GetAllAutoTasks()
        {
            return _context.AutoTask.Where(e => true).ToList();
        }

        public IList<AutoTask> GetByStatus(Category status)
        {
            return _context.AutoTask.Where(e => e.AutoTaskStatus == status).ToList();
        }

        public AutoTask GetByCode(string code)
        {
            return _context.AutoTask.Where(e => e.Code == code).FirstOrDefault();
        }
    }
}
