using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class CompanyManageController : BaseController
    {

        #region Fields
        private readonly ICompanyService _companyService;
        private readonly ICategoryService _categoryService;
        #endregion

        public CompanyManageController(ICompanyService companyService,ICategoryService categoryService)
        {
            _companyService = companyService;
            _categoryService = categoryService;
        }

        #region Action
        public ActionResult Index()
        {
            try
            {
                var model = new CompanyManageViewModel();
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }

        }
        [HttpPost]
        public ActionResult Index(CompanyManageViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new CompanyManageViewModel();
                SearchOrders(model);
                ErrorNotification(ex);
                return View(model);
            }
        }

        public ActionResult CreateOrUpdate(Guid? id = null)
        {
            try
            {
                if (id != null)
                {
                    var entity = _companyService.GetById(id.Value);
                    if (entity != null)
                    {
                        var model = new CompanyViewModel {
                            MembershipUserId = WorkContext.CurrentMembershipUser.Id,
                            Id = entity.Id,
                            CompanyAddress = entity.CompanyAddress,
                            CompanyName = entity.CompanyName,
                            Telephone = entity.Telephone,
                            ContactPerson = entity.ContactPerson,
                            ContactPhone = entity.ContactPhone,
                            Fax = entity.Fax,
                            LegalPerson = entity.LegalPerson,
                            RestDay = entity.RestDay,
                            ZipCode = entity.ZipCode,
                            RegisterDate = entity.RegisterDate,
                            CompanyTypeId = entity.CompanyType == null ? 0 : entity.CompanyType.Id,
                            CompanyRegisterTypeId = entity.CompanyRegisterType == null ? 0 : entity.CompanyRegisterType.Id,
                            CompanyStamp = entity.CompanyStamp,
                        };
                        PrepareCompanyViewModel(model, entity);
                        return View(model);
                    }
                    else
                    {
                        ErrorNotification(new Exception("加载失败，未找到该企业"));
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    CompanyViewModel model = new CompanyViewModel();
                    model.MembershipUserId = WorkContext.CurrentMembershipUser.Id;
                    PrepareCompanyViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("企业编辑页面加载失败"+ ex.Message));
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(CompanyViewModel model)
        {
            if (string.IsNullOrEmpty(model.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "名称不能为空");
            }
            if (ModelState.IsValid)
            {
                if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        var file = Request.Files["importfile"];
                        var fileName = "";
                        if (!string.IsNullOrEmpty(model.CompanyStamp) && file != null && file.ContentLength > 0)
                        {
                            var fileExtension = Path.GetExtension(file.FileName).ToLower();
                            if (!IsValidImage(fileExtension))
                            {
                                ErrorNotification(new Exception("只能上传后缀为jpg、gif或png类型的图片"));
                                return View(model);
                            }
                            else
                            {
                                fileName = Guid.NewGuid().ToString() + fileExtension;
                                file.SaveAs(Server.MapPath("~/Content/Images/CompanyStamps/" + fileName));
                            }
                        }

                        var entity = new Company()
                        {
                            Id = Guid.NewGuid(),
                            CompanyAddress = model.CompanyAddress,
                            CompanyName = model.CompanyName,
                            Telephone = model.Telephone,
                            ContactPerson = model.ContactPerson,
                            ContactPhone = model.ContactPhone,
                            Fax = model.Fax,
                            LegalPerson = model.LegalPerson,
                            RestDay = model.RestDay,
                            ZipCode = model.ZipCode,
                            RegisterDate = model.RegisterDate,
                            CompanyType = model.CompanyTypeId == 0 ? null : _categoryService.GetById(model.CompanyTypeId),
                            CompanyRegisterType = model.CompanyRegisterTypeId == 0 ? null : _categoryService.GetById(model.CompanyRegisterTypeId),
                            CompanyStamp = string.IsNullOrEmpty(fileName) ? "" : "/Content/Images/CompanyStamps/" + fileName,
                            //MembershipUser = _membershipService.GetUser(model.MembershipUserId),

                            CreatedBy = WorkContext.CurrentMembershipUser.Username,
                            CreatedDate = DateTime.Now
                        };
                        _companyService.Add(entity);
                        unitOfWork.Commit();

                        SuccessNotification("添加成功");
                        PrepareCompanyViewModel(model, entity);
                        return View(model);
                    }
                }
                else
                {
                    var entity = _companyService.GetById(model.Id);
                    if (entity != null)
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            entity.CompanyAddress = model.CompanyAddress;
                            entity.CompanyName = model.CompanyName;
                            entity.Telephone = model.Telephone;
                            entity.ContactPerson = model.ContactPerson;
                            entity.ContactPhone = model.ContactPhone;
                            entity.Fax = model.Fax;
                            entity.LegalPerson = model.LegalPerson;
                            entity.RestDay = model.RestDay;
                            entity.ZipCode = model.ZipCode;
                            entity.RegisterDate = model.RegisterDate;
                            entity.CompanyType = model.CompanyTypeId == 0 ? null : _categoryService.GetById(model.CompanyTypeId);
                            entity.CompanyRegisterType = model.CompanyRegisterTypeId == 0 ? null : _categoryService.GetById(model.CompanyRegisterTypeId);

                            var file = Request.Files["importfile"];
                            if (!string.IsNullOrEmpty(model.CompanyStamp) && file != null && file.ContentLength > 0)
                            {
                                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                                if (!IsValidImage(fileExtension))
                                {
                                    ErrorNotification(new Exception("只能上传后缀为jpg、gif或png类型的图片"));
                                    return View(model);
                                }
                                else
                                {
                                    var fileName = Guid.NewGuid() + fileExtension;
                                    file.SaveAs(Server.MapPath("~/Content/Images/CompanyStamps/" + fileName));
                                    entity.CompanyStamp = "/Content/Images/CompanyStamps/" + fileName;
                                }
                            }
                            else
                            {
                                model.CompanyStamp = "";
                                entity.CompanyStamp = "";
                            }

                            unitOfWork.Commit();

                            SuccessNotification("编辑成功");
                            PrepareCompanyViewModel(model, entity);
                            return View(model);
                        }
                    }
                    else
                    {
                        ErrorNotification(new Exception("编辑失败，未找到对应的公司"));
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                ErrorNotification(new Exception("编辑失败，输入信息有误"));
                PrepareCompanyViewModel(model, null);
                return View(model);
            }
        }

        public ActionResult CompanyDelete(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var entity = _companyService.GetById(id);
                if (entity != null)
                {
                    if (entity.IsDefault)
                    {
                        ErrorNotification(new Exception("删除失败，不能删除默认公司"));
                    }
                    else
                    {
                        entity.Deleted = true;
                        unitOfWork.Commit();
                        SuccessNotification("删除成功");
                    }
                }
                else
                {
                    ErrorNotification(new Exception("删除失败，未找到Id为" + id.ToString() + "的公司"));
                }
                return RedirectToAction("Index", "CompanyManage");
            }
        }
        #endregion

        #region Manage
        private void SearchOrders(CompanyManageViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var searchModel = new CompanySearchModel
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    KeyWord = model.KeyWord
                };
                model.ViewList = _companyService.Search(searchModel);
            }
        }

        private void PrepareCompanyViewModel(CompanyViewModel model, Company entity)
        {
            var companyRegisterTypes = _categoryService.GetByParentName("CompanyRegisterType");
            foreach (var item in companyRegisterTypes)
            {
                if (item.Id.ToString() == "35")
                {
                    model.AvailableCompanyRegisterTypes.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity == null || entity.CompanyRegisterType == null || entity.CompanyRegisterType.Id == item.Id });
                }
                else
                {
                    model.AvailableCompanyRegisterTypes.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity != null && entity.CompanyRegisterType != null && item.Id == entity.CompanyRegisterType.Id });
                }
            }

            var companyTypes = _categoryService.GetByParentName("CompanyType");
            foreach (var item in companyTypes)
            {
                if (item.Id.ToString() == "36")
                {
                    model.AvailableCompanyTypes.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity == null || entity.CompanyRegisterType == null || entity.CompanyRegisterType.Id == item.Id });
                }
                else
                {
                    model.AvailableCompanyTypes.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity != null && entity.CompanyType != null && item.Id == entity.CompanyType.Id });
                }
            }

            if (entity != null)
            {
                model.CompanyStamp = entity.CompanyStamp;
            }
        }

        /// <summary>
        /// 判断后缀是否为识别的图片
        /// </summary>
        /// <param name="fileExtention">带“.”</param>
        /// <returns></returns>
        private bool IsValidImage(string fileExtention)
        {
            switch (fileExtention)
            {
                case ".jpg":
                case ".png":
                case ".gif":
                    return true;
                default:
                    return false;
            }
        }
        #endregion
    }
}