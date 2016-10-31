using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XL.CHC.Domain.Constants;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Domain.Interfaces.UnitOfWork;

namespace XL.CHC.Services
{
    public class WorkContext : IWorkContext
    {
        private const string CookieName = "XL.CHC.MembershipUser";
        private readonly HttpContextBase _httpContext;
        private readonly IMembershipService _membershipService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAuthenticationService _authenticationService;

        private MembershipUser _cachedMembershipUser;
        private List<MenuItemWithChildren> _cachedPageMenuItems;

        public WorkContext(IMembershipService membershipService, IUnitOfWorkManager unitOfWorkManager
            , FormsAuthenticationService authenticationService)
        {
            this._httpContext = new HttpContextWrapper(HttpContext.Current) as HttpContextBase;
            this._membershipService = membershipService;
            this._unitOfWorkManager = unitOfWorkManager;
            this._authenticationService = authenticationService;
        }

        public MembershipUser CurrentMembershipUser
        {
            get
            {
                if (_cachedMembershipUser != null)
                {
                    return _cachedMembershipUser;
                }
                else
                {
                    using (_unitOfWorkManager.NewUnitOfWork())
                    {
                        MembershipUser user = null;
                        user = _authenticationService.GetAuthenticatedMembershipUser();
                        if (user != null && !user.Deleted)
                        {
                            SetMembershipUserCookie(user.Username);
                            _cachedMembershipUser = user;
                        }
                        return _cachedMembershipUser;
                    }

                }
            }

            set
            {
                SetMembershipUserCookie(value.Username);
                this._cachedMembershipUser = value;

            }
        }

        public List<MenuItemWithChildren> PageMenuItems
        {
            get
            {
                if (_cachedPageMenuItems != null)
                {
                    return _cachedPageMenuItems;
                }
                else
                {
                    SetPageMenuItems();
                    return _cachedPageMenuItems;
                }
            }

            set
            {
                this._cachedPageMenuItems = value;
            }
        }

        private void SetPageMenuItems()
        {
            _cachedPageMenuItems = new List<MenuItemWithChildren>();
            using (_unitOfWorkManager.NewUnitOfWork())
            {
                var roles = CurrentMembershipUser.MembershipRoles;
                foreach (var role in roles)
                {
                    var mis = role.MenuItems.Where(x => x.ParentId == 0).ToList();
                    foreach (var menu in mis)
                    {
                        if (!_cachedPageMenuItems.Any(x => x.MenuItem.Id == menu.Id))
                        {
                            var mwc = new MenuItemWithChildren();
                            mwc.MenuItem = menu;
                            mwc.SubMenuItems = role.MenuItems.Where(x => x.ParentId == menu.Id)
                                .OrderBy(x => x.MenuOrder)
                                .ToList();
                            _cachedPageMenuItems.Add(mwc);
                        }
                        else
                        {
                           var rootNode = _cachedPageMenuItems.SingleOrDefault(x=>x.MenuItem.Id == menu.Id);
                           var nodeOfRoles = role.MenuItems.Where(x => x.ParentId == menu.Id).ToList();
                            foreach (var item in nodeOfRoles)
                            {
                                if (!rootNode.SubMenuItems.Any(x=>x.Id == item.Id))
                                {
                                    rootNode.SubMenuItems.Add(item);
                                }
                            }
                        }
                    }
                }
                _cachedPageMenuItems = _cachedPageMenuItems.OrderBy(x => x.MenuItem.MenuOrder).ToList();
            }
        }

        private void SetMembershipUserCookie(string username)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(CookieName);
                cookie.HttpOnly = true;
                cookie.Value = username;
                if (string.IsNullOrEmpty(username))
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365;
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }
                _httpContext.Response.Cookies.Remove(CookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        public void RemoveMembershipUser()
        {
            RemoveMembershipUserCookie();
            this._cachedMembershipUser = null;

        }

        protected virtual void RemoveMembershipUserCookie()
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                _httpContext.Response.Cookies.Remove(CookieName);
            }
        }

        public void ClearMenuItems()
        {
            this._cachedPageMenuItems = null;
        }
    }
}
