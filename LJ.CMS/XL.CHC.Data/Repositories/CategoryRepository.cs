using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CHCContext _context;
        public CategoryRepository(ICHCContext context)
        {
            _context = context as CHCContext;
        }

        public Category GetById(int id)
        {
            return _context.Category.FirstOrDefault(x => x.Id == id);
        }

        public IList<Category> GetByParentName(string name)
        {
            return _context.Category.Where(x => x.CategoryType.TypeName == name)
                .OrderBy(x => x.DisplayOrder).ToList();

        }

        public Category GetByName(string name, string parentName)
        {
            return _context.Category.Where(x => x.Name == name && x.CategoryType.TypeName == parentName).FirstOrDefault();
        }
    }
}
