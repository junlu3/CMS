using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Infrastructure;
using XL.CHC.Web.Models;
using XL.Utilities;
using System.Linq;

namespace XL.CHC.Web.Controllers
{

    public class CompanyBookManageController : BaseController
    {
        public readonly IHospitalCalendarService _hospitalCalendarService;
        private readonly ICategoryService _categoryService;
        private readonly ICompanySubOrderService _companySubOrderService;
        private readonly ICompanyService _companyService;
        public CompanyBookManageController(ICategoryService categoryService
            , ICompanySubOrderService companySubOrderService
            , IHospitalCalendarService hospitalCalendar
            , ICompanyService companyService) : base()
        {
            _categoryService = categoryService;
            _companySubOrderService = companySubOrderService;
            _hospitalCalendarService = hospitalCalendar;
            _companyService = companyService;
        }

        #region Index

        // GET: CompanyBookManage
        public ActionResult Index()
        {
            var model = new CompanyBookSearchViewModel();
            SearchSubOrders(model);
            return View(model);
        }

        private void SearchSubOrders(CompanyBookSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                LoadBookStatusList(model);
                model.CompanySubOrders = _companySubOrderService.Search(new CompanySubOrderSearchModel
                {
                    CompanyId = WorkContext.CurrentMembershipUser.Company.Id,
                    MaxSearchTime = model.MaxSearchTime,
                    MinSearchTime = model.MinSearchTime,
                    SelectedBookStatus = model.SelectedBookStatus,
                    PageIndex = model.PageIndex,
                });
            }

        }

        [HttpPost]
        public ActionResult Index(CompanyBookSearchViewModel model)
        {
            SearchSubOrders(model);
            return View(model);
        }

        private void LoadBookStatusList(CompanyBookSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                IList<Category> cats = _categoryService.GetByParentName("SubOrderStatus");
                model.BookStatusList.Add(new SelectListItem { Text = "所有", Value = string.Empty });
                foreach (var item in cats)
                {
                    model.BookStatusList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }

        }

        #endregion

        #region New
        public ActionResult NewSubOrder()
        {
            return View();
        }

        [HttpPost]
        public string CreateNewSubOrder(DateTime date, string comment)
        {
            var returnObject = new AJAXReturnObject();
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    IList<CompanySubOrder> orders = _companySubOrderService.GetByDate(date, date.AddDays(1));
                    if (orders.Count > 0)
                    {
                        returnObject.Status = AJAXReturnResult.Failed;
                        returnObject.Message = "您预定的日期已被其他企业预定。";
                    }
                    else
                    {
                        Category initStatus = _categoryService.GetById(17);
                        var company = _companyService.GetById(WorkContext.CurrentMembershipUser.Company.Id);
                        CompanySubOrder order = new CompanySubOrder
                        {
                            StartDate = date,
                            EndDate = date.AddDays(1),
                            BookStatus = initStatus,
                            Comment = comment,
                            Company = company,
                            CreatedBy = WorkContext.CurrentMembershipUser.Username,
                            ParentOrder = null
                        };
                        _companySubOrderService.Add(order);

                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            throw ex;
                        }
                      
                    }
                }
            }
            catch (Exception ex)
            {
                returnObject.Status = AJAXReturnResult.Failed;
                returnObject.Message = ex.Message;
            }

            return JsonHelper.SerializeToJson(returnObject);
        }
        [AllowAnonymous]
        [HttpPost]
        public string GetMonthData(int year, int month, Guid? companyId)
        {
            List<CompanyCalendarViewModel> models = new List<CompanyCalendarViewModel>();
            DateTime startTime = new DateTime(year, month, 1);
            DateTime endTime = startTime.AddMonths(1);
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                IList<HospitalCalendar> result = _hospitalCalendarService.GetCalendarData(startTime, endTime);
                if (companyId == null)
                    companyId = WorkContext.CurrentMembershipUser.Company.Id;
                var company = _companyService.GetById(new Guid(companyId.ToString()));
                foreach (var item in result)
                {
                    IList<CompanySubOrder> subOrders = _companySubOrderService.GetSubOrders(Convert.ToDateTime(item.StartDate), Convert.ToDateTime(item.EndDate));

                    int orderStatus = 0;
                    if (subOrders.Count > 0)
                    {
                        orderStatus = subOrders[0].BookStatus.Name == "已确认" ? 2 : 1;
                    }
                    bool belongCompanyOrder = company.CompanySubOrders
                        .Where(x => x.StartDate <= item.StartDate && x.EndDate >= item.EndDate).ToList().Count > 0;
                    models.Add(new CompanyCalendarViewModel
                    {
                        Id = item.Id,
                        CreatedDate = item.CreatedDate,
                        EndDateString = Convert.ToDateTime(item.EndDate).ToString("yyyy-MM-dd"),
                        StartDateString = Convert.ToDateTime(item.StartDate).ToString("yyyy-MM-dd"),
                        CreatedBy = item.CreatedBy,
                        Enabled = item.Enabled,
                        OrderStatus = orderStatus,
                        BelongToCompany = belongCompanyOrder
                    });
                }
            }
            return JsonHelper.SerializeToJson(models);
        }
        #endregion

        public ActionResult DeleteSubOrder(string subOrderId)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    _companySubOrderService.Delete(subOrderId);

                    unitOfWork.Commit();

                    SuccessNotification("删除成功");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception(ex.Message));

                return RedirectToAction("Index");
            }
        }
    }
}