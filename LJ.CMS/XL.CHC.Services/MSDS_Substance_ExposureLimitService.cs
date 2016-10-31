using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
using System.Linq;
namespace XL.CHC.Services
{
    public class MSDS_Substance_ExposureLimitService : IMSDS_Substance_ExposureLimitService
    {
        public readonly IMSDS_Substance_ExposureLimitRepository _repository;

        public MSDS_Substance_ExposureLimitService(IMSDS_Substance_ExposureLimitRepository repository)
        {
            _repository = repository;
        }
        public void Add(MSDS_Substance_ExposureLimit entity)
        {
            _repository.Add(entity);
        }
        public void Add(IList<MSDS_Substance_ExposureLimit> entities)
        {
            _repository.Add(entities);
        }

        public void DeleteAll(IList<MSDS_Substance_ExposureLimit> entities)
        {
            _repository.DeleteAll(entities);
        }

        public IList<MSDS_Substance_ExposureLimit> GetAll()
        {
            return _repository.GetAll();
        }

        public IPagedList<MSDS_Substance_ExposureLimit> Search(Substance_ExposureLimitSearchModel searchModel)
        {
            return _repository.Search(searchModel);
        }

        public MSDS_Substance_ExposureLimit Single(string CASCode)
        {
            return _repository.Single(CASCode);
        }


    }
}
