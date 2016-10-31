using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;
using XL.Utilities;

namespace XL.CHC.Web.Controllers
{
    public class HospitalBookManageController : BaseController
    {
        public readonly IHospitalCalendarService _hospitalCalendarService;
        private readonly ICategoryService _categoryService;
        private readonly ICompanySubOrderService _companySubOrderService;
        private readonly ICompanyService _companyService;
        // GET: HospitalBookManage
        public HospitalBookManageController(ICategoryService categoryService
            , ICompanySubOrderService companySubOrderService
            , IHospitalCalendarService hospitalCalendar
            , ICompanyService companyService) : base()
        {
            _categoryService = categoryService;
            _companySubOrderService = companySubOrderService;
            _hospitalCalendarService = hospitalCalendar;
            _companyService = companyService;
        }
        public ActionResult Index()
        {
            var model = new HospitalBookSearchViewModel();
            SearchSubOrders(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(HospitalBookSearchViewModel model)
        {
            SearchSubOrders(model);
            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            var model = new HospitalBookProcessViewModel();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                CompanySubOrder order = _companySubOrderService.GetById(id);
                model.CompanySubOrder = order;
            }
            return View(model);
        }

        [HttpPost]
        public string Save(DateTime date, Guid subOrderId, bool approved)
        {
            var returnObject = new XL.CHC.Web.Infrastructure.AJAXReturnObject();
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        _companySubOrderService.ApproveSubOrder(subOrderId, date, approved);

                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                returnObject.Status = Infrastructure.AJAXReturnResult.Failed;
                returnObject.Message = ex.Message;
            }
            return JsonHelper.SerializeToJson(returnObject);
        }

        private void SearchSubOrders(HospitalBookSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                LoadBookStatusList(model);
                LoadCompanies(model);
                model.CompanySubOrders = _companySubOrderService.Search(new CompanySubOrderSearchModel
                {
                    CompanyId = model.SelectedComany,
                    MaxSearchTime = model.MaxSearchTime,
                    MinSearchTime = model.MinSearchTime,
                    SelectedBookStatus = model.SelectedBookStatus,
                    PageIndex = model.PageIndex,
                });
            }

        }

        private void LoadCompanies(HospitalBookSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                IList<Company> cps = _companyService.GetAll();
                model.CompanyList.Add(new SelectListItem { Text = "所有", Value = string.Empty });
                foreach (var item in cps)
                {
                    model.CompanyList.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString() });
                }
            }
        }

        private void LoadBookStatusList(HospitalBookSearchViewModel model)
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

        public ActionResult Delete(Guid id)
        {

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    _companySubOrderService.DeleteByHospital(id.ToString());

                    unitOfWork.Commit();

                    SuccessNotification("删除成功");
                }

                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    ErrorNotification(ex);
                }
            }
            return RedirectToAction("Index");

        }
    }
}