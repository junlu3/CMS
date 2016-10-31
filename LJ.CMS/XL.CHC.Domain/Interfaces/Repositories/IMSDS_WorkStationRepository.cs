using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_WorkStationRepository
    {
        void Add(MSDS_WorkStation entity);
        void Delete(MSDS_WorkStation entity);
        IList<MSDS_WorkStation> GetAll(Guid workshop_Id);
        MSDS_WorkStation Single(Guid id);
        IPagedList<MSDS_WorkStation> Search(WorkStationSearchModel searchModel);
    }
}
