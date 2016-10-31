using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MenuItem_MembershipRole_MappingService:IMenuItem_MembershipRole_MappingService
   {
       private readonly IMenuItem_MembershipRole_MappingRepository _menuItem_MembershipRole_MappingRepository;
       public MenuItem_MembershipRole_MappingService(IMenuItem_MembershipRole_MappingRepository menuItem_MembershipRole_MappingRepository)
       {
           _menuItem_MembershipRole_MappingRepository = menuItem_MembershipRole_MappingRepository;
       }
   }
}
