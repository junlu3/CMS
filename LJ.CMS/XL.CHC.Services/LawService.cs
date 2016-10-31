using System;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public class LawService : ILawService
    {
        private readonly ILawRepository _emailRepository;

        public LawService(ILawRepository emailRepository)
        {
            this._emailRepository = emailRepository;
        }

        public void Add(Law entity)
        {
            _emailRepository.Add(entity);
        }

        public Law GetById(Guid id)
        {
            return _emailRepository.GetById(id);
        }

        public IPagedList<Law> Search(LawSearchModel searchModel)
        {
            return _emailRepository.Search(searchModel);
        }
    }
}
