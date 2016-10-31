using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class HospitalHealthManageController : BaseController
    {
        private readonly IHealthResultService _healthResultService;
        private readonly ICompanyService _companyService;
        private readonly ICompanyEmployeeService _companyEmployeeService;
        private readonly IWorkContext _workContext;

        public HospitalHealthManageController(IHealthResultService healthResultService
            , ICompanyService companyService
            , ICompanyEmployeeService companyEmployeeService, IWorkContext workContext)
        {
            _healthResultService = healthResultService;
            _companyService = companyService;
            _companyEmployeeService = companyEmployeeService;
            _workContext = workContext;
        }

        #region Utilities

        private void PrepareHealthResultViewModel(HealthResultViewModel model, HealthResult entity)
        {
            var companies = _companyService.GetAll();
            foreach (var item in companies)
            {
                model.AvailableCompanies.Add(new SelectListItem
                {
                    Text = item.CompanyName,
                    Value = item.Id.ToString(),
                    Selected = entity != null
                    && entity.CompanyEmployee != null
                    && entity.CompanyEmployee.Company != null
                    && item.Id == entity.CompanyEmployee.Company.Id
                });
            }
        }

        private void ValidateHealthResultViewModelForSearch(HealthResultViewModel model)
        {
            if (string.IsNullOrEmpty(model.IDCard) || model.IDCard.Length != 18)
            {
                ModelState.AddModelError("IDCard", "无效的身份证号");
            }
        }

        private void ValidateHealthResultViewModelForCreateOrUpdate(HealthResultViewModel model)
        {

        }

        #endregion

        #region Index
        // GET: CompanyHealthManage
        public ActionResult Index()
        {
            var model = new HealthResultSearchViewModel();
            SearchHealthResult(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(HealthResultSearchViewModel model)
        {
            switch (model.ActionType)
            {
                case "Search":
                    return Search(model);
                case "Download":
                    return Download(model);
                case "Reset":
                    return RedirectToAction("Index");
                default:
                    return Content("未知的操作类型");
            }
            //SearchHealthResult(model);
            //return View(model);
        }

        public ActionResult Search(HealthResultSearchViewModel model)
        {
            SearchHealthResult(model);
            return View(model);
        }

        public ActionResult Download(HealthResultSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var companies = _companyService.GetAll();
                LoadCompanyList(model, companies);

                var finalResults = _healthResultService.GetAllFinalResults();
                LoadFinalResultList(model, finalResults);

                IList<string> workTypes = _companyEmployeeService.GetWorkTypes(null);
                LoadWorkTypes(model, workTypes);

                var searchModel = new HealthResultSearchModel
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    EmployeeKeyWord = model.EmployeeKeyWord,
                    HealthResultKeyWord = model.HealthResultKeyWord,
                    MaxAdverseYears = model.MaxAdverseYears,
                    MinAdverseYears = model.MinAdverseYears,
                    SelectedCompany = model.SelectedCompany,
                    SelectedFinalResults = model.SelectedFinalResults,
                    SelectedWorkType = model.SelectedWorkType,
                    AdverseFactor = model.AdverseFactor,
                    MinHealthDate = model.MinHealthDate,
                    MaxHealthDate = model.MaxHealthDate
                };
                model.HealthResults = _healthResultService.Search(searchModel);

                try
                {
                    var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                    var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                    System.IO.File.Copy(Server.MapPath("~/Content/Templates/体检结果模板(导出).xlsx"), filePath);

                    _healthResultService.ExportHealthResult(filePath, model.HealthResults.ToList());

                    return File(filePath, "text/xls", "体检结果.xlsx");
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex);
                    return RedirectToAction("Search", new { @model = model });
                }
            }
        }

        private void SearchHealthResult(HealthResultSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                if (WorkContext.CurrentMembershipUser.MembershipRoles.FirstOrDefault(x => x.Id == new Guid("7A2F0ECA-4DAF-4AA5-8C1D-9CFFD6AAD69F")) != null
                    || WorkContext.CurrentMembershipUser.MembershipRoles.FirstOrDefault(x => x.Id == new Guid("1DE9E15C-D0DE-4B90-BD95-9D1167D86C50")) != null)
                {
                    var companies = _companyService.GetAll();
                    LoadCompanyList(model, companies);
                }
                else
                {
                    LoadCompanyListForEnterpriseUser(model);
                }

                var finalResults = _healthResultService.GetAllFinalResults();
                LoadFinalResultList(model, finalResults);

                IList<string> workTypes = _companyEmployeeService.GetWorkTypes(null);
                LoadWorkTypes(model, workTypes);

                var searchModel = new HealthResultSearchModel
                {
                    PageIndex = model.PageIndex,
                    EmployeeKeyWord = model.EmployeeKeyWord,
                    HealthResultKeyWord = model.HealthResultKeyWord,
                    MaxAdverseYears = model.MaxAdverseYears,
                    MinAdverseYears = model.MinAdverseYears,
                    SelectedCompany = model.SelectedCompany,
                    SelectedFinalResults = model.SelectedFinalResults,
                    SelectedWorkType = model.SelectedWorkType,
                    AdverseFactor = model.AdverseFactor,
                    MinHealthDate = model.MinHealthDate,
                    MaxHealthDate = model.MaxHealthDate
                };
                model.HealthResults = _healthResultService.Search(searchModel);
            }
        }

        private void LoadCompanyListForEnterpriseUser(HealthResultSearchViewModel model)
        {

            model.CompanyList.Add(new SelectListItem { Text = WorkContext.CurrentMembershipUser.Company.CompanyName, Value = WorkContext.CurrentMembershipUser.Company.Id.ToString() });
            
            model.SelectedCompany = WorkContext.CurrentMembershipUser.Company.Id;
        }

        private void LoadWorkTypes(HealthResultSearchViewModel model, IList<string> workTypes)
        {
            foreach (var item in workTypes)
            {
                if (item == string.Empty)
                {
                    model.WorkTypeList.Add("NULL");
                }
                else
                { model.WorkTypeList.Add(item); }
            }
        }

        private void LoadFinalResultList(HealthResultSearchViewModel model, List<string> finalResults)
        {
            foreach (var fr in finalResults)
            {
                if (fr == string.Empty)
                {
                    model.FinalResultList.Add("NULL");
                }
                else
                {
                    model.FinalResultList.Add(fr);
                }
            }
        }

        private void LoadCompanyList(HealthResultSearchViewModel model, IList<Company> companies)
        {
            model.CompanyList.Add(new SelectListItem { Text = "所有", Value = string.Empty });

            foreach (var item in companies)
            {
                model.CompanyList.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString() });
            }
        }


        #endregion

        #region Manage

        public ActionResult CreateOrUpdate(Guid? id = null)
        {
            if (id != null)
            {
                var entity = _healthResultService.GetById(id.Value);
                if (entity != null)
                {
                    var model = new HealthResultViewModel()
                    {
                        Id = entity.Id,
                        UserName = entity.CompanyEmployee.EmployeeBaseInfo.UserName,
                        IDCard = entity.CompanyEmployee.EmployeeBaseInfo.IDCard,
                        Sex = entity.CompanyEmployee.EmployeeBaseInfo.Sex,
                        AdverseMonthes = entity.CompanyEmployee.AdverseMonthes,
                        AdverseFactor = entity.CompanyEmployee.AdverseFactor,
                        CompanyId = entity.CompanyEmployee.Company.Id,
                        CompanyEmployeeId = entity.CompanyEmployee.Id,

                        HealthDate = entity.HealthDate,
                        ReportDate = DateTime.Parse(entity.ReportDate),
                        HealthPerson = entity.HealthPerson,
                        HealthByCompany = entity.HealthByCompany,

                        MainPositiveResult = entity.MainPositiveResult,
                        Result = entity.Result,
                        HealthCode = entity.HealthCode,
                        ImageCode = entity.ImageCode,
                        ReportCode = entity.ReportCode,
                    };
                    PrepareHealthResultViewModel(model, entity);
                    return View(model);
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，未找到Id为" + id.ToString() + "的体检信息"));
                    return RedirectToAction("Index");
                }
            }
            else
            {
                var model = new HealthResultViewModel();
                PrepareHealthResultViewModel(model, null);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(HealthResultViewModel model, string command)
        {
            if (command == "searchEmployee")
            {
                ValidateHealthResultViewModelForSearch(model);

                if (ModelState.IsValid)
                {
                    var entityEmplyee = _companyEmployeeService.GetEmployee(model.IDCard, model.CompanyId);
                    if (entityEmplyee != null)
                    {
                        model.UserName = entityEmplyee.EmployeeBaseInfo.UserName;
                        model.IDCard = entityEmplyee.EmployeeBaseInfo.IDCard;
                        model.Sex = entityEmplyee.EmployeeBaseInfo.Sex;
                        model.AdverseMonthes = entityEmplyee.AdverseMonthes;
                        model.AdverseFactor = entityEmplyee.AdverseFactor;
                        model.CompanyId = entityEmplyee.Company.Id;
                        model.CompanyEmployeeId = entityEmplyee.Id;
                    }
                    else
                    {
                        ErrorNotification(new Exception("未找到员工"));
                        model = new HealthResultViewModel();
                    }
                }
                else
                {
                    ErrorNotification(new Exception("输入信息有误"));
                }
            }
            else if (command == "createOrUpdate")
            {
                ValidateHealthResultViewModelForCreateOrUpdate(model);

                if (ModelState.IsValid)
                {
                    if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            var entity = new HealthResult()
                            {
                                Id = Guid.NewGuid(),
                                MainPositiveResult = model.MainPositiveResult,
                                Result = model.Result,
                                HealthCode = model.HealthCode,
                                ImageCode = model.ImageCode,
                                ReportCode = model.ReportCode,
                                HealthDate = model.HealthDate,
                                ReportDate = model.ReportDate.HasValue ? model.ReportDate.Value.ToString("yyyy-MM-dd") : "",
                                HealthPerson = model.HealthPerson,
                                CompanyEmployee = _companyEmployeeService.GetById(model.CompanyEmployeeId),
                                HealthByCompany = model.HealthByCompany,

                                CreatedBy = _workContext.CurrentMembershipUser.Username,
                                CreatedDate = DateTime.Now
                            };
                            _healthResultService.Add(entity);
                            unitOfWork.Commit();

                            SuccessNotification("添加成功");
                            model = new HealthResultViewModel();
                            PrepareHealthResultViewModel(model, entity);
                            return View(model);
                        }
                    }
                    else
                    {
                        var entity = _healthResultService.GetById(model.Id);
                        if (entity != null)
                        {
                            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                            {
                                entity.MainPositiveResult = model.MainPositiveResult;
                                entity.Result = model.Result;
                                entity.HealthCode = model.HealthCode;
                                entity.ImageCode = model.ImageCode;
                                entity.ReportCode = model.ReportCode;
                                entity.HealthDate = model.HealthDate;
                                entity.ReportDate = model.ReportDate.HasValue ? model.ReportDate.Value.ToString("yyyy-MM-dd") : "";
                                entity.HealthPerson = model.HealthPerson;
                                entity.CompanyEmployee = _companyEmployeeService.GetById(model.CompanyEmployeeId);
                                entity.HealthByCompany = model.HealthByCompany;

                                entity.UpdatedBy = _workContext.CurrentMembershipUser.Username;
                                entity.UpdatedDate = DateTime.Now;

                                unitOfWork.Commit();

                                SuccessNotification("编辑成功");
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ErrorNotification(new Exception("编辑失败，未找到Id为" + model.Id.ToString() + "的体检信息"));
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，输入信息有误"));
                    PrepareHealthResultViewModel(model, null);
                    return View(model);
                }
            }

            PrepareHealthResultViewModel(model, null);
            return View(model);
        }

        #endregion

        #region Delete
        public ActionResult Delete(Guid id)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var healthResult = _healthResultService.GetById(id);
                    healthResult.Deleted = true;
                    unitOfWork.SaveChanges();
                    unitOfWork.Commit();
                }
                SuccessNotification("删除成功！");
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Import Export

        public ActionResult Import()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Import(FormCollection form)
        {
            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        var result = new ImportResultViewModel();

                        //是否覆盖
                        var isCoverData = !string.IsNullOrEmpty(form["IsCoverData"]) && form["IsCoverData"] == "on";

                        //体检结果
                        result.Results.Add(_healthResultService.ImportHealthResult(file.InputStream, isCoverData));

                        //执行插入或修改
                        unitOfWork.Commit();
                        //赋值准备导出
                        this.TempData["ImportResult"] = result;
                        if (result.Results[0].ErrorRows.Count == 0)
                        {
                            SuccessNotification("上传文件成功！");
                        }
                        return View(result);
                    }
                }
                else
                {
                    ErrorNotification(new Exception("上传失败，尝试上传无效的文件"));
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
        }

        [HttpPost]
        public ActionResult ExportErrorResult()
        {
            try
            {
                ImportResultViewModel data = (ImportResultViewModel)this.TempData["ImportResult"];

                var fileName = DateTime.Now.Ticks + ".xlsx";
                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                System.IO.File.Copy(Server.MapPath("~/Content/Templates/体检结果模板.xlsx"), filePath);

                _healthResultService.ExportHealthResult(filePath, data.Results);
                //byte[] bytes = result.ToArray();

                //return File(bytes, "text/xls", "出错信息.xlsx");

                return File(filePath, "text/xls", "出错信息.xlsx");
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Import");
            }
        }

        #endregion
    }
}