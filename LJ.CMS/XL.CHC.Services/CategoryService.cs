using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class CategoryService:ICategoryService
   {
       private readonly ICategoryRepository _categoryRepository;
       public CategoryService(ICategoryRepository categoryRepository)
       {
           _categoryRepository = categoryRepository;
       }

        public Category GetById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public IList<Category> GetByParentName(string name)
        {
            return _categoryRepository.GetByParentName(name);
        }
    }
}
