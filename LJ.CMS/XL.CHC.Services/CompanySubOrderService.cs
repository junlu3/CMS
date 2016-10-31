using System;
using System.Collections.Generic;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Repositories;
using XL.CHC.Domain.Interfaces.Services;
using System.Linq;
namespace XL.CHC.Services
{
    public class CompanySubOrderService : ICompanySubOrderService
    {
        private readonly ICompanySubOrderRepository _companySubOrderRepository;
        private readonly IHospitalCalendarRepository _hospitalCalendarRepository;
        private readonly IEmailService _emailService;
        private readonly IWorkContext _workContext;
        private readonly ICategoryService _categoryService;

        //private readonly static string DELETE_EMAIL_BY_HOSPITAL_FORMATTER = "您的预约\"{0}\" 已被医院删除，请核查。";
        //private readonly static string APPROVE_EMAIL_BY_HOSPITAL_FORMATTER = "您定于{0}的预约已被批准，请核查。";
        //private readonly static string APPROVE_CHANGE_EMAIL_BY_HOSPITAL_FORMATTER = "您原定于{0}的预约已被医院修改为{1}且已确认。";
        //private readonly static string REJECT_EMAIL_BY_HOSPITAL_FORMATTER = "您的预约{0}已被医院拒绝，请核查。";

        public CompanySubOrderService(ICompanySubOrderRepository companySubOrderRepository
            , IHospitalCalendarRepository hospitalCalendarRepository
            , IEmailService emailService
            , IWorkContext workContext
            , ICategoryService categoryService)
        {
            _companySubOrderRepository = companySubOrderRepository;
            _hospitalCalendarRepository = hospitalCalendarRepository;
            _emailService = emailService;
            _workContext = workContext;
            _categoryService = categoryService;
        }

        public void Add(CompanySubOrder order)
        {
            //IList<HospitalCalendar> calendars = _hospitalCalendarRepository.GetCalendarData(Convert.ToDateTime(order.StartDate), Convert.ToDateTime(order.EndDate));
            //if (calendars.Count == 0)
            //{
            //    HospitalCalendar entity = new HospitalCalendar
            //    {
            //        StartDate = order.StartDate,
            //        EndDate = order.EndDate,
            //        CreatedBy = order.CreatedBy
            //    };
            //    _hospitalCalendarRepository.AddCalendar(entity);
            //}
            _companySubOrderRepository.Add(order);


        }

        public IList<CompanySubOrder> GetByDate(DateTime startDate, DateTime endDate)
        {
            return _companySubOrderRepository.GetByDate(startDate, endDate);
        }

        public CompanySubOrder GetById(Guid id)
        {
            return _companySubOrderRepository.GetById(id);
        }

        public IList<CompanySubOrder> GetSubOrders(DateTime startDate, DateTime endDate)
        {
            return _companySubOrderRepository.GetSubOrders(startDate, endDate);
        }

        public IPagedList<CompanySubOrder> Search(CompanySubOrderSearchModel companySubOrderSearchModel)
        {
            return _companySubOrderRepository.Search(companySubOrderSearchModel);
        }

        public void Delete(string subOrderId)
        {
            var entity = _companySubOrderRepository.GetById(new Guid(subOrderId));
            if (entity == null)
            {
                throw new Exception("未找到编号为 " + subOrderId + " 的子订单");
            }
            else
            {
                if (entity.ParentOrder == null)
                {
                    _companySubOrderRepository.Delete(entity);
                }
                else
                {
                    throw new Exception("编号为 " + subOrderId + " 的子订单已有父订单，不能被删除");
                }
            }
        }

        public void DeleteByHospital(string subOrderId)
        {
            var entity = _companySubOrderRepository.GetById(new Guid(subOrderId));
            if (entity == null)
            {
                throw new Exception("未找到编号为 " + subOrderId + " 的子订单");
            }
            else
            {
                if (entity.ParentOrder == null)
                {
                    
                    //if (entity.Company.MembershipUser.EmailTaskTypes.FirstOrDefault(x => x.Id == 37) != null)
                    //{
                    //    var email = new Email
                    //    {
                    //        To = entity.Company.MembershipUser.Email,
                    //        ToName = entity.Company.CompanyName,
                    //        Body = string.Format(EmailBodyFormatter.EnterpriseBody, entity.Company.CompanyName, string.Format(DELETE_EMAIL_BY_HOSPITAL_FORMATTER, Convert.ToDateTime(entity.StartDate).ToString("yyyy-MM-dd"))),
                    //        CreatedBy=_workContext.CurrentMembershipUser.Username
                    //    };
                    //    _emailService.Add(email);

                    //}


                    _companySubOrderRepository.Delete(entity);

                }
                else
                {
                    throw new Exception("编号为 " + subOrderId + " 的子订单已有父订单，不能被删除");
                }
            }
        }

        public void ApproveSubOrder(Guid subOrderId, DateTime date, bool approved)
        {
            DateTime? oldStartTime = null;
            CompanySubOrder subOrder = GetById(subOrderId);
            oldStartTime = subOrder.StartDate;
            var bookStatus = _categoryService.GetById(18);
            if (!approved)
            {
                bookStatus = _categoryService.GetById(19);
            }

            subOrder.StartDate = date;
            subOrder.EndDate = date.AddDays(1);
            subOrder.UpdatedBy = _workContext.CurrentMembershipUser.Username;
            subOrder.UpdatedDate = DateTime.Now;
            subOrder.BookStatus = bookStatus;

            //if (subOrder.Company.MembershipUser.EmailTaskTypes.FirstOrDefault(x => x.Id == 28) != null)
            //{
            //    string emailBody = string.Empty;
            //    if (approved)
            //    {
            //        if (oldStartTime == date)
            //        {
            //            emailBody = string.Format(APPROVE_EMAIL_BY_HOSPITAL_FORMATTER, Convert.ToDateTime(date).ToString("yyyy-MM-dd"));
            //        }
            //        else
            //        {
            //            emailBody = string.Format(APPROVE_CHANGE_EMAIL_BY_HOSPITAL_FORMATTER, Convert.ToDateTime(oldStartTime).ToString("yyyy-MM-dd"), Convert.ToDateTime(date).ToString("yyyy-MM-dd"));
            //        }
            //    }
            //    else
            //    {
            //        emailBody = string.Format(REJECT_EMAIL_BY_HOSPITAL_FORMATTER, Convert.ToDateTime(oldStartTime).ToString("yyyy-MM-dd"));
            //    }
            //    var email = new Email
            //    {
            //        To = subOrder.Company.MembershipUser.Email,
            //        ToName = subOrder.Company.CompanyName,
            //        Body = string.Format(EmailBodyFormatter.EnterpriseBody, subOrder.Company.CompanyName, emailBody),
            //    };

            //    _emailService.Add(email);
            //}
        }
    }
}
