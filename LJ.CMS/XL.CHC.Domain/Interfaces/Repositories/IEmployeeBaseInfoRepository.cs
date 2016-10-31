using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Repositories
{
   public interface IEmployeeBaseInfoRepository
   {
        EmployeeBaseInfo GetByIDCard(string idCard);

        void Add(EmployeeBaseInfo entity);
        EmployeeBaseInfo GetById(Guid id);
    }
}
