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
    public class MSDS_CustomerService : IMSDS_CustomerService
    {
        public readonly IMSDS_CustomerRepository _repository;

        public MSDS_CustomerService(IMSDS_CustomerRepository repository)
        {
            _repository = repository;
        }

        public void Add(MSDS_Customer entity)
        {
            _repository.Add(entity);
        }

        public void Delete(MSDS_Customer entity)
        {
            _repository.Delete(entity);
        }

        public IList<MSDS_Customer> GetAll()
        {
            return _repository.GetAll();
        }

        public IPagedList<MSDS_Customer> Search(CustomerSearchModel searchModel)
        {
            return _repository.Search(searchModel);
        }

        public MSDS_Customer Single(Guid id)
        {
            return _repository.Single(id);
        }
    }
}
