using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class MSDS_HazardousSubstancesService : IMSDS_HazardousSubstancesService
    {
        private readonly IMSDS_HazardousSubstancesRepository _msds_HazardousSubstancesRepository;
        public MSDS_HazardousSubstancesService(IMSDS_HazardousSubstancesRepository msds_HazardousSubstancesRepository)
        {
            _msds_HazardousSubstancesRepository = msds_HazardousSubstancesRepository;
        }
        public void Add(MSDS_HazardousSubstances entity)
        {
            _msds_HazardousSubstancesRepository.Add(entity);
        }

        public IList<MSDS_HazardousSubstances> GetAll()
        {
            return _msds_HazardousSubstancesRepository.GetAll();
        }

        public MSDS_HazardousSubstances Single(Guid id)
        {
            return _msds_HazardousSubstancesRepository.Single(id);
        }

        public void Delete(MSDS_HazardousSubstances entity)
        {
            _msds_HazardousSubstancesRepository.Delete(entity);
        }
        public void DeleteList(IList<MSDS_HazardousSubstances> entities)
        {
            _msds_HazardousSubstancesRepository.DeleteList(entities);
        }
    }
}
