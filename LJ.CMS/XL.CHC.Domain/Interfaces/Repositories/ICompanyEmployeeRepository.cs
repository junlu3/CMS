using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Repositories
{
   public interface ICompanyEmployeeRepository
   {
        CompanyEmployee GetByIDCard(string idCard);

        void Add(CompanyEmployee entity);
        IList<string> GetAdverseFactors(Guid companyId);
        IPagedList<CompanyEmployee> Search(CompanyEmployeeSearchModel searchModel);
        IList<string> GetDepartments(Guid companyId);
        IList<string> GetWorkTypes(Guid? companyId);

        void DeleteByCompanyID(Guid companyID);
        CompanyEmployee GetByID(Guid id);
        CompanyEmployee GetEmployee(string idCard, Guid  companyId);
    }
}
