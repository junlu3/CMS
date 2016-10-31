using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
namespace XL.CHC.Domain.Interfaces.Services
{
    public interface ICompanySubOrderService
    {
        /// <summary>
        /// 获取时间段未被拒绝的预约
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IList<CompanySubOrder> GetSubOrders(DateTime startDate, DateTime endDate);
        IPagedList<CompanySubOrder> Search(CompanySubOrderSearchModel companySubOrderSearchModel);
        IList<CompanySubOrder> GetByDate(DateTime date, DateTime dateTime);
        void Add(CompanySubOrder order);
        CompanySubOrder GetById(Guid id);
        void Delete(string subOrderId);
        void DeleteByHospital(string v);
        void ApproveSubOrder(Guid subOrderId, DateTime date, bool approved);
    }
}
