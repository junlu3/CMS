using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;
using System.Linq;
using XL.CHC.Domain.Interfaces;

namespace XL.CHC.Web.Controllers
{
    public class CompanyEmployeeManageController : BaseController
    {
        #region Fields

        private readonly ICompanyEmployeeService _companyEmployeeService;
        private readonly IEmployeeWorkHistoryService _employeeWorkHistoryService;
        private readonly ICategoryService _categoryService;
        private readonly IWorkContext _workContext;
        private readonly ICompanyService _companyService;
        private readonly IEmployeeBaseInfoService _employeeBaseInfoService;

        #endregion

        #region Ctor

        public CompanyEmployeeManageController(ICompanyEmployeeService companyEmployeeService,
            IEmployeeWorkHistoryService employeeWorkHistoryService,
            ICategoryService categoryService, IWorkContext workContext, ICompanyService companyService,
            IEmployeeBaseInfoService employeeBaseInfoService) : base()
        {
            this._companyEmployeeService = companyEmployeeService;
            this._employeeWorkHistoryService = employeeWorkHistoryService;
            this._categoryService = categoryService;
            this._workContext = workContext;
            this._companyService = companyService;
            this._employeeBaseInfoService = employeeBaseInfoService;
        }

        #endregion

        #region Utilities

        private void PrepareCompanyEmployeeViewModel(CompanyEmployeeViewModel model, CompanyEmployee entity)
        {
            var companies = _companyService.GetAll();
            if (entity == null)
            {
                var currentCompany = _workContext.CurrentMembershipUser.Company;
                foreach (var item in companies)
                {
                    model.AvailableCompanies.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = currentCompany != null && item.Id == currentCompany.Id });
                }
            }
            else
            {
                foreach (var item in companies)
                {
                    model.AvailableCompanies.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = entity != null && item.Id == entity.Company.Id });
                }
            }

            var healthStatusCategories = _categoryService.GetByParentName("HealthStatus");
            foreach (var item in healthStatusCategories)
            {
                model.AvailableHealthStatus.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity != null && item.Id == entity.HealthStatus.Id });
            }

            var marriedCategories = _categoryService.GetByParentName("MarriedType");
            foreach (var item in marriedCategories)
            {
                model.AvailableMarried.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity != null && item.Id == entity.Married.Id });
            }

            var migrantWorkerCategories = _categoryService.GetByParentName("YesNotType");
            foreach (var item in migrantWorkerCategories)
            {
                model.AvailableMigrantWorker.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity != null && item.Id == entity.MigrantWorker.Id });
            }
        }

        private void ValidateCompanyEmployeeViewModel(CompanyEmployeeViewModel model)
        {
            if (model.IDCard.Length != 18)
            {
                ModelState.AddModelError("IDCard", "无效的身份证号");
            }

            if (model.EndPostDate.HasValue)
            {
                if (model.EndPostDate <= model.StartPostDate)
                {
                    ModelState.AddModelError("EndPostDate", "离岗时间必须大于上岗时间");
                }
            }

            if (model.EntryDate.HasValue && model.LeaveDate.HasValue)
            {
                if (model.LeaveDate <= model.EntryDate)
                {
                    ModelState.AddModelError("LeaveDate", "离职时间必须大于入职时间");
                }
            }

            if (model.EntryDate.HasValue)
            {
                if (model.StartPostDate <= model.EntryDate)
                {
                    ModelState.AddModelError("StartPostDate", "上岗时间不能小于入职时间");
                }
            }

            if (model.LeaveDate.HasValue)
            {
                if (model.LeaveDate <= model.StartPostDate)
                {
                    ModelState.AddModelError("LeaveDate", "离职时间必须大于上岗时间");
                }
            }
        }

        #endregion

        #region Methods

        #region Index
        // GET: EmployeeManage
        public ActionResult Index()
        {

            var model = new CompanyEmployeeSearchViewModel();
            SearchCompanyEmployees(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Index(CompanyEmployeeSearchViewModel model)
        {
            SearchCompanyEmployees(model);
            return View(model);
        }

        public ActionResult SelectEmployeeModal()
        {

            var model = new CompanyEmployeeSearchViewModel();
            SearchCompanyEmployees(model);
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult SelectEmployeeModal(CompanyEmployeeSearchViewModel model)
        {
            SearchCompanyEmployees(model);
            return PartialView(model);
        }

        private void SearchCompanyEmployees(CompanyEmployeeSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                LoadPostStatus(model);
                var healthStatus = _categoryService.GetByParentName("HealthStatus");
                LoadHealthStatus(model, healthStatus);
                var adverseFactors = _companyEmployeeService.GetAdverseFactors();
                LoadAdverseFator(model, _companyEmployeeService.GetAdverseFactors());
                IList<string> departments = _companyEmployeeService.GetDepartments();
                IList<string> workTypes = _companyEmployeeService.GetWorkTypes(WorkContext.CurrentMembershipUser.Company.Id);
                LoadDepartments(model, departments);
                LoadWorkTypes(model, workTypes);

                var searchModel = new CompanyEmployeeSearchModel
                {
                    PageIndex = model.PageIndex,
                    KeyWords = string.IsNullOrEmpty(model.KeyWords) ? "" : model.KeyWords,
                    SelectedAdverseFactor = model.SelectedAdverseFactor,
                    SelectedHealthStatus = model.SelectedHealthStatus,
                    SelectedPostStatus = model.SelectedPostStatus,
                    SeletedWorkType = model.SelectedWorkType,
                    SelectedDepartment = model.SelectedDepartment,
                    CompanyId = WorkContext.CurrentMembershipUser.Company.Id,
                    PageSize = int.MaxValue
                };
                model.Employees = _companyEmployeeService.Search(searchModel);
            }
        }

        private void LoadWorkTypes(CompanyEmployeeSearchViewModel model, IList<string> workTypes)
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

        private void LoadDepartments(CompanyEmployeeSearchViewModel model, IList<string> departments)
        {
            foreach (var item in departments)
            {
                if (item == string.Empty)
                {
                    model.DepartmentList.Add("NULL");
                }
                else
                { model.DepartmentList.Add(item); }
            }
        }

        private void LoadAdverseFator(CompanyEmployeeSearchViewModel model, IList<string> list)
        {
            foreach (var item in list)
            {
                if (item == string.Empty)
                {
                    model.AdverseFactorList.Add("NULL");
                }
                else
                {
                    model.AdverseFactorList.Add(item);
                }
            }
        }

        private void LoadHealthStatus(CompanyEmployeeSearchViewModel model, IList<Category> healthStatus)
        {
            model.HealthStatusList.Add(new SelectListItem { Text = "所有", Value = string.Empty });
            foreach (var item in healthStatus)
            {
                model.HealthStatusList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
        }

        private void LoadPostStatus(CompanyEmployeeSearchViewModel model)
        {
            model.PostStatusList.Add(new SelectListItem { Text = "所有", Value = string.Empty });
            model.PostStatusList.Add(new SelectListItem { Text = "在职", Value = ((int)CompanyEmployeePostStatus.Stay).ToString() });
            model.PostStatusList.Add(new SelectListItem { Text = "离职", Value = ((int)CompanyEmployeePostStatus.Leave).ToString() });

        }
        #endregion

        #region Import
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

                        //导入职业史
                        result.Results.Add(_employeeWorkHistoryService.ImportEmployeeWorkHistories(file.InputStream));

                        //导入员工信息
                        result.Results.Add(_companyEmployeeService.ImportCompanyEmployees(file.InputStream, isCoverData));
                        //执行插入或修改
                        unitOfWork.Commit();
                        //赋值准备导出
                        this.TempData["ImportResult"] = result;
                        var flag = true;
                        foreach (var r in result.Results)
                        {
                            if (r.ErrorRows.Count > 0)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            SuccessNotification("导入员工信息成功！");
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
        public ActionResult ExportCompanyEmployeeManage()
        {
            try
            {
                ImportResultViewModel data = (ImportResultViewModel)this.TempData["ImportResult"];

                var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
                var filePath = Server.MapPath("~/Content/ExportFiles/" + fileName);
                System.IO.File.Copy(Server.MapPath("~/Content/Templates/员工信息模板.xlsx"), filePath);

                _companyEmployeeService.ExportCompanyEmployees(filePath, data.Results);
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

        #region Manage

        public ActionResult CreateOrUpdate(Guid? id = null)
        {
            if (id != null)
            {
                var entity = _companyEmployeeService.GetById(id.Value);
                if (entity != null)
                {
                    var model = new CompanyEmployeeViewModel()
                    {
                        Id = entity.Id,
                        UserName = entity.EmployeeBaseInfo.UserName,
                        IDCard = entity.EmployeeBaseInfo.IDCard,
                        Sex = entity.EmployeeBaseInfo.Sex,
                        AdverseMonthes = entity.AdverseMonthes,
                        TotalWorkMonthes = entity.TotalWorkMonthes,
                        EntryDate = entity.EntryDate,
                        LeaveDate = entity.LeaveDate,
                        StartPostDate = entity.StartPostDate.Value,
                        EndPostDate = entity.EndPostDate,
                        AdverseFactor = entity.AdverseFactor,
                        Comment = entity.Comment,
                        ContactPhone = entity.ContactPhone,
                        Department = entity.Department,
                        Email = entity.Email,
                        ProtectType = entity.ProtectType,
                        WorkNumber = entity.WorkNumber,
                        WorkType = entity.WorkType,
                        CompanyId = entity.Company.Id,
                        HealthStatusId = entity.HealthStatus.Id,
                        MarriedId = entity.Married.Id,
                        MigrantWorkerId = entity.MigrantWorker.Id
                    };
                    PrepareCompanyEmployeeViewModel(model, entity);
                    return View(model);
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，未找到Id为" + id.ToString() + "的员工"));
                    return RedirectToAction("Index");
                }
            }
            else
            {
                var model = new CompanyEmployeeViewModel();
                PrepareCompanyEmployeeViewModel(model, null);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(CompanyEmployeeViewModel model)
        {
            ValidateCompanyEmployeeViewModel(model);

            if (ModelState.IsValid)
            {
                if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        //判断是否有baseinfo
                        var baseInfoEntity = _employeeBaseInfoService.GetByIdCard(model.IDCard);
                        if (baseInfoEntity == null)
                        {
                            baseInfoEntity = new EmployeeBaseInfo();
                            baseInfoEntity.Id = Guid.NewGuid();
                            baseInfoEntity.IDCard = model.IDCard;
                            baseInfoEntity.Sex = model.Sex;
                            baseInfoEntity.UserName = model.UserName;
                            baseInfoEntity.CreatedBy = _workContext.CurrentMembershipUser.Username;
                            baseInfoEntity.CreatetDate = DateTime.Now;
                            _employeeBaseInfoService.Add(baseInfoEntity);
                            unitOfWork.SaveChanges();
                        }
                        else//覆盖掉原来的数据，奇怪的设计
                        {
                            baseInfoEntity.Sex = model.Sex;
                            baseInfoEntity.UserName = model.UserName;
                        }
                        var company = _companyService.GetById(model.CompanyId);
                        //职业史
                        var lastWork = baseInfoEntity.WorkHistories.OrderByDescending(x => x.EntryDate).FirstOrDefault();
                        if (lastWork == null || lastWork.EntryDate > model.StartPostDate)
                        {
                            var workHistory = new EmployeeWorkHistory
                            {
                                WorkType = model.WorkType,
                                Department = model.Department,
                                EntryDate = model.StartPostDate,
                                LeaveDate = model.EndPostDate,
                                AdverseFactor = model.AdverseFactor,
                                CompanyName = company.CompanyName,
                                EmployeeBaseInfo = baseInfoEntity,
                                CreatedBy = WorkContext.CurrentMembershipUser.Username,
                                Deleted = false
                            };
                            _employeeWorkHistoryService.Add(workHistory);
                        }
                        if (model.EndPostDate != null)
                        {
                            if (model.LeaveDate == null)
                            {
                                model.LeaveDate = model.EndPostDate;
                            }
                            if (lastWork != null && model.StartPostDate == lastWork.EntryDate && company.CompanyName == lastWork.CompanyName && model.Department == lastWork.Department && model.WorkType == lastWork.WorkType && model.AdverseFactor == lastWork.AdverseFactor)
                            {
                                lastWork.LeaveDate = model.EndPostDate;
                            }
                        }
                        if (model.EntryDate == null)
                        {
                            model.EntryDate = model.StartPostDate;
                        }

                        //体检状态根据上岗时间及离岗时间自动生成（上岗时间在三个月内自动默认为上岗前，与数据库关联，未做岗前体检的人员超过三个月自动默认为在岗
                        if (model.EndPostDate != null)
                        {
                            model.HealthStatusId = 12;
                        }
                        else
                        {
                            if ((DateTime.Now - Convert.ToDateTime(model.StartPostDate)).Days > 90)
                            {
                                model.HealthStatusId = 11;
                            }
                            else
                            {
                                model.HealthStatusId = 13;
                            }
                        }
                        var entity = new CompanyEmployee()
                        {
                            Id = Guid.NewGuid(),
                            AdverseMonthes = model.AdverseMonthes,
                            TotalWorkMonthes = model.TotalWorkMonthes,
                            EntryDate = model.EntryDate,
                            LeaveDate = model.LeaveDate,
                            StartPostDate = model.StartPostDate,

                            EndPostDate = model.EndPostDate,
                            AdverseFactor = model.AdverseFactor,
                            Comment = model.Comment,
                            ContactPhone = model.ContactPhone,
                            Department = model.Department,
                            Email = model.Email,
                            ProtectType = model.ProtectType,
                            WorkNumber = model.WorkNumber,
                            WorkType = model.WorkType,
                            Company = company,
                            HealthStatus = _categoryService.GetById(model.HealthStatusId),
                            Married = _categoryService.GetById(model.MarriedId),
                            MigrantWorker = _categoryService.GetById(model.MigrantWorkerId),
                            EmployeeBaseInfo = baseInfoEntity,

                            CreatedBy = _workContext.CurrentMembershipUser.Username,
                            CreatedDate = DateTime.Now
                        };

                        //7、总工龄企业未维护空白项的根据职业史中的入岗时间自动生成，接害工龄企业未维护的，根据职业史中危害因素为非空白的记录推算时间，如企业维护时间的按维护时间为准。
                        if (model.TotalWorkMonthes == null)
                        {
                            var earliestWorkHisotory = baseInfoEntity.WorkHistories.OrderBy(x => x.EntryDate).FirstOrDefault();
                            if (earliestWorkHisotory != null)
                            {
                                entity.TotalWorkMonthes = Math.Abs(DateTime.Now.Month - entity.StartPostDate.Value.Month) + 12 * (DateTime.Now.Year - entity.StartPostDate.Value.Year);
                            }
                        }

                        if (model.AdverseMonthes == null)
                        {
                            var workHisotries = baseInfoEntity.WorkHistories.OrderBy(x => x.EntryDate);
                            int monthes = 0;
                            foreach (var item in workHisotries)
                            {
                                if (!string.IsNullOrEmpty(item.AdverseFactor))
                                    monthes += (Math.Abs(item.LeaveDate.Value.Month - item.EntryDate.Value.Month) + 12 * (item.LeaveDate.Value.Year - item.EntryDate.Value.Year));
                            }
                            entity.AdverseMonthes = monthes;
                        }
                        _companyEmployeeService.Add(entity);
                        unitOfWork.Commit();

                        SuccessNotification("添加成功");
                        PrepareCompanyEmployeeViewModel(model, entity);
                        return View(model);
                    }
                }
                else
                {
                    var entity = _companyEmployeeService.GetById(model.Id);
                    if (entity != null)
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            //判断是否有baseinfo，修改状态下应该是有的
                            var baseInfoEntity = _employeeBaseInfoService.GetByIdCard(model.IDCard);
                            if (baseInfoEntity == null)
                            {
                                baseInfoEntity = new EmployeeBaseInfo();
                                baseInfoEntity.Id = Guid.NewGuid();
                                baseInfoEntity.IDCard = model.IDCard;
                                baseInfoEntity.Sex = model.Sex;
                                baseInfoEntity.UserName = model.UserName;
                                baseInfoEntity.CreatedBy = _workContext.CurrentMembershipUser.Username;
                                baseInfoEntity.CreatetDate = DateTime.Now;
                                _employeeBaseInfoService.Add(baseInfoEntity);
                            }
                            else//覆盖掉原来的数据，奇怪的设计
                            {
                                baseInfoEntity.Sex = model.Sex;
                                baseInfoEntity.UserName = model.UserName;
                            }

                            entity.AdverseMonthes = model.AdverseMonthes;
                            entity.TotalWorkMonthes = model.TotalWorkMonthes;
                            entity.EntryDate = model.EntryDate;
                            entity.LeaveDate = model.LeaveDate;
                            if (model.EntryDate == null)
                            {
                                entity.EntryDate = model.StartPostDate;
                            }

                            entity.StartPostDate = model.StartPostDate;
                            var company = _companyService.GetById(model.CompanyId);

                            var lastWork = baseInfoEntity.WorkHistories.OrderByDescending(x => x.EntryDate).FirstOrDefault();
                            if (lastWork == null || lastWork.EntryDate > model.StartPostDate)
                            {
                                var workHistory = new EmployeeWorkHistory
                                {
                                    WorkType = model.WorkType,
                                    Department = model.Department,
                                    EntryDate = model.StartPostDate,
                                    LeaveDate = model.EndPostDate,
                                    AdverseFactor = model.AdverseFactor,
                                    CompanyName = company.CompanyName,
                                    EmployeeBaseInfo = baseInfoEntity,
                                    CreatedBy = WorkContext.CurrentMembershipUser.Username,
                                    Deleted = false
                                };
                                _employeeWorkHistoryService.Add(workHistory);
                            }
                            if (model.EndPostDate != null)
                            {
                                if (model.LeaveDate == null)
                                {
                                    entity.LeaveDate = model.EndPostDate;
                                }


                                if (lastWork != null && model.StartPostDate == lastWork.EntryDate && entity.Company.CompanyName == lastWork.CompanyName)
                                {
                                    lastWork.LeaveDate = model.EndPostDate;
                                }
                            }
                            entity.EndPostDate = model.EndPostDate;

                            entity.AdverseFactor = model.AdverseFactor;
                            entity.Comment = model.Comment;
                            entity.ContactPhone = model.ContactPhone;
                            entity.Department = model.Department;
                            entity.Email = model.Email;
                            entity.ProtectType = model.ProtectType;
                            entity.WorkNumber = model.WorkNumber;
                            entity.WorkType = model.WorkType;
                            entity.Company = company;

                            //体检状态根据上岗时间及离岗时间自动生成（上岗时间在三个月内自动默认为上岗前，与数据库关联，未做岗前体检的人员超过三个月自动默认为在岗
                            if (entity.EndPostDate != null)
                            {
                                model.HealthStatusId = 12;
                            }
                            else
                            {
                                if ((DateTime.Now - Convert.ToDateTime(model.StartPostDate)).Days > 90)
                                {
                                    model.HealthStatusId = 11;
                                }
                                else
                                {
                                    model.HealthStatusId = 13;
                                }
                            }

                            entity.HealthStatus = _categoryService.GetById(model.HealthStatusId);
                            entity.Married = _categoryService.GetById(model.MarriedId);
                            entity.MigrantWorker = _categoryService.GetById(model.MigrantWorkerId);
                            entity.EmployeeBaseInfo = baseInfoEntity;

                            entity.UpdatedBy = _workContext.CurrentMembershipUser.Username;
                            entity.UpdatedDate = DateTime.Now;

                            unitOfWork.Commit();

                            SuccessNotification("编辑成功");
                            PrepareCompanyEmployeeViewModel(model, entity);
                            return View(model);
                        }
                    }
                    else
                    {
                        ErrorNotification(new Exception("编辑失败，未找到Id为" + model.Id.ToString() + "的员工"));
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                ErrorNotification(new Exception("编辑失败，输入信息有误"));
                PrepareCompanyEmployeeViewModel(model, null);
                return View(model);
            }
        }

        public ActionResult Delete(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var entity = _companyEmployeeService.GetById(id);
                if (entity != null)
                {
                    entity.Deleted = true;
                    unitOfWork.Commit();
                    SuccessNotification("删除成功");
                }
                else
                {
                    ErrorNotification(new Exception("删除失败，未找到Id为" + id.ToString() + "的员工"));
                }
                return RedirectToAction("Index");
            }
        }

        #endregion

        #endregion
    }
}