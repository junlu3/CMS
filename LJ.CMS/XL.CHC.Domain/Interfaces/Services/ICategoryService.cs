using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
    public interface ICategoryService
    {
        IList<Category> GetByParentName(string v);
        Category GetById(int v);
    }
}
