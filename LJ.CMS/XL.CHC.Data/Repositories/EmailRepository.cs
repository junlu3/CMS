using System;
using System.Collections.Generic;
using System.Linq;
using XL.CHC.Data.Context;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
namespace XL.CHC.Data.Repositories
{
   public class EmailRepository:IEmailRepository
   {
       private readonly CHCContext _context;
       public EmailRepository(ICHCContext context)
       {
            _context = context as CHCContext;
       }

        public void Add(Email item)
        {
            _context.Email.Add(item);
        }
    }
}
