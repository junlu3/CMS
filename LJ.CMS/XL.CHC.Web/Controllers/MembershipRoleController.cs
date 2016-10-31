using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Web.Models;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.DomainModel;

namespace XL.CHC.Web.Controllers
{
    public class MembershipRoleController : BaseController
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IMembershipService _membershipService;
        private readonly ICompanyService _companyService;

        public MembershipRoleController(IMenuItemService menuItemService,IMembershipService membershipService,ICompanyService companyService)
        {
            _menuItemService = menuItemService;
            _membershipService = membershipService;
            _companyService = companyService;
        }
        // GET: MembershipRole
        public ActionResult Permission()
        {
            var model = new PermissionViewModel();
            GeneratePermissions(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Permission(FormCollection form)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var menuItems = _menuItemService.GetAll();
                    var membershipRoles = MembershipService.GetAllRoles();
                    foreach (var mr in membershipRoles)
                    {
                        string formKey = "allow_" + mr.Id;
                        var menuItemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                        foreach (var mi in menuItems)
                        {
                            bool allow = menuItemNamesToRestrict.Contains(mi.MenuName);
                            bool hasPr = false;
                            if (allow)
                            {
                                foreach (var tmpPr in mr.MenuItems)
                                {
                                    if (tmpPr.Id == mi.Id)
                                    {
                                        hasPr = true;
                                        break;
                                    }
                                }
                                if (!hasPr)
                                {
                                    mr.MenuItems.Add(mi);
                                }
                            }
                            else
                            {
                                foreach (var tempMi in mr.MenuItems)
                                {
                                    if (tempMi.Id == mi.Id)
                                    {
                                        hasPr = true;
                                        break;
                                    }
                                }
                                if (hasPr)
                                {
                                    mr.MenuItems.Remove(mi);
                                }
                            }
                        }
                    }

                    unitOfWork.Commit();
                    SuccessNotification("修改成功");

                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }


            return RedirectToAction("Permission", "MembershipRole");
        }
        private void GeneratePermissions(PermissionViewModel model)
        {
            MembershipUser user = WorkContext.CurrentMembershipUser;


                if (WorkContext.CurrentMembershipUser.MembershipRoles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")))
                {
                    #region 加载超级管理员菜单
                    model.MenuItems = _menuItemService.GetAll();
                    model.MembershipRoles = MembershipService.GetAllRoles();
                    #endregion
                }
                else
                {
                    #region 加载拥有权限的菜单
                    List<MenuItem> list = new List<MenuItem>();
                    foreach (var item in user.MembershipRoles)
                    {
                        list.AddRange(item.MenuItems);
                    }
                    model.MenuItems = list;
                    #endregion

                    #region 加载当前公司的角色
                    model.MembershipRoles = WorkContext.CurrentMembershipUser.Company.MembershipRoles;
                    #endregion
                }

        }

        public ActionResult RoleManage()
        {
            try
            {
                var model = new MemberShipRoleSearchViewModel();
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
        public ActionResult RoleManage(MemberShipRoleSearchViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new MemberShipRoleSearchViewModel();
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
                    var entity = _membershipService.GetRole(id.Value);
                    if (entity != null)
                    {
                        var model = new MemberShipRoleViewModel {
                            Id = entity.Id,
                            Name = entity.Name,
                            Company = entity.Company
                        };
                        PrepareRoleViewModel(model,entity);
                        return View(model);
                    }
                    else
                    {
                        ErrorNotification(new Exception("加载失败，未找到该角色"));
                        return RedirectToAction("RoleManage");
                    }
                }
                else
                {
                    MemberShipRoleViewModel model = new MemberShipRoleViewModel();
                    MembershipUser user = WorkContext.CurrentMembershipUser;
                    PrepareRoleViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("角色编辑页面加载失败" + ex.Message));
                return RedirectToAction("RoleManage");
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(MemberShipRoleViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name","角色名不能为空");
                }
                if (ModelState.IsValid)
                {
                    if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        Guid company_Id = Guid.Empty;
                        if (model.isAdmin)
                        {
                            company_Id = Guid.Parse(model.Company_Id);
                        }
                        else
                        {
                            company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                        }

                        var company = _companyService.GetById(company_Id);

                        #region 新增
                        using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            var entity = new MembershipRole {
                                Id = Guid.NewGuid(),
                                Name = model.Name,
                                Create_Date = DateTime.Now,
                                Create_By = WorkContext.CurrentMembershipUser.Username,
                                Company = company
                            };

                            _membershipService.AddRole(entity);

                            unitOfwork.Commit();

                            SuccessNotification("添加成功");
                            PrepareRoleViewModel(model, entity);
                            return View(model);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 修改
                        var entity = _membershipService.GetRole(model.Id);
                        Guid company_Id = Guid.Empty;
                        if (model.isAdmin)
                        {
                            company_Id = Guid.Parse(model.Company_Id);
                        }
                        else
                        {
                            company_Id = WorkContext.CurrentMembershipUser.Company.Id;
                        }

                        var company = _companyService.GetById(company_Id);

                        if (entity != null)
                        {
                            using (var unitOfwork = UnitOfWorkManager.NewUnitOfWork())
                            {
                                entity.Name = model.Name;
                                entity.Update_By = WorkContext.CurrentMembershipUser.Username;
                                entity.Update_Date = DateTime.Now;
                                entity.Company = company;
                                unitOfwork.Commit();

                                SuccessNotification("编辑成功");
                                PrepareRoleViewModel(model, entity);
                                return View(model);
                            }
                        }
                        else
                        {
                            ErrorNotification(new Exception("编辑失败，未找到对应的角色"));
                            return RedirectToAction("RoleManage");
                        }
                        #endregion
                    }
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，输入信息有误"));
                    //PrepareCompanyViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("编辑失败"+ex.Message));
                return RedirectToAction("RoleManage");
            }
        }

        public ActionResult Delete(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var entity = _membershipService.GetRole(id);
                if (entity != null)
                {
                    entity.Deleted = true;
                    unitOfWork.Commit();
                    SuccessNotification("删除成功");
                }
                else
                {
                    ErrorNotification(new Exception("删除失败，未找到对应的角色"));
                }
                return RedirectToAction("RoleManage");
            }
        }

