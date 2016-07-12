﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.Owin.Security.OpenIdConnect;

namespace AzureKit.Policies
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PolicyAuthorize : System.Web.Mvc.AuthorizeAttribute
    {
        public string Policy { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {Startup.PolicyKey, Policy}
                        })
                    {
                        RedirectUri = "/",
                    }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
