using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class WorkShopController : BaseController
    {
        #region Fields
        private readonly IMSDS_WorkShopService _workShopService;
        private readonly ICompanyService _companyService;
        #endregion
        public WorkShopController(IMSDS_WorkShopService workshopService,ICompanyService companyService)
        {
            _workShopService = workshopService;
            _companyService = companyService;
        }

        #region Action
        public ActionResult Index()
        {
            try
            {
                var model = new WorkShopSearchViewModel();
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
        public ActionResult Index(WorkShopSearchViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new WorkShopSearchViewModel();
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
                    var entity = _workShopService.Single(id.Value);
                    if (entity != null)
                    {
                        var model = new WorkShopViewModel
                        {
                            Id = entity.Id,
                            WorkShop_Name = entity.WorkShop_Name.Trim(),
                            Company_Id = entity.Company.Id                            
                        };
                        PrepareWorkShopViewModel(model, entity);
                        return View(model);
                    }
                    else
                    {
                        ErrorNotification(new Exception("加载失败，未找到该车间"));
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    WorkShopViewModel model = new WorkShopViewModel();
                    PrepareWorkShopViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("车间编辑页面加载失败" + ex.Message));
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(WorkShopViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.WorkShop_Name))
                {
                    ModelState.AddModelError("WorkShop_Name", "名称不能为空");
                }
                if (ModelState.IsValid)
                {
                    if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            Company company = _companyService.GetById(WorkContext.CurrentMembershipUser.Company.Id);
                            var entity = new MSDS_WorkShop()
                            {
                                Id = Guid.NewGuid(),
                                WorkShop_Name = model.WorkShop_Name.Trim(),
                                Company = company
                            };
                            _workShopService.Add(entity);

                            unitOfWork.Commit();

                            SuccessNotification("添加成功");
                            PrepareWorkShopViewModel(model, entity);
                            return View(model);
                        }
                    }
                    else
                    {
                        var entity = _workShopService.Single(model.Id);
                        if (entity != null)
                        {
                            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                            {
                                if (entity.WorkShop_Name != model.WorkShop_Name)
                                {
                                    if (_workShopService.GetAll(WorkContext.CurrentMembershipUser.Company.Id).Any(x => x.WorkShop_Name == model.WorkShop_Name))
                                    {
                                        ErrorNotification(new Exception("车间名已存在"));
                                        PrepareWorkShopViewModel(model, entity);
                                        return View(model);
                                    }
                                    else
                                    {
                                        entity.WorkShop_Name = model.WorkShop_Name.Trim();
                                        unitOfWork.Commit();
                                        SuccessNotification("编辑成功");
                                        PrepareWorkShopViewModel(model, entity);
                                        return View(model);
                                    }

                                }
                                else
                                {
                                    SuccessNotification("编辑成功");
                                    PrepareWorkShopViewModel(model, entity);
                                    return View(model);
                                }
                            }
                        }
                        else
                        {
                            ErrorNotification(new Exception("编辑失败，未找到对应的车间"));
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，输入信息有误"));
                    PrepareWorkShopViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                PrepareWorkShopViewModel(model, null);
                return View(model);
            }

        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var entity = _workShopService.Single(id);
                    if (entity != null)
                    {
                        _workShopService.Delete(entity);
                        unitOfWork.Commit();
                        SuccessNotification("删除成功");
                    }
                    else
                    {
                        ErrorNotification(new Exception("删除失败，未找到Id为" + id.ToString() + "的车间"));
                    }
                    return RedirectToAction("Index", "WorkShop");
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Index", "WorkShop");
            }

        }
        #endregion

        #region Manage
        private void SearchOrders(WorkShopSearchViewModel model)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var searchModel = new WorkShopSearchModel
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    KeyWord = model.KeyWord
                };
                model.ViewList = _workShopService.Search(searchModel);
            }
        }

        private void PrepareWorkShopViewModel(WorkShopViewModel model,MSDS_WorkShop entity)
        {

        }
        #endregion
    }
}