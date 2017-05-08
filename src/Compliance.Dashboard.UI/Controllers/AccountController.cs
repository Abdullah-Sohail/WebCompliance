using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using StructureMap.Attributes;
using System.Web.Security;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Security.OAuth;
using NLog;
using Compliance.Dashboard.UI.Filters;
using Compliance.Dashboard.UI.Models;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.UI.Models.Shared;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Implementation.Ef;
using Compliance.Dashboard.Domain.ValueTypes;
using Compliance.Dashboard.Implementation.Ef.Services;
using Compliance.Dashboard.Domain.Service;
using Newtonsoft.Json.Linq;

namespace Compliance.Dashboard.UI.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
		private Logger _logger;
        private IDashProfileService _profileService;


        public AccountController()
        {
            _profileService = DependencyResolver.Current.GetService<IDashProfileService>();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            _profileService = DependencyResolver.Current.GetService<IDashProfileService>();
            _logger = LogManager.GetCurrentClassLogger();
            
        }

        private DashProfile _userProfileService = null;
        
        [SetterProperty]
        public DashProfile UserProfileService
        {
            get
            {
                _userProfileService = CacheHelper.GetProfileByUserId(User.Identity.GetUserId(), DependencyResolver.Current);//IoC.Instance().GetAllInstances<IUserProfileService>().FirstOrDefault();
                return _userProfileService;
            }
            set
            {
                _userProfileService = value;
            }
        }
        
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return this._roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set { this._roleManager = value; }
        }

        //IEnumerable<ProfileViewModel> LastSearch = null;
        
        [Authorize(Roles = "Admin")]
        public ActionResult Users(int? page, string search, string sortField, bool? isAsc)
        {
            ViewBag.ActiveMenu = "Users";
            var profiles = _profileService.GetAll().Where(u => u.IsActive == true).Select(p => new ProfileViewModel
            {
                CellNumber = p.CellNumber.ToString()=="0"?"-": p.CellNumber.ToString(),
                Email = p.Email.ToString(),
                FirstName = p.FirstName,
                LastName = p.LastName??"-",
                UserId = p.UserId,
                UtcCreated = p.UtcCreated,
                SelectedRole = UserManager.GetRoles(p.UserId.ToString()).FirstOrDefault()??"-"
            });

            #region search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                profiles = profiles
                        .Where(m => (!string.IsNullOrEmpty(m.FirstName) && m.FirstName.ToLower().Contains(search)) ||
                           (!string.IsNullOrEmpty(m.LastName) && m.LastName.ToLower().Contains(search)) ||
                           (!string.IsNullOrEmpty(m.SelectedRole) && m.SelectedRole.ToLower().Contains(search)) ||
                           (!string.IsNullOrEmpty(m.Email) && m.Email.ToLower().Contains(search)));
            }
            #endregion

            #region Sorting
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                Func<ProfileViewModel, string> orderingFunction = (c => sortField == "FirstName" ? c.FirstName :
                                                                    sortField == "LastName" ? c.LastName :
                                                                    sortField == "SelectedRole" ? c.SelectedRole :
                                                                    sortField == "Email" ? c.Email :
                                                                    c.FirstName);
                 
                if (isAsc.HasValue && isAsc.Value)
                    profiles = profiles.OrderBy(orderingFunction);
                else
                    profiles = profiles.OrderByDescending(orderingFunction);
                
            }
            #endregion

            #region pagination
            var pager = new Pager(profiles.Count(), page);

            var viewModel = new UserPaginationViewModel
            {
                Items = profiles.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).AsEnumerable(),
                Pager = pager
            };
            #endregion
            
            if (Request.IsAjaxRequest())
            {
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            return View(viewModel);
        }

        //
        // GET: /Manage/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                DashProfile userProfile = _profileService.GetByUserId(new Guid(userId));
                if (userProfile == null)
                {
                    ViewBag.Message = "Your profile is not updated.";
                    return RedirectToAction("UpdateProfile");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View();
            }    
        }

        [Authorize(Roles = "Admin")]
        public JsonResult DeleteProfile(ProfileViewModel profile)
        {
            var userId = User.Identity.GetUserId();
            var userToDelete = UserManager.FindById(profile.UserId.ToString());
            if (userToDelete.Id == userId)
            {
                return Json(new { IsSuccess = false, Message="You can not delete your account." });
            }
            if (userToDelete != null)
            {
                DashProfile userProfile = _profileService.GetByUserId(new Guid(userToDelete.Id));
                _profileService.DeleteProfile(userProfile);

                UserManager.RemoveFromRoles(userToDelete.Id, UserManager.GetRoles(userToDelete.Id).ToArray());
                UserManager.Delete(userToDelete);

                return Json(new { IsSuccess=true});
            }
            else
            {
                return Json(new { IsSuccess = false, Message = "Failed to delete user." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginPageCommonModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var manager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //Checking for Email
            var user = manager.FindByEmail(model.LoginViewModel.Email);
            //JObject objAccessToken = GenerateLocalAccessTokenResponse(user.UserName);

            var result = SignInManager.PasswordSignIn(user != null ? user.UserName : model.LoginViewModel.Email, model.LoginViewModel.Password, false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    DashProfile userProfile = _profileService.GetByUserId(new Guid(user.Id));// CacheHelper.GetProfileByUserId(user.Id, DependencyResolver.Current);
                    if (userProfile == null)
                    {
                        userProfile = DashProfile.Create(user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber);
                        _profileService.CreateProfile(userProfile);
                    }
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    ViewBag.Error = "Your account has been blocked by admin.";
                    return View(model);
                case SignInStatus.RequiresVerification:
                    ViewBag.Error = "Please verify your account.";
                    return View(model);
                case SignInStatus.Failure:
                    ViewBag.Error = "Invalid credentials.";
                    return View(model);
                default:
                    ViewBag.Error = "Invalid credentials.";
                    return View(model);
            }
        }

        [Authorize]
        public ActionResult UpdateProfile(string id)
        {
            var userId = User.Identity.GetUserId();
            if (!string.IsNullOrWhiteSpace(id) && User.IsInRole("Admin"))
            {
                userId = id;
            }
            
            DashProfile userProfile = _profileService.GetByUserId(new Guid(userId));
            //ViewBag.ReturnUrl = returnUrl;
            ViewBag.Roles = new SelectList(RoleManager.Roles, "Name", "Name",UserManager.GetRoles(userId).FirstOrDefault());

            if (userProfile != null)
            {
                ProfileViewModel model = new ProfileViewModel
                {
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    CellNumber = userProfile.CellNumber.Value.ToString(),
                    UserId=userProfile.UserId,
                    Email=userProfile.Email.ToString(),
                    SelectedRole = UserManager.GetRoles(userId).FirstOrDefault()
                };
                return View(model);
            }
            else
            {
                ProfileViewModel model = new ProfileViewModel
                {
                    UserId = new Guid(userId),
                    SelectedRole = UserManager.GetRoles(userId).FirstOrDefault()
                };
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(ProfileViewModel model)
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin") && model.UserId!=null && model.UserId!=new Guid())
            {
                userId = model.UserId.ToString();
            }
            if (ModelState.IsValid)
            {
                DashProfile userProfile = _profileService.GetByUserId(new Guid(userId));

                if (userProfile == null)
                {
                    var user= UserManager.FindById(userId);
                    userProfile = DashProfile.Create(user.Id, model.FirstName, model.LastName, user.Email, model.CellNumber);
                    _profileService.CreateProfile(userProfile);

                }
                else
                {
                    userProfile.CellNumber = new Phone(model.CellNumber);
                    userProfile.FirstName = model.FirstName;
                    userProfile.LastName = model.LastName;
                    _profileService.UpdateProfile(userProfile);
                }
                if (!string.IsNullOrWhiteSpace(model.SelectedRole))
                {
                    UserManager.RemoveFromRoles(userId, UserManager.GetRoles(userId).ToArray());
                    var roleresult = UserManager.AddToRole(userId, model.SelectedRole);
                }
                

            }
            else
            {
                return View(model);
            }
            
            
            return RedirectToAction("Users");
        }
        

        private Dictionary<string, string> GenerateLocalAccessTokenResponse(string Id, string userName, bool IsExternal)
        {

            var tokenExpiration = TimeSpan.FromDays(365);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            if (IsExternal)
                identity.AddClaim(new Claim("IsExternal", "true"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);
            
            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

            Dictionary<string, string> res = new Dictionary<string, string>();

            res.Add("userName", userName);
            res.Add("access_token", accessToken);
            res.Add("token_type", "bearer");
            res.Add("expires_in", tokenExpiration.TotalSeconds.ToString());
            res.Add(".issued", ticket.Properties.IssuedUtc.ToString());
            res.Add(".expires", ticket.Properties.ExpiresUtc.ToString());
            //JObject tokenResponse = new JObject(
            //                            new JProperty("userName", userName),
            //                            new JProperty("access_token", accessToken),
            //                            new JProperty("token_type", "bearer"),
            //                            new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
            //                            new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
            //                            new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            //);

            return res;
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //[HttpPost]
        //[Authorize]
        //[ValidateJsonAntiForgeryToken]
        //public async Task<JsonResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    try
        //    {
        //        var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
        //        if (result.Succeeded)
        //        {
        //            model.NewPassword = string.Empty;

        //            return Json(new { success = true, model = string.Join(",", result.Errors) });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, model = string.Join(",", result.Errors) });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, model = ex.Message });
        //    }

        //    return Json(new { success = false, model = model });
        //}

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model.Id == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = UserManager.ResetPassword(model.Id, model.Code, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            else
            {
                ViewBag.Error = "Token has been expired.";
                return View();
            }
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(LoginPageCommonModel model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.ForgotPasswordViewModel.Email))
                {
                    var user = UserManager.FindByEmail(model.ForgotPasswordViewModel.Email);
                    if (user == null)
                    {
                        ViewBag.Error = "Email not found.";
                        return View("Login", model);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(user.PasswordHash))
                        {
                            ViewBag.Error = "You account is associated with social login.";
                            return View("Login", model);
                        }
                    }


                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { Id = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    
                    var htmlEmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Templates/password-reset.html"));
                    if (!string.IsNullOrEmpty(htmlEmail))
                        Code.EmailService.SendEmail(Code.Constants.EmailFrom,
                            model.ForgotPasswordViewModel.Email,
                            "Password Change Request",
                            htmlEmail.Replace("{{ResetLink}}", callbackUrl.Replace("/templates/", "/")));

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Login", model);
            }

            ViewBag.Error = "Error locating account. Contact support for further assistance.";
            return View("Login", model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        public JsonResult ForgotPasswordWithEmail(ForgotPasswordViewModel model)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                var user = UserManager.FindByEmail(model.Email);
                if (user == null)
                {
                    result.Add("success", "false");
                    result.Add("message", "Email not found.");
                    //return Json(new
                    //{
                    //    success = false,
                    //    message = "Email not found.",
                    //});

                    return Json(result);
                }
                else
                {
                    if (string.IsNullOrEmpty(user.PasswordHash))
                    {
                        result.Add("success", "false");
                        result.Add("message", "This account is associated with social login.");

                        return Json(result);
                        //return Json(new
                        //{
                        //    success = false,
                        //    message = "You account is associated with social login.",
                        //});
                    }
                }

                string code = UserManager.GeneratePasswordResetToken(user.Id);
                var callbackUrl = Url.Action("recoveryreturn", "Templates", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                var htmlEmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Templates/password-reset.html"));
                if (!string.IsNullOrEmpty(htmlEmail))
                    Code.EmailService.SendEmail(Code.Constants.EmailFrom,
                        model.Email,
                        "Password Change Request",
                        htmlEmail.Replace("{{ResetLink}}", callbackUrl.Replace("/templates/", "/")));

                result.Add("success", "true");
                result.Add("message", "Password reset email has been send.");

                return Json(result);

                //return Json(new
                //{
                //    success = true,
                //    message = "Check your email for a recovery link.",
                //});

            }
            catch (Exception ex)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                result.Add("success", "false");
                result.Add("message", ex.Message);

                return Json(result);
                // Log ex
                //return Json(new
                //{
                //    success = false,
                //    message = ex.Message,
                //});
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        public JsonResult AjaxExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                Dictionary<string, string> accessToken = GenerateLocalAccessTokenResponse(User.Identity.GetUserId(), User.Identity.GetUserName(), true);
                return Json(new { success = true, message = "Account Validated.", access_token = accessToken });
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = AuthenticationManager.GetExternalLoginInfo();
                if (info == null)
                    return Json(new { success = false, message = "Fail to load external account." });

                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = UserManager.Create(user);
                if (result.Succeeded)
                {
                    var roleresult = UserManager.AddToRole(user.Id, "User");

                    Dictionary<string, string> accessToken = GenerateLocalAccessTokenResponse(user.Id, user.UserName, true);

                    result = UserManager.AddLogin(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

                        if (UserProfileService != null)
                        {
                            var profile = DashProfile.Create(User.Identity.GetUserId(), model.FirstName, model.LastName, user.Email, "");

                            //var htmlEmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Templates/registration-receipt.html"));
                            //if (!string.IsNullOrEmpty(htmlEmail))
                            //    Code.EmailService.SendEmail(Code.Constants.EmailFrom,
                            //        model.Email,
                            //        "Registration Confirmation",
                            //        htmlEmail);
                        }

                        return Json(new { success = true, message = "Logging successful.", access_token = accessToken });
                    }
                    else
                        return Json(new { success = false, message = "Login failed!" });
                }
                else
                {
                    var qr = result.Errors.Where(w => w == string.Format("Email '{0}' is already taken.", model.Email)).ToList();
                    if (qr.Count > 0)
                    {
                        if (model.Email == info.Email)
                        {
                            // Need to fill in if we can't reuse login
                        }
                    }

                    return Json(new { success = false, message = "Email or Username already taken!" });
                }
                AddErrors(result);
            }

            return Json(new { success = false, message = "Invalid model data." });
        }
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        //public ActionResult Register(LoginPageCommonModel model)
        //{
        //    model.RegisterViewModel.Username = model.RegisterViewModel.Email;
        //    if (!string.IsNullOrWhiteSpace(model.RegisterViewModel.FirstName) || !string.IsNullOrWhiteSpace(model.RegisterViewModel.Email))
        //    {
        //        var user = new ApplicationUser
        //        {
        //            UserName = model.RegisterViewModel.Email,
        //            Email = model.RegisterViewModel.Email,
        //            FirstName = model.RegisterViewModel.FirstName,
        //            LastName = model.RegisterViewModel.LastName,
        //        };

        //        try
        //        {
        //            var result = UserManager.Create(user, model.RegisterViewModel.Password);
        //            if (result.Succeeded)
        //            {
        //                //Dictionary<string, string> accessToken = GenerateLocalAccessTokenResponse(user.Id, user.UserName, false);
        //                //SignInManager.SignIn(user, isPersistent: true, rememberBrowser: true);
        //                if (!string.IsNullOrWhiteSpace(model.RegisterViewModel.SelectedRole))
        //                {
        //                    var roleresult = UserManager.AddToRole(user.Id, "Agent");
        //                }

        //                //var user = UserManager.FindById(User.Identity.GetUserId());
        //                if (model.RegisterViewModel.PhoneNo == null)
        //                {
        //                    model.RegisterViewModel.PhoneNo = "";
        //                }
        //                var profile = DashProfile.Create(user.Id, model.RegisterViewModel.FirstName, model.RegisterViewModel.LastName, model.RegisterViewModel.Email, model.RegisterViewModel.PhoneNo);
        //                _profileService.CreateProfile(profile);

        //                var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme);

        //                var htmlEmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Templates/registration-receipt.html"));
        //                htmlEmail = htmlEmail.Replace("{Password}", "");
        //                htmlEmail = htmlEmail.Replace("{ResetLink}", callbackUrl);

        //                if (!string.IsNullOrEmpty(htmlEmail))
        //                    Code.EmailService.SendEmail(Code.Constants.EmailFrom,
        //                        model.RegisterViewModel.Email,
        //                        "Registration Confirmation",
        //                        htmlEmail);


        //                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //                ViewBag.Message = "Your account has been registered.";
        //                return View("Login");
        //            }
        //            else
        //            {
        //                ViewBag.Error = result.Errors.FirstOrDefault();
        //                return View("Login", model);
        //            }

        //            //AddErrors(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            string message = ex.Message;
        //            if (ex.InnerException != null)
        //                message += ex.InnerException.Message;
        //            return View("Login", model);
        //            //return Json(new { success = false, message = message });
        //        }
        //    }

        //    return View("Login",model);
        //}
        
        public ActionResult AddUser(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Roles = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(AddUserViewModel model)
        {
            model.Username = model.Email;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                try
                {
                    var result = UserManager.Create(user, model.Password);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrWhiteSpace(model.SelectedRole))
                        {
                            var roleresult = UserManager.AddToRole(user.Id, model.SelectedRole);
                        }
                        
                        var profile = DashProfile.Create(user.Id, model.FirstName, model.LastName, model.Email, model.PhoneNo);
                        _profileService.CreateProfile(profile);

                        var callbackUrl = Url.Action("Index", "Home", null, protocol: Request.Url.Scheme);
                        
                        var htmlEmail = System.IO.File.ReadAllText(Server.MapPath(@"~/Templates/registration-receipt.html"));
                        htmlEmail = htmlEmail.Replace("{Email}", model.Email);
                        htmlEmail = htmlEmail.Replace("{Password}", model.Password);
                        htmlEmail = htmlEmail.Replace("{HomeLink}", callbackUrl);

                        if (!string.IsNullOrEmpty(htmlEmail))
                            Code.EmailService.SendEmail(Code.Constants.EmailFrom,
                                model.Email,
                                "Registration Complete",
                                htmlEmail);


                        return RedirectToAction("Users");
                    }
                    else
                    {
                        ViewBag.Error = result.Errors.FirstOrDefault();
                        return View(model);
                    }

                    //AddErrors(result);
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    if (ex.InnerException != null)
                        message += ex.InnerException.Message;
                    return View(model);
                    //return Json(new { success = false, message = message });
                }
            }

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult ConfirmEmail(string userId, string code)
        {
            try
            {
                var result = UserManager.ConfirmEmail(userId, code);

                return result.Succeeded ? Json(new { success = true }) : Json(new { success = false });
            }
            catch (Exception ex)
            {
                // Log
            }
            return Json(new { success = false });
        }
        
        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
            //return new ChallengeResult(provider, returnUrl);
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
	        try
	        {
		        ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

		        //var ctx = Request.GetOwinContext();
		        //var resultAuthentication = ctx.Authentication.AuthenticateAsync("ExternalCookie").Result;
		        //this.AuthenticationManager.SignOut("ExternalCookie");

		        //var claims = resultAuthentication.Identity.Claims.ToList();
		        //claims.Add(new Claim(ClaimTypes.AuthenticationMethod, "External"));

		        //resultAuthentication.Identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, "External"));

		        // These next three lines is how I get the email from the stuff that gets returned from the Facebook external provider
		        //var externalIdentity = HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
		        //var emailClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
		        //var email = emailClaim.Value;

		        if (loginInfo == null)
		        {
			        return RedirectToAction("Login");
		        }
		        else
			        return RedirectToLocal("/socialreturn");

		        // Sign in the user with this external login provider if the user already has a login
		        var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
		        switch (result)
		        {
			        case SignInStatus.Success:
				        return RedirectToLocal(returnUrl);
			        case SignInStatus.LockedOut:
				        return View("Lockout");
			        case SignInStatus.RequiresVerification:
				        return RedirectToAction("SendCode", new {ReturnUrl = returnUrl, RememberMe = false});
			        case SignInStatus.Failure:
			        default:
				        // If the user does not have an account, then prompt the user to create an account
				        ViewBag.ReturnUrl = returnUrl;
				        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

				        return View("ExternalLoginConfirmation",
					        new ExternalLoginConfirmationViewModel
					        {
						        Email = loginInfo.Email,
						        FirstName = loginInfo.ExternalIdentity.Name,
						        Username = loginInfo.ExternalIdentity.GetUserName()
					        });
		        }
	        }
	        catch (Exception ex)
	        {
		        _logger.Error(ex);
		        throw;
	        }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            Request.GetOwinContext().Authentication.SignOut();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}