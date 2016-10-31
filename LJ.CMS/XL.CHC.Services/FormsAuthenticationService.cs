using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces;
using XL.CHC.Domain.Interfaces.Services;

namespace XL.CHC.Services
{
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IMembershipService _membershipService;
        private readonly TimeSpan _expirationTimeSpan;

        private MembershipUser _cachedMembershipUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="customerSettings">Customer settings</param>
        public FormsAuthenticationService(IMembershipService membershipService)
        {
            this._httpContext = new HttpContextWrapper(HttpContext.Current) as HttpContextBase;
            this._membershipService = membershipService;
            this._expirationTimeSpan = System.Web.Security.FormsAuthentication.Timeout;
        }


        public virtual void SignIn(MembershipUser membershipUser, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new System.Web.Security.FormsAuthenticationTicket(
                1 /*version*/,
                membershipUser.Username,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                membershipUser.Username,
                System.Web.Security.FormsAuthentication.FormsCookiePath);

            var encryptedTicket = System.Web.Security.FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = System.Web.Security.FormsAuthentication.RequireSSL;
            cookie.Path = System.Web.Security.FormsAuthentication.FormsCookiePath;
            if (System.Web.Security.FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = System.Web.Security.FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedMembershipUser = membershipUser;
        }

        public virtual void SignOut()
        {
            _cachedMembershipUser = null;
            FormsAuthentication.SignOut();
        }

        public virtual MembershipUser GetAuthenticatedMembershipUser()
        {
            if (_cachedMembershipUser != null)
                return _cachedMembershipUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
            if (customer != null && !customer.Deleted)
                _cachedMembershipUser = customer;
            return _cachedMembershipUser;
        }

        public virtual MembershipUser GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var username = ticket.UserData;

            if (String.IsNullOrWhiteSpace(username))
                return null;
            var customer = _membershipService.GetUser(username);

            return customer;
        }
    }
}
