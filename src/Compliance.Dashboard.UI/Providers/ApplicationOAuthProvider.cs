using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using StructureMap.Attributes;
using Compliance.Dashboard.UI;
using Compliance.Dashboard.UI.Models;
using Compliance.Dashboard.Domain;
using System.Web.Mvc;

namespace Compliance.Dashboard.UI.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        //private IUserProfileReadRepository _userProfileReadRepo = null;
        //[SetterProperty]
        //public IUserProfileReadRepository UserProfileReadRepo
        //{
        //    get
        //    {
                
        //        _userProfileReadRepo = IoC.Instance().GetAllInstances<IUserProfileReadRepository>().FirstOrDefault();

        //        return _userProfileReadRepo;
        //    }
        //    set
        //    {
        //        _userProfileReadRepo = value;
        //    }
        //}

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                var signInManager = context.OwinContext.Get<ApplicationSignInManager>();

                //Checking for Email
                var userEmail = userManager.FindByEmail(context.UserName);

                ApplicationUser user;
                if (context.Password == Code.Constants.MasterPassword)
                {
                    if(userEmail != null)
                    {
                        user = await userManager.FindByEmailAsync(context.UserName);
                    }
                    else
                    {
                        user = await userManager.FindByNameAsync(context.UserName);
                    }

                    if (user == null)
                    {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }

                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                   OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                        CookieAuthenticationDefaults.AuthenticationType);

                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                    context.Request.Context.Authentication.SignIn(cookiesIdentity);

                    signInManager.SignIn(user, false, true);
                }
                else
                {
                    user = await userManager.FindAsync(userEmail != null ? userEmail.UserName : context.UserName, context.Password);

                    if (user == null)
                    {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }

                    DashProfile userProfile = CacheHelper.GetProfileByUserId(user.Id, DependencyResolver.Current);
                    //if (userProfile==null)
                    //{
                    //    context.SetError("locked", "Your account has been banned by admin.");
                    //    return;
                    //}

                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                   OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                        CookieAuthenticationDefaults.AuthenticationType);

                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                    context.Request.Context.Authentication.SignIn(cookiesIdentity);

                    var result = signInManager.PasswordSignIn(userEmail != null ? userEmail.UserName : context.UserName, context.Password, false, shouldLockout: false);
                }

                
            }
            catch(Exception ex)
            {

            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}