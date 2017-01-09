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
    public class CustomerController : BaseController
    {

        #region Fields
        private readonly IMSDS_CustomerService _customerService;
        #endregion

        public CustomerController(IMSDS_CustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customer
        public ActionResult Index()
        {
            try
            {
                var model = new CustomerSearchViewModel();
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
            
        }


        private void SearchOrders(CustomerSearchViewModel model)
        {
            CustomerSearchModel searchModel = new CustomerSearchModel {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                KeyWord = model.Keyword
            };

            model.ViewList = _customerService.Search(searchModel);
           
        }
    }
}