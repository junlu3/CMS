using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
   public interface IEmailService
   {
        void Add(Email item);
   }
}
