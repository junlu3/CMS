using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class LawsManageController : BaseController
    {
        private readonly ILawService _lawsService;
        private readonly IWorkContext _workContext;

        public LawsManageController(ILawService lawsService, IWorkContext workContext)
        {
            this._lawsService = lawsService;
            this._workContext = workContext;
        }

        public ActionResult Index()
        {
            var model = new LawListViewModel()
            {
                LawList = _lawsService.Search(new LawSearchModel())
            };
            return View(model);
        }

        public ActionResult CreateOrUpdate(Guid? id = null)
        {
            if (id != null)
            {
                var entity = _lawsService.GetById(id.Value);
                if (entity != null)
                {
                    var model = new LawViewModel()
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        DocumentNumber = entity.DocumentNumber,
                        ImplementationDate = entity.ImplementationDate,
                        FilePath = entity.FilePath,
                    };
                    return View(model);
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，未找到Id为" + id.ToString() + "的法律法规"));
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(new LawViewModel());
            }
        }
        //[AllowAnonymous]
        //public ActionResult View(string filePath)
        //{
        //    var model =new  LawViewModel();
        //    model.FilePath = filePath;
        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(LawViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                    {
                        var file = Request.Files["importfile"];
                        var fileName = "";
                        if (!string.IsNullOrEmpty(model.FilePath) && file != null && file.ContentLength > 0)
                        {
                            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                            {
                                ErrorNotification(new Exception("只能上传pdf类型的文档"));
                                return View(model);
                            }
                            else
                            {
                                fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".pdf";
                                file.SaveAs(Server.MapPath("~/Content/Laws/" + fileName));
                            }
                        }

                        var entity = new Law()
                        {
                            Id = Guid.NewGuid(),
                            Name = model.Name,
                            DocumentNumber = model.DocumentNumber,
                            ImplementationDate = model.ImplementationDate,
                            FilePath = string.IsNullOrEmpty(fileName) ? "" : "/Content/Laws/" + fileName,
                            CreatedBy = _workContext.CurrentMembershipUser.Username,
                            CreatedDate = DateTime.Now
                        };
                        _lawsService.Add(entity);
                        unitOfWork.Commit();

                        SuccessNotification("添加成功");
                        return View(model);
                    }
                }
                else
                {
                    var entity = _lawsService.GetById(model.Id);
                    if (entity != null)
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {
                            entity.Name = model.Name;
                            entity.DocumentNumber = model.DocumentNumber;
                            entity.ImplementationDate = model.ImplementationDate;
                            var file = Request.Files["importfile"];
                            if (!string.IsNullOrEmpty(model.FilePath) && file != null && file.ContentLength > 0)
                            {
                                if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                                {
                                    ErrorNotification(new Exception("只能上传pdf类型的文档"));
                                    return View(model);
                                }
                                else
                                {
                                    var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".pdf";
                                    file.SaveAs(Server.MapPath("~/Content/Laws/" + fileName));
                                    entity.FilePath = "/Content/Laws/" + fileName;
                                }
                            }
                            else
                            {
                                model.FilePath = "";
                                entity.FilePath = "";
                            }
                            unitOfWork.Commit();

                            SuccessNotification("编辑成功");
                            return View(model);
                        }
                    }
                    else
                    {
                        ErrorNotification(new Exception("编辑失败，未找到Id为" + model.Id.ToString() + "的法律法规"));
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                ErrorNotification(new Exception("编辑失败，无效的模型状态"));
                return View(model);
            }
        }

        public ActionResult Delete(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var entity = _lawsService.GetById(id);
                if (entity != null)
                {
                    entity.Deleted = true;
                    unitOfWork.Commit();
                    SuccessNotification("删除成功");
                }
                else
                {
                    ErrorNotification(new Exception("删除失败，未找到Id为" + id.ToString() + "的法律法规"));
                }
                return RedirectToAction("Index");
            }
        }
    }
}