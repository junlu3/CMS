using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Web.Models;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Web.Infrastructure;
using XL.Utilities;
using System.IO;

namespace XL.CHC.Web.Controllers
{
    public class CompanyOrderManageController : BaseController
    {
        private readonly ICompanyOrderService _companyOrderService;
        private readonly ICompanySubOrderService _companySubOrderService;
        private readonly ICompanyEmployeeService _companyEmployeeService;
        private readonly ICompanyService _companyService;

        public CompanyOrderManageController(ICompanyOrderService companyOrderService
            , ICompanySubOrderService companySubOrderService
            , ICompanyEmployeeService companyEmployeeService
            , ICompanyService companyService)
        {
            _companyOrderService = companyOrderService;
            _companySubOrderService = companySubOrderService;
            _companyEmployeeService = companyEmployeeService;
            _companyService = companyService;
        }

        #region Index

        // GET: CompanyOrderManage
        public ActionResult Index()
        {
            var model = new CompanyOrderSearchViewModel();
            if (WorkContext.CurrentMembershipUser.Company != null)
            {
                model.SelectedCompanyId = WorkContext.CurrentMembershipUser.Company.Id;
            }
            SearchOrders(model);
            return View(model);
        }

        private void SearchOrders(CompanyOrderSearchViewModel model)
        {
            try
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    var companies = _companyService.GetAll();
                    model.CompanyList.Add(new SelectListItem { Text = "所有", Value = "" });
                    foreach (var item in companies)
                    {
                        model.CompanyList.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString() });
                    }
                    Guid? companyId = model.SelectedCompanyId;

