using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
   public class MenuItemRepository:IMenuItemRepository
   {
       private readonly CHCContext _context;
       public MenuItemRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }

        public IList<MenuItem> GetAll()
        {
            return _context.MenuItem.OrderBy(x => x.MenuOrder).ToList() ;
        }


    }
}
