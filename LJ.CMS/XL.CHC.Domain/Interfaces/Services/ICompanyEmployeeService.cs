using System;
using System.Collections.Generic;
using System.IO;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Domain.Interfaces.Services
{
    public interface ICompanyEmployeeService
    {
        ImportResultModel ImportCompanyEmployees(Stream stream, bool isCoverData);

        void ExportCompanyEmployees(string templateFilePath, List<ImportResultModel> data);
        void Test();
        IList<string> GetAdverseFactors();
        IPagedList<CompanyEmployee> Search(CompanyEmployeeSearchModel searchModel);
        IList<string> GetDepartments();
        IList<string> GetWorkTypes(Guid? companyId);
        CompanyEmployee GetById(Guid id);
        void Add(CompanyEmployee entity);
        CompanyEmployee GetEmployee(string idCard, Guid companyId);
    }
}
