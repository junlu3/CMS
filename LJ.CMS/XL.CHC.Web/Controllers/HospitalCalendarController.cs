
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.UnitOfWork;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Services;
using XL.Utilities;
using System;
using System.Collections.Generic;
using XL.CHC.Web.Models;
using XL.CHC.Web.Infrastructure;

namespace XL.CHC.Web.Controllers
{
    public class HospitalCalendarController : BaseController
    {
        public readonly IHospitalCalendarService _hospitalCalendarService;
        public readonly ICompanySubOrderService _companySubOrderService;
        public HospitalCalendarController(IHospitalCalendarService hospitalCalendar
            , ICompanySubOrderService companySubOrderService) : base()
        {
            _hospitalCalendarService = hospitalCalendar;
            _companySubOrderService = companySubOrderService;
        }
        // GET: HospitoalCalendar
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetMonthData(int year, int month)
        {
            List<HospitalCalendarViewModel> models = new List<HospitalCalendarViewModel>();
            DateTime startTime = new DateTime(year, month, 1);
            DateTime endTime = startTime.AddMonths(1);
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                IList<HospitalCalendar> result = _hospitalCalendarService.GetCalendarData(startTime, endTime);
                foreach (var item in result)
                {
                    IList<CompanySubOrder> subOrders = _companySubOrderService.GetSubOrders(Convert.ToDateTime(item.StartDate), Convert.ToDateTime(item.EndDate));

                    int orderStatus = 0;
                    if (subOrders.Count > 0)
                    {
                        orderStatus = subOrders[0].BookStatus.Name == "已确认" ? 2 : 1;
                    }
                    models.Add(new HospitalCalendarViewModel
                    {
                        Id = item.Id,
                        CreatedDate = item.CreatedDate,
                        EndDateString = Convert.ToDateTime(item.EndDate).ToString("yyyy-MM-dd"),
                        StartDateString = Convert.ToDateTime(item.StartDate).ToString("yyyy-MM-dd"),
                        CreatedBy = item.CreatedBy,
                        Enabled = item.Enabled,
                        OrderStatus = orderStatus
                    });
                }
                if (models.Count == 0)
                {
                    var interval = (endTime - startTime).Days;
                    for (var i = 0; i < interval; i++)
                    {
                        models.Add(new HospitalCalendarViewModel
                        {
                            StartDate = startTime.AddDays(i),
                            EndDate = startTime.AddDays(i + 1),
                            StartDateString = startTime.AddDays(i).ToString("yyyy-MM-dd"),
                            EndDateString = startTime.AddDays(i + 1).ToString("yyyy-MM-dd"),
                        });
                    }
                  
                }
                return JsonHelper.SerializeToJson(models);
            }
            //return JsonHelper.SerializeToJson(new List<HospitalCalendar>());
        }

        [HttpPost]
        public string SaveCalendar(List<HospitalCalendarViewModel> calendarData)
        {
            var returnObject = new AJAXReturnObject();
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    HospitalCalendar entity = new HospitalCalendar();
                    foreach (var item in calendarData)
                    {
                        if (item.Id != null)
                        {
                            entity = _hospitalCalendarService.GetById(new Guid(item.Id.ToString()));
                            entity.Enabled = item.Enabled;
                            entity.UpdatedBy = WorkContext.CurrentMembershipUser.Username;
                        }
                        else
                        {
                            entity = new HospitalCalendar();
                            entity.Enabled = item.Enabled;
                            entity.CreatedBy = entity.UpdatedBy = WorkContext.CurrentMembershipUser.Username;
                            entity.StartDate = item.StartDate;
                            entity.EndDate = item.EndDate;
                            _hospitalCalendarService.AddCalendar(entity);
                        }
                        unitOfWork.SaveChanges();
                    }
                    unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                returnObject.Status = AJAXReturnResult.Failed;
                returnObject.Message = ex.Message;
                
            }

            return JsonHelper.SerializeToJson(returnObject);
        }
    }
}