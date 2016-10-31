using System;
using System.Web.Mvc;
using XL.CHC.Web.Models;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace XL.CHC.Web.Controllers
{
    public class CompanyUserManageController : BaseController
    {
        private readonly ICompanyService _companyService;

        public CompanyUserManageController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public ActionResult Index()
        {
            var model = new CompanyUserSearchViewModel();
            SearchUsers(model);
            return View(model);
        }

        private void SearchUsers(CompanyUserSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var searchModel = new MembershipUserSearchModel
                {
                    PageIndex = model.PageIndex,
                    KeyWord = model.KeyWord,
                    Roles = WorkContext.CurrentMembershipUser.MembershipRoles,
                    Company_Id = WorkContext.CurrentMembershipUser.Company.Id
                };
                model.Users = MembershipService.SearchUser(searchModel);
            }
        }

        [HttpPost]
        public ActionResult Index(CompanyUserSearchViewModel model)
        {
            SearchUsers(model);
            return View(model);
        }

        public ActionResult CreateCompanyAccount()
        {
            var model = new CompanyUserCreateViewModel();

            PrepareCompanyUserViewModel(model, null);

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCompanyAccount(CompanyUserCreateViewModel model)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    Guid roleId = Guid.Parse(model.Role_Id);
                    var role = MembershipService.GetRole(roleId);
                    var user = MembershipService.GetUser(model.AccountName);
                    var company = _companyService.GetById(WorkContext.CurrentMembershipUser.Company.Id);
                    if (user != null && user.Deleted == true)
                    {
                        user.Deleted = false;
                        user.IsActived = true;
                        user.Password = Utilities.MD5Helper.MD5Encrypt(model.Password);
                        user.Email = model.Email;
                        user.Company = company;
                        user.MembershipRoles.Clear();
                        user.MembershipRoles.Add(role);
                        unitOfWork.SaveChanges();

                        PrepareCompanyUserViewModel(model,user);
                    }
                    else if (user != null && user.Deleted == false)
                    {
                        throw new Exception("该用户名已被占用!");
                    }
                    else
                    {
                        user = new MembershipUser();
                        user.Username = model.AccountName;
                        user.Password = model.Password;
                        user.Email = model.Email;
                        user.Company = company;
                        user.IsActived = true;
                        user.MembershipRoles.Add(role);
                        MembershipService.AddUser(user);

                        PrepareCompanyUserViewModel(model, user);
                    }
                    unitOfWork.Commit();

                    SuccessNotification("创建成功！");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                PrepareCompanyUserViewModel(model, null);
                ErrorNotification(ex);
                return View(model);
            }
            
        }


        public ActionResult EditCompanyAccount(Guid id)
        {
            var model = new CompanyUserCreateViewModel();

            var user = MembershipService.GetUser(id);
            if (user != null)
            {
                model.Id = user.Id;
                model.AccountName = user.Username;
                model.Email = user.Email;
                model.Password = Utilities.MD5Helper.MD5Decrypt(user.Password);
                PrepareCompanyUserViewModel(model, user);
                return View(model);
            }
            else
            {
                ErrorNotification(new Exception("未能找到该用户！"));
                return RedirectToAction("Index");
            }
           
        }

        [HttpPost]
        public ActionResult EditCompanyAccount(CompanyUserCreateViewModel model)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = MembershipService.GetUser(model.Id.Value);
                    if (user == null)
                    {
                        throw new Exception("未能找到该用户！");
                    }

                    if (user.Username != model.AccountName)
                    {
                        var user2 = MembershipService.GetUser(model.AccountName);
                        if (user2 != null)
                        {
                            throw new Exception("该用户名已被占用!");
                        }
                    }
                    
                    user.Username = model.AccountName;
                    user.Password = Utilities.MD5Helper.MD5Encrypt(model.Password);
                    user.Email = model.Email;

                    Guid roleId = Guid.Parse(model.Role_Id);
                    if (!user.MembershipRoles.Any(x=>x.Id == roleId))
                    {
                        var role = MembershipService.GetRole(roleId);
                        user.MembershipRoles.Clear();
                        user.MembershipRoles.Add(role);
                    }

                    unitOfWork.SaveChanges();
                    unitOfWork.Commit();

                    PrepareCompanyUserViewModel(model, user);
                    SuccessNotification("修改成功！");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("EditCompanyAccount", new { id=model.Id});
            }
        }


        public ActionResult DeleteCompanyAccount(Guid id)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var user = MembershipService.GetUser(id);
                    user.Deleted = true;
                    user.MembershipRoles.Clear();

                    unitOfWork.SaveChanges();
                    unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }
            return RedirectToAction("Index");
        }

        #region 
        private void PrepareCompanyUserViewModel(CompanyUserCreateViewModel model,MembershipUser entity)
        {
            IList<SelectListItem> selList = new List<SelectListItem>();
            IList<MembershipRole> roles = MembershipService.GetAllRolesByCompany(WorkContext.CurrentMembershipUser.Company.Id);

            if (entity == null)
            {
                foreach (var item in roles)
                {
                    selList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() ,Selected =false });
                }
            }
            else
            {
                foreach (var item in roles)
                {
                    selList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString(), Selected = entity.MembershipRoles.Any(x=>x.Id == item.Id) });
                }
            }

            model.MembershipRoles = selList;

        }
        #endregion
    }
}