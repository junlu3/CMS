using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MenuItemService:IMenuItemService
   {
       private readonly IMenuItemRepository _menuItemRepository;
       public MenuItemService(IMenuItemRepository menuItemRepository)
       {
           _menuItemRepository = menuItemRepository;
       }

        public IList<MenuItem> GetAll()
        {
            return _menuItemRepository.GetAll();
        }

    }
}
