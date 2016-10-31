using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_Substance_ExposureLimitService
    {
        void Add(MSDS_Substance_ExposureLimit entity);
        void Add(IList<MSDS_Substance_ExposureLimit> entities);
        MSDS_Substance_ExposureLimit Single(string CASCode);
        IList<MSDS_Substance_ExposureLimit> GetAll();
        void DeleteAll(IList<MSDS_Substance_ExposureLimit> entities);
        IPagedList<MSDS_Substance_ExposureLimit> Search(Substance_ExposureLimitSearchModel searchModel);
    }
}
