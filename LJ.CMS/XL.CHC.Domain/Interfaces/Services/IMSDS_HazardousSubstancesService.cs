using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IMSDS_HazardousSubstancesService
    {
        void Add(MSDS_HazardousSubstances entity);
        IList<MSDS_HazardousSubstances> GetAll();
        MSDS_HazardousSubstances Single(Guid id);
        void Delete(MSDS_HazardousSubstances entity);
        void DeleteList(IList<MSDS_HazardousSubstances> entities);
    }
}
