using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class CategoryTypeService:ICategoryTypeService
   {
       private readonly ICategoryTypeRepository _categoryTypeRepository;
       public CategoryTypeService(ICategoryTypeRepository categoryTypeRepository)
       {
           _categoryTypeRepository = categoryTypeRepository;
       }
   }
}
