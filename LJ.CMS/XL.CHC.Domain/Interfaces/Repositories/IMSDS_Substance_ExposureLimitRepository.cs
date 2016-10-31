using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface IMSDS_Substance_ExposureLimitRepository
    {
        void Add(MSDS_Substance_ExposureLimit entity);
        void Add(IList<MSDS_Substance_ExposureLimit> entities);
        MSDS_Substance_ExposureLimit Single(string CASCode);
        IList<MSDS_Substance_ExposureLimit> GetAll();
        void DeleteAll(IList<MSDS_Substance_ExposureLimit> entities);
        IPagedList<MSDS_Substance_ExposureLimit> Search(Substance_ExposureLimitSearchModel searchModel);
    }
}
