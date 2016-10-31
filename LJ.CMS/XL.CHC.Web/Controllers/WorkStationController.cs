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
    public class WorkStationController : BaseController
    {
        #region Fields
        private readonly IMSDS_WorkStationService _workstationService;
        private readonly IMSDS_WorkShopService _workshopService;
        #endregion

        public WorkStationController(IMSDS_WorkStationService workstationService,
            IMSDS_WorkShopService workshopService)
        {
            _workshopService = workshopService;
            _workstationService = workstationService;
        }

        #region Action
        public ActionResult Index()
        {
            try
            {
                var model = new WorkStationSearchViewModel();

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
        public ActionResult Index(WorkStationSearchViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new WorkStationSearchViewModel();
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
                    var entity = _workstationService.Single(id.Value);
                    if (entity != null)
                    {
                        var model = new WorkStationViewModel
                        {
                            Id = entity.Id,
                            WorkStation_Name = entity.WorkStation_Name.Trim()
                        };
                        PrepareWorkStationViewModel(model, entity);
                        return View(model);
                    }
                    else
                    {
                        ErrorNotification(new Exception("加载失败，未找到该工位"));
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    WorkStationViewModel model = new WorkStationViewModel();
                    PrepareWorkStationViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("工位编辑页面加载失败" + ex.Message));
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(WorkStationViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.WorkStation_Name))
                {
                    ModelState.AddModelError("WorkStation_Name", "名称不能为空");
                }
                if (string.IsNullOrEmpty(model.WorkShop_Id))
                {
                    ModelState.AddModelError("WorkShop_Id", "车间不能为空");
                }
                if (ModelState.IsValid)
                {
                    if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            Guid workshop_Id = Guid.Parse(model.WorkShop_Id);
                            MSDS_WorkShop workShop = _workshopService.Single(workshop_Id);
                            var entity = new MSDS_WorkStation()
                            {
                                Id = Guid.NewGuid(),
                                WorkStation_Name = model.WorkStation_Name.Trim(),
                                WorkShop = workShop
                            };
                            _workstationService.Add(entity);

                            unitOfWork.Commit();

                            SuccessNotification("添加成功");
                            PrepareWorkStationViewModel(model, entity);
                            return View(model);
                        }
                    }
                    else
                    {
                        var entity = _workstationService.Single(model.Id);
                        if (entity != null)
                        {
                            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                            {
                                if (entity.WorkStation_Name != model.WorkStation_Name)
                                {
                                    if (_workstationService.GetAll(WorkContext.CurrentMembershipUser.Company.Id).Any(x => x.WorkStation_Name == model.WorkStation_Name))
                                    {
                                        ErrorNotification(new Exception("工位名已存在"));
                                        PrepareWorkStationViewModel(model, entity);
                                        return View(model);
                                    }
                                    else
                                    {
                                        entity.WorkStation_Name = model.WorkStation_Name.Trim();
                                        unitOfWork.Commit();
                                        SuccessNotification("编辑成功");
                                        PrepareWorkStationViewModel(model, entity);
                                        return View(model);
                                    }

                                }
                                else
                                {
                                    SuccessNotification("编辑成功");
                                    PrepareWorkStationViewModel(model, entity);
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
                    PrepareWorkStationViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                PrepareWorkStationViewModel(model, null);
                return View(model);
            }

        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var entity = _workstationService.Single(id);
                    if (entity != null)
                    {
                        _workstationService.Delete(entity);
                        unitOfWork.Commit();
                        SuccessNotification("删除成功");
                    }
                    else
                    {
                        ErrorNotification(new Exception("删除失败，未找到Id为" + id.ToString() + "的工位"));
                    }
                    return RedirectToAction("Index", "WorkStation");
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Index", "WorkStation");
            }
        }
        #endregion

        #region Manage
        private void SearchOrders(WorkStationSearchViewModel model)
        {

            IList<SelectListItem> selList = new List<SelectListItem>();
            selList.Add(new SelectListItem { Text="全部", Value= "00000000-0000-0000-0000-000000000000", Selected= true });
            IList<MSDS_WorkShop> shops = _workshopService.GetAll(WorkContext.CurrentMembershipUser.Company.Id);
            foreach (var item in shops)
            {
                selList.Add(new SelectListItem { Text = item.WorkShop_Name, Value = item.Id.ToString(), Selected = false });
            }
            model.WorkShops = selList;

            Guid workshop_id = Guid.Parse(model.WorkShop_Id);

            var searchModel = new WorkStationSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                KeyWord = model.KeyWord,
                WorkShop_Id = workshop_id
            };
            model.ViewList = _workstationService.Search(searchModel);
        }

        private void PrepareWorkStationViewModel(WorkStationViewModel model,MSDS_WorkStation entity)
        {
            IList<SelectListItem> selList = new List<SelectListItem>();
            IList<MSDS_WorkShop> shops = _workshopService.GetAll(WorkContext.CurrentMembershipUser.Company.Id);
            
            if (entity == null)
            {
                foreach (var item in shops)
                {
                    selList.Add(new SelectListItem { Text = item.WorkShop_Name, Value = item.Id.ToString(), Selected = false });
                }
            }
            else
            {
                foreach (var item in shops)
                {
                    selList.Add(new SelectListItem { Text = item.WorkShop_Name, Value = item.Id.ToString(), Selected = item.Id == entity.WorkShop.Id });
                }
            }
            model.WorkShops = selList;
        }
        #endregion

    }
}