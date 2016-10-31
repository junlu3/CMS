using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Web.Models;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using System.IO;

namespace XL.CHC.Web.Controllers
{
    public class MembershipUserController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICategoryService _categoryService;
        private readonly IMembershipService _membershipService;
        private readonly ICompanyService _companyService;
        private readonly IWorkContext _workContext;

        public MembershipUserController(IAuthenticationService authenticationService, ICategoryService categoryService,
            IMembershipService membershipService, ICompanyService companyService, IWorkContext workContext)
        {
            this._authenticationService = authenticationService;
            this._membershipService = membershipService;
            this._companyService = companyService;
            this._categoryService = categoryService;
            this._workContext = workContext;
        }

        #region Manage

        public ActionResult Manage()
        {
            var model = new MembershipUserManageViewModel();
            SearchMembershipUsers(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Manage(MembershipUserManageViewModel model)
        {
            SearchMembershipUsers(model);
            return View(model);
        }

        private void SearchMembershipUsers(MembershipUserManageViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var searchModel = new MembershipUserSearchModel
                {
                    KeyWord = model.KeyWord,
                    PageIndex = model.PageIndex,
                    Roles = WorkContext.CurrentMembershipUser.MembershipRoles
                };
                model.MembershipUsers = MembershipService.SearchUser(searchModel);
            }
        }

        #endregion

        #region Eidt & Create

        public ActionResult Create()
        {
            var model = new MembershipUserEditViewModel();
            PrepareUserCreateViewModel(model,null);
            return View(model);

        }
        [HttpPost]
        public ActionResult Create(MembershipUserEditViewModel model)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = _membershipService.GetUser(model.UserName);
                    if (user != null)
                    {
                        throw new Exception("用户名已被使用");
                    }
                    user = new MembershipUser();
                    user.Username = model.UserName;
                    user.Email = model.Email;
                    user.Password = "123456";
                    user.Deleted = false;
                    user.IsActived = true;

                    Guid role_Id = Guid.Parse(model.Role_Id);
                    var role = _membershipService.GetRole(role_Id);
                    user.MembershipRoles = new List<MembershipRole>();
                    user.MembershipRoles.Add(role);

                    Guid company_Id = Guid.Parse(model.Company_Id);
                    var company = _companyService.GetById(company_Id);
                    user.Company = company;

                    MembershipService.AddUser(user);
                    unitOfWork.Commit();
                    SuccessNotification(string.Format("用户{0}创建成功！", model.UserName));
                    return RedirectToAction("Manage");
                }
            }
            catch (Exception ex)
            {
                PrepareUserCreateViewModel(model, null);
                ErrorNotification(ex);
                return View(model);
            }
        }

        public ActionResult Edit(Guid id)
        {
            var model = new MembershipUserEditViewModel();
            model.Id = id;

            var user = MembershipService.GetUser(id);
            if (user == null)
            {
                ErrorNotification(new Exception("未能找到该用户！"));
                return RedirectToAction("Manage");
            }
            else
            {
                model.UserName = user.Username;
                model.Email = user.Email;
                PrepareUserCreateViewModel(model, user);
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult Edit(MembershipUserEditViewModel model)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = MembershipService.GetUser(model.Id.Value);
                    if (user != null)
                    {
                        if (user.Username != model.UserName)
                        {
                            var user2 = MembershipService.GetUser(model.UserName);
                            if (user2 != null)
                            {
                                throw new Exception("该用户名已被占用!");
                            }
                        }

                        user.Username = model.UserName;
                        user.Email = model.Email;
                        if (user.Company.Id.ToString() != model.Company_Id)
                        {
                            Guid company_Id = Guid.Parse(model.Company_Id);
                            var company = _companyService.GetById(company_Id);
                            user.Company = company;
                        }

                        if (!user.MembershipRoles.Any(x=>x.Id.ToString() == model.Role_Id))
                        {
                            user.MembershipRoles.Clear();

                            Guid role_Id = Guid.Parse(model.Role_Id);
                            var role = MembershipService.GetRole(role_Id);
                            user.MembershipRoles.Add(role);
                        }
                        

                        unitOfWork.Commit();
                        PrepareUserCreateViewModel(model, user);

                        SuccessNotification("编辑成功");
                        return View(model);
                    }
                    else
                    {
                        throw new Exception("未能找到该用户！");
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Edit",new { id=model.Id });
            }
            
        }

        [HttpPost]
        public JsonResult GetRoles(Guid? id)
        {
            try
            {
                if (id != null && id != new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    var roles = _membershipService.GetAllRolesByCompany(id.Value);
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (var item in roles)
                    {
                        list.Add(new SelectListItem { Text=item.Name,Value = item.Id.ToString(),Selected=false });
                    }
                    var json = new JsonResult {
                        Data = list,
                    };
                    return json;
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception)
            {
                return Json("");
            }
        }
        private void PrepareUserCreateViewModel(MembershipUserEditViewModel model,MembershipUser entity)
        {
            List<SelectListItem> companylist = new List<SelectListItem>();
            var companys = _companyService.GetAll();
            if (entity == null)
            {
                foreach (var item in companys)
                {
                    companylist.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = (item.Id == WorkContext.CurrentMembershipUser.Company.Id) });
                }
            }
            else
            {
                foreach (var item in companys)
                {
                    companylist.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = (item.Id == entity.Company.Id) });
                }
            }
            model.Companys = companylist;

            List<SelectListItem> roleList = new List<SelectListItem>();
            
            if (entity == null )
            {
                
                var company_Id = string.IsNullOrEmpty(model.Company_Id) ? WorkContext.CurrentMembershipUser.Company.Id : Guid.Parse(model.Company_Id);
                var roles = _membershipService.GetAllRolesByCompany(company_Id);
                foreach (var item in roles)
                {
                    roleList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = !string.IsNullOrEmpty(model.Role_Id) && Guid.Parse(model.Role_Id) == item.Id });
                }
            }
            else
            {
                
                var roles = _membershipService.GetAllRolesByCompany(entity.Company.Id);
                foreach (var item in roles)
                {
                    roleList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity.MembershipRoles.Any(x=>x.Id== item.Id) });
                }
            }
            model.Roles = roleList;
        }
        #endregion 

        #region  Login

        // GET: MembershipUser
        [AllowAnonymous]
        public ActionResult Login()
        {
            _authenticationService.SignOut();
            WorkContext.RemoveMembershipUser();
            WorkContext.ClearMenuItems();
            var model = new MembershipUserLogonViewModel();
            model.ReturnUrl = Request["ReturnUrl"];
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(MembershipUserLogonViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = MembershipService.ValidateUser(model.UserName, model.Password);
                    if (user != null)
                    {
                        try
                        {
                            _authenticationService.SignIn(user, model.RememberMe);
                            WorkContext.CurrentMembershipUser = user;
                            user.LastLoginDate = DateTime.Now;
                            unitOfWork.Commit();
                            ////to do 企业用户未激活的需要先完善资料
                            //if (user.MembershipRoles.Where(x => x.Id == new Guid("04544256-4A44-4876-9C30-3186A71CC862")).Count() > 0)
                            //{
                            //    if (user.IsActived == false)
                            //    {
                            //        return RedirectToAction("ActiveEnterpriseUser", "MembershipUser");
                            //    }
                            //}

                            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                                return Redirect(model.ReturnUrl);
                            else
                                return RedirectToAction("Index", "Specification");
                        }
                        catch (Exception ex)
                        {
                            model.ErrorMessage = ex.Message;
                        }
                    }
                    else
                    {
                        model.ErrorMessage = "用户名或者密码错误！";
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region 企业账户

        public ActionResult ActiveEnterpriseUser()
        {
            var model = new ActiveEnterpriseUserViewModel();
            InitActiveEnterpriseUserViewModel(model);
            return View(model);
        }

        private void InitActiveEnterpriseUserViewModel(ActiveEnterpriseUserViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var cats = _categoryService.GetByParentName("CompanyType");
                foreach (var item in cats)
                {
                    model.CompanyTypeList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
                cats = _categoryService.GetByParentName("CompanyRegisterType");
                foreach (var item in cats)
                {
                    model.CompanyRegisterTypeList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
        }
        [HttpPost]
        public ActionResult ActiveEnterpriseUser(ActiveEnterpriseUserViewModel model, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                var user = WorkContext.CurrentMembershipUser;
                Guid userId = user.Id;
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var stampPath = string.Empty;
                    var file = Request.Files["ImportFile"];
                    var fileName = "";
                    if (!string.IsNullOrEmpty(model.StampFilePath) && file != null && file.ContentLength > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!(fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg" || fileExtension == ".bmp"))
                        {
                            ErrorNotification(new Exception("只能上传图片类型的文档（后缀名为：.jpg/.png/.jpeg/.bmp）"));
                            return View(model);
                        }
                        else
                        {
                            fileName = DateTime.Now.Ticks.ToString() + fileExtension;
                            stampPath = Server.MapPath("~/Content/Images/CompanyStamps/" + fileName);
                            file.SaveAs(stampPath);
                        }
                    }

                    try
                    {
                        var currentUser = MembershipService.GetUser(userId);
                        var company = new Company
                        {
                            CompanyName = model.CompanyName,
                            CompanyAddress = model.Address,
                            Telephone = model.Telephone,
                            Fax = model.Fax,
                            ZipCode = model.PostCode,
                            RestDay = model.RestDay,
                            RegisterDate = model.RegisterDate,
                            CompanyType = _categoryService.GetById(model.CompanyType),
                            CompanyRegisterType = _categoryService.GetById(model.CompanyRegisterType),
                            //MembershipUser = currentUser,
                            IsDefault = true,
                            CompanyStamp = fileName == string.Empty ? string.Empty : "/Content/Images/CompanyStamps/" + fileName
                        };
                        _companyService.Add(company);
                        currentUser.IsActived = true;
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {

                        unitOfWork.Rollback();

                        ErrorNotification(ex);
                        InitActiveEnterpriseUserViewModel(model);
                        return View(model);
                    }

                }
            }
            return RedirectToAction("Index", "CompanyEmployeeManage");
        }

        #endregion

        #region Profile

        public ActionResult PersonalInfo(Guid id)
        {
            try
            {
                var model = new MembershipUserPersonalInfoViewModel();
                var user = _membershipService.GetUser(id);

                model.Id = user.Id;
                var defaultCompany = user.Company;
                if (defaultCompany != null)
                {
                    model.DefaultCompanyId = user.Company.Id;
                }
                model.Email = user.Email;
                model.SelectedEmailTaskTypeIds = (user.EmailTaskTypes != null && user.EmailTaskTypes.Count > 0) ? user.EmailTaskTypes.Select(e => e.Id).ToList() : new List<int>();

                model.AvailableCompanies = new List<Company> { WorkContext.CurrentMembershipUser.Company };
                model.AvailableEmailTaskTypes = _categoryService.GetByParentName("EmailTaskType");

                return View(model);
            }
            catch (Exception e)
            {
                ErrorNotification(new Exception(e.Message));
                return View(new MembershipUserPersonalInfoViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalInfo(MembershipUserPersonalInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = _membershipService.GetUser(model.Id);
                    //foreach (var co in user.Companies)
                    //{
                    //    if (co.Id == model.DefaultCompanyId)
                    //    {
                    //        co.IsDefault = true;
                    //    }
                    //    else
                    //    {
                    //        co.IsDefault = false;
                    //    }
                    //}
                    user.Email = model.Email;
                    user.EmailTaskTypes.Clear();
                    var allEmailTaskTypes = _categoryService.GetByParentName("EmailTaskType");
                    foreach (var type in allEmailTaskTypes)
                    {
                        if (model.SelectedEmailTaskTypeIds.Contains(type.Id))
                        {
                            user.EmailTaskTypes.Add(type);
                        }
                    }

                    unitOfWork.Commit();

                    model.AvailableCompanies = new List<Company> { WorkContext.CurrentMembershipUser.Company };
                    model.AvailableEmailTaskTypes = allEmailTaskTypes;

                    SuccessNotification("修改成功");
                    return View(model);
                }
            }
            else
            {
                ErrorNotification(new Exception("无效的模型状态"));
                return RedirectToAction("PersonalInfo", new { @id = model.Id });
            }
        }

        #endregion

        #region Logout
        [AllowAnonymous]
        public ActionResult Logout()
        {

            return RedirectToAction("Login", "MembershipUser");
        }
        #endregion

        #region Company

        #region Utilities

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

        private void ValidateCompanyViewModel(CompanyViewModel model)
        {
            if (string.IsNullOrEmpty(model.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "名称不能为空");
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

        #region Methods

        public ActionResult CompanyCreateOrUpdate(Guid membershipUserId, Guid? id = null)
        {
            if (id != null)
            {
                var entity = _companyService.GetById(id.Value);
                if (entity != null)
                {
                    var model = new CompanyViewModel()
                    {
                        MembershipUserId = membershipUserId,
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
                    ErrorNotification(new Exception("编辑失败，未找到Id为" + id.ToString() + "的公司"));
                    return RedirectToAction("PersonalInfo", "MembershipUser", new { @id = membershipUserId });
                }
            }
            else
            {
                var model = new CompanyViewModel()
                {
                    MembershipUserId = membershipUserId,
                };
                PrepareCompanyViewModel(model, null);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyCreateOrUpdate(CompanyViewModel model)
        {
            ValidateCompanyViewModel(model);

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

                            CreatedBy = _workContext.CurrentMembershipUser.Username,
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
                        ErrorNotification(new Exception("编辑失败，未找到Id为" + model.Id.ToString() + "的公司"));
                        return RedirectToAction("PersonalInfo", "MembershipUser", new { @id = model.MembershipUserId });
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

        #endregion

        #endregion

        #region 修改密码

        #region Utilities

        private void ValidateEditPasswordViewModel(EditPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "旧密码不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "新密码不能为空");
            }

            if (string.IsNullOrWhiteSpace(model.RepeatNewPassword))
            {
                ModelState.AddModelError("RepeatNewPassword", "重复密码不能为空");
            }

            if (model.NewPassword != model.RepeatNewPassword)
            {
                ModelState.AddModelError("NewPassword", "新密码不一致");
            }
        }

        #endregion

        #region Methods

        public ActionResult EditPassword(string username)
        {
            var entity = _membershipService.GetUser(username);
            if (entity != null)
            {
                var model = new EditPasswordViewModel()
                {
                    Username = entity.Username,
                };
                return View(model);
            }
            else
            {
                ErrorNotification(new Exception("未找到用户"));
                return RedirectToAction("MainPage", "Employee");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(EditPasswordViewModel model)
        {
            try
            {
                ValidateEditPasswordViewModel(model);
                
                if (ModelState.IsValid)
                {
                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        var user = MembershipService.ValidateUser(model.Username, model.OldPassword);
                        if (user != null)
                        {
                            user.Password = XL.Utilities.MD5Helper.MD5Encrypt(model.NewPassword);
                            unitOfWork.Commit();
                            SuccessNotification("修改成功");
                        }
                        else
                        {
                            ErrorNotification(new Exception("旧密码错误，请重试"));
                        }
                        return View(model);
                    }
                }
                else
                {
                    ErrorNotification(new Exception("修改失败，请检查输入项"));
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("修改失败，原因是: " + ex.Message));
                return View(model);
            }
        }

        #endregion

        #endregion
    }
}