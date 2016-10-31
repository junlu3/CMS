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
    public class MSDS_WorkShopService:IMSDS_WorkShopService
    {
        private readonly IMSDS_WorkShopRepository _repository;
        public MSDS_WorkShopService(IMSDS_WorkShopRepository repository)
        {
            _repository = repository;
        }

        public void Add(MSDS_WorkShop entity)
        {
            _repository.Add(entity);
        }

        public void Delete(MSDS_WorkShop entity)
        {
            _repository.Delete(entity);
        }

        public IList<MSDS_WorkShop> GetAll(Guid company_Id)
        {
            return _repository.GetAll(company_Id);
        }

        public MSDS_WorkShop Single(Guid id)
        {
            return _repository.Single(id);
        }

        public IPagedList<MSDS_WorkShop> Search(WorkShopSearchModel searchModel)
        {
            return _repository.Search(searchModel);
        }
    }
}
