using System;
using System.Linq;
using System.Web.Mvc;
using XL.CHC.Web.Models;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeBaseInfoService _employeeBaseInfoService;
        private readonly ICompanyOrderService _companyOrderService;
        private readonly ICompanyEmployeeService _companyEmployeeService;

        public EmployeeController(IEmployeeBaseInfoService employeeBaseInfoService
            , ICompanyOrderService companyOrderService, ICompanyEmployeeService companyEmployeeService)
        {
            _employeeBaseInfoService = employeeBaseInfoService;
            _companyOrderService = companyOrderService;
            _companyEmployeeService = companyEmployeeService;
        }
        // GET: Employee
        public ActionResult MainPage(Guid? id, DateTime? startDate = null, DateTime? endDate = null)
        {
            var model = new EmployeeMainPageViewModel();
            if (id != null)
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = _employeeBaseInfoService.GetById(new Guid(id.ToString()));
                    if (user != null)
                    {
                        model.EmployeeBaseInfo = user;
                        model.KeyWord = user.IDCard;
                        var attachementViewModel = new EmployeeAttachementViewModel();
                        attachementViewModel.IDCard = model.EmployeeBaseInfo.IDCard;
                        attachementViewModel.MinSearchTime = startDate;
                        attachementViewModel.MaxSearchTime = endDate;

                        GenerateAttachementOrders(attachementViewModel);

                        model.AttachementViewModel = attachementViewModel;
                    }
                }
            }
            return View(model);
        }

        private void GenerateAttachementOrders(EmployeeAttachementViewModel attachementViewModel)
        {
            attachementViewModel.CompanyOrders.Clear();
            var orders = _companyOrderService.Search(new Domain.DomainModel.CompanyOrderSearchModel
            {
                MinBookTime = attachementViewModel.MinSearchTime,
                MaxBookTime = attachementViewModel.MaxSearchTime
            });
            foreach (var item in orders)
            {
                if (item.CompanyEmployees.FirstOrDefault(x => x.EmployeeBaseInfo.IDCard == attachementViewModel.IDCard) != null)
                {
                    attachementViewModel.CompanyOrders.Add(item);
                }
            }
        }

        public ActionResult _OrderAttachement()
        {
            var model = new EmployeeAttachementViewModel();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _OrderAttachement(EmployeeAttachementViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                GenerateAttachementOrders(model);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ExportAttachement(EmployeeAttachementViewModel model, FormCollection form)
        {
            if (!string.IsNullOrEmpty(model.IDCard) && model.ExportOrderId.HasValue && model.ExportOrderId.Value != null && !string.IsNullOrEmpty(form["ExportFileName"]))
            {
                var fileName = form["ExportFileName"];
                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                var order = _companyOrderService.GetById(model.ExportOrderId.Value);
                var emp = order.CompanyEmployees.Where(e => e.EmployeeBaseInfo.IDCard == model.IDCard).SingleOrDefault();
                switch (model.ExportTarget)
                {
                    case EmployeeAttachementExportTarget.NotifyReport:
                        System.IO.File.Copy(Server.MapPath("~/Content/Templates/职业病危害因素告知书.xlsx"), filePath, true);
                        _companyOrderService.ExportAdverseFactorNoticeForm(filePath, emp);
                        return File(filePath, "text/xls", fileName);
                    case EmployeeAttachementExportTarget.HealthRegisterForm:
                        System.IO.File.Copy(Server.MapPath("~/Content/Templates/上海市职业健康检查表.xlsx"), filePath, true);
                        _companyOrderService.ExportCheckForm(filePath, emp, Server.MapPath("~"));
                        //转成pdf

                        return File(filePath, "text/xls", fileName);
                    default:
                        return Content("未知的导出类型");
                }
            }
            return Content("导出所需资料不足");
        }

        [HttpPost]
        public ActionResult ExportHealthResult(FormCollection form)
        {
            var idCard = form["EmployeeBaseInfo.IDCard"];
            var company = WorkContext.CurrentMembershipUser.Company;
            var emp = company.CompanyEmployees.Where(e => e.EmployeeBaseInfo.IDCard == idCard).SingleOrDefault();
            if (emp != null)
            {
                var fileName = "职业卫生监护档案模板_" + idCard + ".xlsx";
                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                System.IO.File.Copy(Server.MapPath("~/Content/Templates/职业卫生监护档案模板.xlsx"), filePath, true);
                _companyOrderService.ExportHealthCareRecords(filePath, emp);
                return File(filePath, "text/xls", fileName);
            }
            else
            {
                return Content("未找到该用户");
            }
            //return ExportAttachement(new EmployeeAttachementViewModel());
        }
    }
}