        public ActionResult RolePermission(Guid id)
        {
            var model = new RolePermissionViewModel();
            MembershipRole role = _membershipService.GetRole(id);
            model.MembershipRole = role;
            GenerateRolePermissions(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult RolePermission(RolePermissionViewModel model)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    IList<MenuItem> menuItems = new List<MenuItem>();
                    if (WorkContext.CurrentMembershipUser.MembershipRoles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")))
                    {
                        menuItems = _menuItemService.GetAll();
                    }
                    else
                    {

                        foreach (var item in WorkContext.CurrentMembershipUser.MembershipRoles)
                        {
                            var tempRole = _membershipService.GetRole(item.Id);
                            foreach (var menu in tempRole.MenuItems)
                            {
                                menuItems.Add(menu);
                            }
                        }
                    }

                    
                    var role = _membershipService.GetRole(model.Id);
                    foreach (var menu in menuItems)
                    {
                        if (!model.CheckedMenus.Contains(menu.Id) && role.MenuItems.Any(x=>x.Id == menu.Id))
                        {
                            //删除了此菜单
                            role.MenuItems.Remove(menu);
                        }
                        else if (model.CheckedMenus.Contains(menu.Id) && !role.MenuItems.Any(x => x.Id == menu.Id))
                        {
                            // 选中了此菜单
                            role.MenuItems.Add(menu);
                        }
                    }
                    
                    unitOfWork.Commit();
                    SuccessNotification("修改成功");

                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
            }

            
            return RedirectToAction("RolePermission", "MembershipRole", new { id=model.Id });
        }

        #region Manage
        private void SearchOrders(MemberShipRoleSearchViewModel model)
        {
            model.isAdmin = (WorkContext.CurrentMembershipUser.MembershipRoles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")));
            if (model.isAdmin)
            {
                IList<SelectListItem> selList = new List<SelectListItem>();

                var companyList = _companyService.GetAll();

                foreach (var item in companyList)
                {
                    selList.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = (item.Id == WorkContext.CurrentMembershipUser.Company.Id) });
                }
                
                model.Companys = selList;
            }
            Guid company_Id = Guid.Empty;
            if (string.IsNullOrEmpty(model.Company_Id))
            {
                company_Id = WorkContext.CurrentMembershipUser.Company.Id;
            }
            else
            {
                company_Id = Guid.Parse(model.Company_Id);
            }
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var searchModel = new MembershipRoleSearchModel
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    KeyWord = model.KeyWord,
                    Company_Id = company_Id
                };
                model.ViewList = _membershipService.SearchRole(searchModel);
            }
        }

        private void PrepareRoleViewModel(MemberShipRoleViewModel model, MembershipRole entity)
        {
            model.isAdmin = (WorkContext.CurrentMembershipUser.MembershipRoles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")));

            if (model.isAdmin)
            {
                IList<SelectListItem> selList = new List<SelectListItem>();

                var companyList = _companyService.GetAll();
                if (entity == null)
                {
                    foreach (var item in companyList)
                    {
                        selList.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = (item.Id == WorkContext.CurrentMembershipUser.Company.Id) });
                    }
                }
                else
                {
                    foreach (var item in companyList)
                    {
                        selList.Add(new SelectListItem { Text = item.CompanyName, Value = item.Id.ToString(), Selected = (item.Id == entity.Company.Id) });
                    }
                }
                model.Companys = selList;
            }


        }

        private void GenerateRolePermissions(RolePermissionViewModel model)
        {
            MembershipUser user = WorkContext.CurrentMembershipUser;

            if (WorkContext.CurrentMembershipUser.MembershipRoles.Any(x => x.Id == new Guid("7a2f0eca-4daf-4aa5-8c1d-9cffd6aad69f")))
            {
                #region 加载超级管理员菜单
                model.MenuItems = _menuItemService.GetAll();
                #endregion
            }
            else
            {
                #region 加载拥有权限的菜单
                List<MenuItem> list = new List<MenuItem>();
                foreach (var item in user.MembershipRoles)
                {
                    list.AddRange(item.MenuItems);
                }
                model.MenuItems = list;
                #endregion
            }
        }
        #endregion

    }
}