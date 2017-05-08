using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using Compliance.Dashboard.UI.Models;
using Compliance.Dashboard.UI.Providers;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;

namespace Compliance.Dashboard.UI
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //if (!string.IsNullOrEmpty(Code.Constants.FbAppId))
            //{
            //    //app.UseFacebookAuthentication(
            //    //   appId: Code.Constants.FbAppId,
            //    //   appSecret: Code.Constants.FbAppSecret);
            //    var facebookAuthenticationOptions = new FacebookAuthenticationOptions()
            //    {

            //        AppId = Code.Constants.FbAppId,
            //        AppSecret = Code.Constants.FbAppSecret,


            //        Provider = new FacebookAuthenticationProvider
            //        {
            //            OnAuthenticated = context =>
            //            {

            //                // All data from facebook in this object. 
            //                var rawUserObjectFromFacebookAsJson = context.User;

            //                context.Identity.AddClaim(
            //                    new System.Security.Claims.Claim("FacebookAccessToken",
            //                                         context.AccessToken));
            //                context.Identity.AddClaim(new System.Security.Claims.Claim("IsExternal", "true"));
            //                //context.Identity.AddClaim(new System.Security.Claims.Claim("access_token", context.AccessToken));
            //                //context.Identity.AddClaim(new System.Security.Claims.Claim("Email", context.Email));
            //                return Task.FromResult(true);
            //            }
            //        }
            //    };

            //    facebookAuthenticationOptions.Scope.Add("email");
            //    app.UseFacebookAuthentication(facebookAuthenticationOptions);
            //}

            //if (!string.IsNullOrEmpty(Code.Constants.GoogleeClientId))
            //{
            //    var googleProvider = new GoogleOAuth2AuthenticationOptions()
            //    {
            //        ClientId = Code.Constants.GoogleeClientId,
            //        ClientSecret = Code.Constants.GoogleeClientSecret,
            //        //AccessType = "offline",
            //        CallbackPath = new PathString("/social-return"),
            //    };

            //    app.UseGoogleAuthentication(googleProvider);
            //}

        }
    }
}