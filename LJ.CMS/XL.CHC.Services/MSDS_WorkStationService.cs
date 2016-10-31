using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class MSDS_WorkStationService : IMSDS_WorkStationService
    {
        private readonly IMSDS_WorkStationRepository _repository;
        public MSDS_WorkStationService(IMSDS_WorkStationRepository repository)
        {
            _repository = repository;
        }

        public void Add(MSDS_WorkStation entity)
        {
            _repository.Add(entity);
        }

        public void Delete(MSDS_WorkStation entity)
        {
            _repository.Delete(entity);
        }

        public IList<MSDS_WorkStation> GetAll(Guid workshop_Id)
        {
            return _repository.GetAll(workshop_Id);
        }

        public IPagedList<MSDS_WorkStation> Search(WorkStationSearchModel searchModel)
        {
            return _repository.Search(searchModel);
        }

        public MSDS_WorkStation Single(Guid id)
        {
            return _repository.Single(id);
        }
    }
}
