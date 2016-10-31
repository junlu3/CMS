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
    public class WorkerController : BaseController
    {
        #region Fields
        private readonly IMSDS_WorkStationService _workStationService;
        private readonly IMSDS_WorkerService _workerService;
        private readonly IMSDS_WorkShopService _workShopService;
        private readonly ICompanyService _companyService;
        #endregion

        public WorkerController(IMSDS_WorkStationService workStationService,
            IMSDS_WorkerService workerService,
            ICompanyService companyService,
            IMSDS_WorkShopService workShopService)
        {
            _workStationService = workStationService;
            _workerService = workerService;
            _companyService = companyService;
            _workShopService = workShopService;
        }

        #region Action
        public ActionResult Index()
        {
            try
            {
                var model = new WorkerSearchViewModel();
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
        public ActionResult Index(WorkerSearchViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new WorkerSearchViewModel();
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
                    var entity = _workerService.Single(id.Value);
                    if (entity != null)
                    {
                        var model = new WorkerViewModel
                        {
                            Id = entity.Id,
                            Worker_Name = entity.Worker_Name,
                            Worker_ID = entity.Worker_ID
                        };
                        //PrepareWorkerViewModel(model, entity);
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
                    WorkerViewModel model = new WorkerViewModel();
                    //PrepareWorkerViewModel(model, null);
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
        public ActionResult CreateOrUpdate(WorkerViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Worker_ID))
                {
                    ModelState.AddModelError("Worker_ID", "身份证号码不能为空");
                }
                if (string.IsNullOrEmpty(model.Worker_Name))
                {
                    ModelState.AddModelError("Worker_Name", "工人名字不能为空");
                }
                if (ModelState.IsValid)
                {
                    if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            IList<MSDS_WorkStation> workStations = new List<MSDS_WorkStation>();

                            Company company = _companyService.GetById(WorkContext.CurrentMembershipUser.Company.Id);
                            var entity = new MSDS_Worker()
                            {
                                Id = Guid.NewGuid(),
                                Worker_ID = model.Worker_ID?.Trim(),
                                Worker_Name = model.Worker_Name?.Trim(),
                                Company = company,
                                WorkStations = workStations
                            };

                            _workerService.Add(entity);

                            unitOfWork.Commit();

                            SuccessNotification("添加成功");
                            //PrepareWorkerViewModel(model, entity);
                            return View(model);
                        }
                    }
                    else
                    {
                        var entity = _workerService.Single(model.Id);
                        if (entity != null)
                        {
                            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                            {
                                if (entity.Worker_ID != model.Worker_ID)
                                {
                                    if (_workerService.GetAll(WorkContext.CurrentMembershipUser.Company.Id).Any(x => x.Worker_ID == model.Worker_ID))
                                    {
                                        ErrorNotification(new Exception("身份证已存在"));
                                        //PrepareWorkerViewModel(model, entity);
                                        return View(model);
                                    }
                                    else
                                    {
                                        entity.Worker_ID = model.Worker_ID?.Trim();

                                    }

                                }

                                entity.Worker_Name = model.Worker_Name?.Trim();


                                
                                unitOfWork.Commit();
                                SuccessNotification("编辑成功");
                                //PrepareWorkerViewModel(model, entity);
                                return View(model);
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
                    //PrepareWorkerViewModel(model, null);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                //PrepareWorkerViewModel(model, null);
                return View(model);
            }

        }
        #endregion

        public ActionResult AssignWorkStation(Guid id)
        {
            try
            {
                var entity = _workerService.Single(id);
                if (entity != null)
                {
                    WorkerViewModel model = new WorkerViewModel();
                    foreach (var item in entity.WorkStations)
                    {
                        model.WorkStations_Seleted.Add(new SelectListItem { Text= "["+item.WorkShop.WorkShop_Name + "][" + item.WorkStation_Name+"]",Value=item.Id.ToString() });
                    }
                    Guid defaultShopId = Guid.Empty;
                    var shops = _workShopService.GetAll(WorkContext.CurrentMembershipUser.Company.Id);
                    int i = 1;
                    foreach (var item in shops)
                    {
                        if (i == 1)
                        {
                            defaultShopId = item.Id;
                        }
                        model.WorkShops.Add(new SelectListItem { Text = item.WorkShop_Name, Value = item.Id.ToString(), Selected = i == 1 });
                        i++;
                    }

                    var stations = _workStationService.GetAll(defaultShopId);
                    foreach (var item in stations)
                    {
                         model.WorkStations.Add(new SelectListItem { Text = item.WorkStation_Name, Value = item.Id.ToString() });
                    }

                    return View(model);

                }
                else
                {
                    ErrorNotification(new Exception("未能找到该工人"));
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
        public ActionResult AssignWorkStation(WorkerViewModel model)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var entity = _workerService.Single(model.Id);
                    if (entity != null)
                    {

                        if (model.WorkStations_Seleted_Value != null)
                        {
                            entity.WorkStations.Clear();
                            if (model.WorkStations_Seleted_Value.Length != 0)
                            {
                                foreach (var item in model.WorkStations_Seleted_Value)
                                {
                                    Guid station_id = Guid.Parse(item);
                                    MSDS_WorkStation station = _workStationService.Single(station_id);
                                    if (station != null)
                                    {
                                        entity.WorkStations.Add(station);
                                    }
                                }
                            }
                            unitOfWork.Commit();
                        }
                            SuccessNotification("编辑成功");
                            return RedirectToAction("AssignWorkStation",new { id=model.Id});
                       
                    }
                    else
                    {
                        ErrorNotification(new Exception("未能找到该工人"));
                        return View();
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
        }

        public JsonResult GetWorkStations(Guid id)
        {
            var stations = _workStationService.GetAll(id);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in stations)
            {
                list.Add(new SelectListItem { Text = item.WorkStation_Name, Value = item.Id.ToString(), Selected = false });
            }
            var json = new JsonResult
            {
                Data = list,
            };
            return json;
        }
        #region Manage

        private void SearchOrders(WorkerSearchViewModel model)
        {
            var searchModel = new WorkerSearchModel
            {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                KeyWord = model.KeyWord
            };
            model.ViewList = _workerService.Search(searchModel);
        }

        #endregion
    }
}