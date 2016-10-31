using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
    public interface ICompanyOrderService
    {
        IPagedList<CompanyOrder> Search(CompanyOrderSearchModel searchModel);

        CompanyOrder GetById(Guid orderId);

        void Add(CompanyOrder order);

        void ExportCompanyOrderForms(string webRootPath, CompanyOrder order);

        void ExportCheckForm(string filePath, CompanyEmployee emp, string webRootPath = "");

        void ExportCheckerRegistrationForm(string filePath, CompanyOrder order);

        void ExportAdverseFactorContactSituationRegistrationForm(string filePath, CompanyOrder order);

        void ExportProtocol(string filePath, CompanyOrder order);

        void ExportAdverseFactorNoticeForm(string filePath, CompanyEmployee emp);

        void ExportHealthCareRecords(string filePath, CompanyEmployee emp);

        void Delete(string orderId);
        void SendEmailWhileUploadAgreement(Guid id);
    }
}