                    var searchModel = new CompanyOrderSearchModel
                    {
                        PageIndex = model.PageIndex,
                        CompanyId = companyId,
                        MinBookTime = model.MinSearchTime,
                        MaxBookTime = model.MaxSearchTime,
                    };
                    model.CompanyOrders = _companyOrderService.Search(searchModel);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }

        }

        [HttpPost]
        public ActionResult Index(CompanyOrderSearchViewModel model)
        {
            SearchOrders(model);
            return View(model);
        }

        public ActionResult IndexForHospital()
        {
            var model = new CompanyOrderSearchViewModel();
            SearchOrders(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult IndexForHospital(CompanyOrderSearchViewModel model)
        {
            SearchOrders(model);
            return View(model);
        }



        #endregion

        #region DownLoad
        public ActionResult DownLoad(Guid id)
        {
            var model = new CompanyOrderDownLoadViewModel();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var order = _companyOrderService.GetById(id);
                model.Order = order;
                model.IsHospitalRole = WorkContext.CurrentMembershipUser.MembershipRoles.FirstOrDefault(x => x.Id == new Guid("1DE9E15C-D0DE-4B90-BD95-9D1167D86C50")) != null;
            }
            return View(model);
        }


        [HttpPost]
        public ActionResult UploadAgreement(CompanyOrderDownLoadViewModel model)
        {
            var targetPath = string.Empty;
            var file = Request.Files["ImportFile"];
            var fileName = "";
            if (!string.IsNullOrEmpty(Request["upfile"]) && file != null && file.ContentLength > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!(fileExtension == ".doc" || fileExtension == ".docx" || fileExtension == ".xls" || fileExtension == ".xlsx"))
                {
                    ErrorNotification(new Exception("只能上传Word 或者 Excel 文档"));
                    return View(model);
                }
                else
                {
                    fileName = DateTime.Now.Ticks.ToString() + fileExtension;
                    targetPath = Server.MapPath(string.Format("~/Content/ExportFiles/Orders/{0}/{1}", model.Order.Id.ToString(), "协议书.xlsx"));
                    file.SaveAs(targetPath);
                    SuccessNotification("上传协议书成功！");

                    try
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            _companyOrderService.SendEmailWhileUploadAgreement(model.Order.Id);
                        }

                    }
                    catch (Exception ex)
                    {
                        ErrorNotification(ex);
                    }
                }
            }

            return RedirectToAction("Download", new { id = model.Order.Id });
        }
        #endregion

        public ActionResult Create(Guid? id = null)
        {
            var model = new CompanyOrderCreateViewModel();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (id != null)
                {
                    var order = _companyOrderService.GetById(new Guid(id.ToString()));
                    model.Comment = order.Comment;
                    model.OrderId = order.Id;
                    model.AssignedEmployees = order.CompanyEmployees;
                    model.AssignedSubOrders = order.SubOrders;
                }

                InitModelSubOrders(model);
                InitModelEmployees(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyOrderCreateViewModel model, FormCollection form)
        {
            try
            {
                  var company = WorkContext.CurrentMembershipUser.Company;
                Guid companyId = company.Id;

                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var order = new CompanyOrder();

                    if (model.OrderId == null)
                    {
                        order.Comment = model.Comment;
                        if (!string.IsNullOrEmpty(model.AssignedSubOrderString))
                        {
                            var subOrderIds = model.AssignedSubOrderString.Split(',');
                            GenerateOrderSubOrders(order, subOrderIds);
                        }
                        if (!string.IsNullOrEmpty(model.AssignedEmployeesString))
                        {
                            var employeeIds = model.AssignedEmployeesString.Split(',');
                            GenerateEmployees(order, employeeIds);
                        }
                        order.Company = _companyService.GetById(companyId);
                        order.IsBuildCompleted = false;
                        order.Locked = false;
                        order.CreatedBy = WorkContext.CurrentMembershipUser.Username;
                        order.CreatedDate = DateTime.Now;
                        _companyOrderService.Add(order);

                    }
                    else
                    {
                        order = _companyOrderService.GetById(new Guid(model.OrderId.ToString()));
                        order.Comment = model.Comment;
                        order.SubOrders.Clear();
                        if (!string.IsNullOrEmpty(model.AssignedSubOrderString))
                        {
                            var subOrderIds = model.AssignedSubOrderString.Split(',');
                            GenerateOrderSubOrders(order, subOrderIds);
                        }
                        order.CompanyEmployees.Clear();
                        if (!string.IsNullOrEmpty(model.AssignedEmployeesString))
                        {
                            var employeeIds = model.AssignedEmployeesString.Split(',');
                            GenerateEmployees(order, employeeIds);
                        }
                        order.Company = _companyService.GetById(companyId);
                        order.IsBuildCompleted = false;
                        order.Locked = model.IsLocked;
                        order.UpdatedBy = WorkContext.CurrentMembershipUser.Username;
                        order.UpdatedDate = DateTime.Now;

                    }
                    model.AssignedSubOrders = order.SubOrders;
                    model.AssignedEmployees = order.CompanyEmployees;
                    model.OrderId = order.Id;
                    unitOfWork.Commit();

                    SuccessNotification("保存成功");
                }


            }
            catch (Exception e)
            {
                ErrorNotification(e);

            }
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                InitModelSubOrders(model);
                InitModelEmployees(model);
            }
            return View(model);
        }

        private void GenerateEmployees(CompanyOrder order, string[] employeeIds)
        {
            foreach (var employeeId in employeeIds)
            {
                if (string.IsNullOrEmpty(employeeId)) continue;
                var employee = _companyEmployeeService.GetById(new Guid(employeeId));
                if (employee != null)
                {
                    order.CompanyEmployees.Add(employee);
                }
            }
        }

        private void GenerateOrderSubOrders(CompanyOrder order, string[] subOrderIds)
        {
            foreach (var subOrderId in subOrderIds)
            {
                if (string.IsNullOrEmpty(subOrderId)) continue;

                var subOrder = _companySubOrderService.GetById(new Guid(subOrderId));
                if (subOrder != null)
                {
                    order.SubOrders.Add(subOrder);
                }
            }
        }

        private void InitModelSubOrders(CompanyOrderCreateViewModel model)
        {
            var companyId = WorkContext.CurrentMembershipUser.Company.Id;
            var searchSubOrderModel = new CompanySubOrderSearchModel
            {
                CompanyId = companyId,
                SelectedBookStatus = 18,
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            var subOrders = _companySubOrderService.Search(searchSubOrderModel);
            LoadSubOrders(model, subOrders);
        }

        private void InitModelEmployees(CompanyOrderCreateViewModel model)
        {
            var company = WorkContext.CurrentMembershipUser.Company;
            var allCompanEmployees = company.CompanyEmployees;
            if (model.AssignedEmployees.Count > 0)
            {
                foreach (var item in allCompanEmployees)
                {
                    if (model.AssignedEmployees.FirstOrDefault(x => x.Id == item.Id) == null)
                    {
                        model.NotAssignedEmployees.Add(item);
                    }
                }
                foreach (var item in model.AssignedEmployees)
                {
                    model.AssignedEmployeesString += (item.Id.ToString() + ",");
                }

                model.AssignedEmployeesString = model.AssignedEmployeesString.Substring(0, model.AssignedEmployeesString.Length - 1);

            }

        }

        [HttpPost]
        public string SaveFirstStepInfo(Guid? orderId, string subOrdersString, string comment)
        {
            var returnObject = new AJAXReturnObject();
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var order = new CompanyOrder();
                    if (orderId == null)
                    {
                        var company = _companyService.GetById(WorkContext.CurrentMembershipUser.Company.Id);
                        order = new CompanyOrder
                        {
                            CreatedBy = WorkContext.CurrentMembershipUser.Username,
                            Company = company,
                        };

                    }
                    else
                    {
                        order = _companyOrderService.GetById(new Guid(orderId.ToString()));
                        order.UpdatedBy = WorkContext.CurrentMembershipUser.Username;
                    }


                    order.Comment = comment;
                    string[] subOrderIds = subOrdersString.Split(',');
                    if (subOrderIds.Length > 0)
                    {
                        foreach (var subOrderId in subOrderIds)
                        {
                            var subOrder = _companySubOrderService.GetById(new Guid(subOrderId));
                            order.SubOrders.Add(subOrder);
                        }
                    }
                    if (orderId == null)
                    {
                        _companyOrderService.Add(order);
                    }
                    unitOfWork.Commit();
                    orderId = order.Id;
                }
                catch (Exception ex)
                {
                    returnObject.Status = AJAXReturnResult.Failed;
                    returnObject.Message = ex.Message;
                }
            }
            return "{\"Status\":\"0\",\"orderId\":\"" + orderId + "\"}";
        }


        private void LoadSubOrders(CompanyOrderCreateViewModel model, IPagedList<CompanySubOrder> subOrders)
        {

            foreach (var item in subOrders)
            {
                if (model.AssignedSubOrders.FirstOrDefault(x => x.Id == item.Id) == null)
                {
                    model.NotAssignedSubOrders.Add(item);
                }
            }
            foreach (var item in model.AssignedSubOrders)
            {
                model.AssignedSubOrderString += item.Id.ToString() + ",";
            }
            if (model.AssignedSubOrders.Count > 0)
            {
                model.AssignedSubOrderString = model.AssignedSubOrderString.Substring(0, model.AssignedSubOrderString.Length - 1);
            }
        }

        public ActionResult View(Guid id)
        {
            var model = new CompanyOrderCreateViewModel();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (id != null)
                {
                    var order = _companyOrderService.GetById(new Guid(id.ToString()));
                    model.Comment = order.Comment;
                    model.OrderId = order.Id;
                    model.AssignedEmployees = order.CompanyEmployees;
                    model.AssignedSubOrders = order.SubOrders;

                }

                InitModelSubOrders(model);
                InitModelEmployees(model);
            }
            return View(model);
        }

        public ActionResult Delete(string orderId)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    _companyOrderService.Delete(orderId);

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