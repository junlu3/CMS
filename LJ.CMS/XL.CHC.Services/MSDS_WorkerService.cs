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
    public class MSDS_WorkerService : IMSDS_WorkerService
    {
        private readonly IMSDS_WorkerRepository _repository;

        public MSDS_WorkerService(IMSDS_WorkerRepository repository)
        {
            _repository = repository;
        }

        public void Add(MSDS_Worker entity)
        {
            _repository.Add(entity);
        }

        public void Delete(MSDS_Worker entity)
        {
            _repository.Delete(entity);
        }

        public IList<MSDS_Worker> GetAll(Guid company_Id)
        {
            return _repository.GetAll(company_Id);
        }

        public IPagedList<MSDS_Worker> Search(WorkerSearchModel searchModel)
        {
            return _repository.Search(searchModel);
        }

        public MSDS_Worker Single(Guid id)
        {
            return _repository.Single(id);
        }
    }
}
