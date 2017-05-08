using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;
using Compliance.Dashboard.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Linq;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(Compliance.Dashboard.UI.Startup))]
namespace Compliance.Dashboard.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login   
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var _profileService= DependencyResolver.Current.GetService<IDashProfileService>();


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Agent"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Agent";
                roleManager.Create(role);

            }

            if (UserManager.Users.ToList().Count==0)
            {
                //Here we create a Admin super user who will maintain the website                  
                var user = new ApplicationUser();
                user.UserName = "rickstephen@gmail.com";
                user.Email = "rickstephen@gmail.com";
                user.FirstName = "Rick";
                user.PhoneNumber = "123456";
                string userPWD = "123456";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                    var profile = DashProfile.Create(user.Id, user.FirstName, user.LastName, user.Email, user.PhoneNumber);
                    _profileService.CreateProfile(profile);
                }
            }
        }
    }
}
