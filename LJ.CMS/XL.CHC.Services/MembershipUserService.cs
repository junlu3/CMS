using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
namespace XL.CHC.Services
{
   public class MembershipUserService:IMembershipUserService
   {
       private readonly IMembershipUserRepository _membershipUserRepository;
       public MembershipUserService(IMembershipUserRepository membershipUserRepository)
       {
           _membershipUserRepository = membershipUserRepository;
       }
   }
}
