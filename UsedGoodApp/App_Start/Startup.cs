using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using UsedGoodApp.Models;
using Microsoft.Owin.Security.Cookies;
using static UsedGoodApp.Models.IdentityUserModel;

[assembly: OwinStartup(typeof(UsedGoodApp.Startup))]
namespace UsedGoodApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IdentityUserContext>(IdentityUserContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}