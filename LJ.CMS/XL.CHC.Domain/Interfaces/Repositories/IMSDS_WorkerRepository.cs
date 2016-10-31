using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{

    public interface IMSDS_WorkerRepository
    {
        void Add(MSDS_Worker entity);
        IList<MSDS_Worker> GetAll(Guid company_Id);
        void Delete(MSDS_Worker entity);
        MSDS_Worker Single(Guid id);
        IPagedList<MSDS_Worker> Search(WorkerSearchModel searchModel);
    }
}
