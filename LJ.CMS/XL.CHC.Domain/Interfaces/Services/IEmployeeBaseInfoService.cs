using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
    public interface IEmployeeBaseInfoService
    {
        EmployeeBaseInfo GetById(Guid id);

        EmployeeBaseInfo GetByIdCard(string idCard);

        void Add(EmployeeBaseInfo entity);
    }
}
