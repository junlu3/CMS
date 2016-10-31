using System;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface ILawService
    {
        IPagedList<Law> Search(LawSearchModel searchModel);

        Law GetById(Guid id);

        void Add(Law entity);
    }
}
