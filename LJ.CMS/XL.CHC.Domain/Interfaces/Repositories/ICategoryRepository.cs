using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        IList<Category> GetByParentName(string name);

        Category GetById(int id);

        Category GetByName(string name, string parentName);
    }
}
