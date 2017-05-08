using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;
using Compliance.Dashboard.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StructureMap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Compliance.Dashboard.UI.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private IDashProfileService _profileService;


        public RoleController()
        {
            _profileService = DependencyResolver.Current.GetService<IDashProfileService>();
        }

        public RoleController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            _profileService = DependencyResolver.Current.GetService<IDashProfileService>();
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

        private RolePaginationViewModel GetRoles(int? page, string search, string sortField, bool? isAsc)
        {
            var roles = RoleManager.Roles.Select(role => new RoleViewModel
            {
                Name = role.Name,
                RoleId = role.Id
            }).AsEnumerable();

            #region search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                roles = roles
                        .Where(m => (!string.IsNullOrEmpty(m.Name) && m.Name.ToLower().Contains(search)));
            }
            #endregion

            #region Sorting
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                Func<RoleViewModel, string> orderingFunction = (c => sortField == "Name" ? c.Name :
                                                                    c.Name);

                if (isAsc.HasValue && isAsc.Value)
                    roles = roles.OrderBy(orderingFunction);
                else
                    roles = roles.OrderByDescending(orderingFunction);

            }
            #endregion

            #region pagination
            var pager = new Pager(roles.Count(), page);

            var viewModel = new RolePaginationViewModel
            {
                Items = roles.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).AsEnumerable(),
                Pager = pager
            };
            #endregion
            return viewModel;
        }

        // GET: Role
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page, string search, string sortField, bool? isAsc)
        {
            ViewBag.ActiveMenu = "Roles";
            var viewModel = GetRoles(page, search, sortField, isAsc);

            if (Request.IsAjaxRequest())
            {
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        // POST: Role/Create
        [HttpPost]
        public JsonResult Create(RoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    role.Name = model.Name;
                    var result = RoleManager.Create(role);
                    if (result.Succeeded)
                    {
                        var viewModel = GetRoles(1, null, null, null);
                        return Json(new { IsSuccess = true, data= viewModel });
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, msg = result.Errors.FirstOrDefault() });
                    }
                }
                return Json(new { IsSuccess = false, msg = "Role name can not be empty!" });
            }
            catch
            {
                return Json(new { IsSuccess = false });
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: Role/Edit/5
        [HttpPost]
        public JsonResult Edit(RoleViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = RoleManager.FindById(model.RoleId);
                    role.Name = model.Name;
                    var result = RoleManager.Update(role);
                    if (result.Succeeded)
                    {
                        var viewModel = GetRoles(1, null, null, null);
                        return Json(new { IsSuccess = true, data = viewModel });
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, msg = result.Errors.FirstOrDefault() });
                    }
                }
                return Json(new { IsSuccess = false, msg = "Role name can not be empty!" });
            }
            catch
            {
                return Json(new { IsSuccess = false });
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: Role/Delete/5
        [HttpPost]
        public JsonResult Delete(RoleViewModel model)
        {
            try
            {
                var role = RoleManager.FindById(model.RoleId);
                role.Name = model.Name;
                var result = RoleManager.Delete(role);
                if (result.Succeeded)
                {
                    var viewModel = GetRoles(1, null, null, null);
                    return Json(new { IsSuccess = true, data = viewModel });
                }
                else
                {
                    return Json(new { IsSuccess = false, msg = result.Errors.FirstOrDefault() });
                }
            }
            catch
            {
                return Json(new { IsSuccess = false });
            }
        }
    }
}
