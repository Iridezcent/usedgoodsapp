using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UsedGoodApp.Models
{
    public class IdentityUserModel
    {
        public class IdentityUserContext : IdentityDbContext<ApplicationUser>
        {
            public IdentityUserContext() : base("IdentityUserDb") { }

            public static IdentityUserContext Create()
            {
                return new IdentityUserContext();
            }
        }

        public class ApplicationUser : IdentityUser
        {
            public ApplicationUser()
            {

            }

            public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
            {
                // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
                var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
                // Здесь добавьте утверждения пользователя
                return userIdentity;
            }
        }

        public class ApplicationUserManager : UserManager<ApplicationUser>
        {
            public ApplicationUserManager(IUserStore<ApplicationUser> store)
                    : base(store)
            {
            }
            public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
                                                    IOwinContext context)
            {
                IdentityUserContext db = context.Get<IdentityUserContext>();
                ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                var dataProtectionProvider = options.DataProtectionProvider;
                if(dataProtectionProvider != null)
                {
                    manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("UsedGoodApp"));    
                }
                return manager;
            }
        }

        public class ApplicationRole : IdentityRole
        {
            public ApplicationRole() { }
        }

        public class ApplicationRoleManager : RoleManager<ApplicationRole>
        {
            public ApplicationRoleManager(RoleStore<ApplicationRole> store)
                        : base(store)
            { }
            public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
                                                    IOwinContext context)
            {
                return new ApplicationRoleManager(new
                        RoleStore<ApplicationRole>(context.Get<IdentityUserContext>()));
            }
        }

        public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
        {
            public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
                : base(userManager, authenticationManager)
            {
            }

            public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
            {
                return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
            }

            public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
            {
                return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
            }
        }

        public class IdentityUserDbInitializer : CreateDatabaseIfNotExists<IdentityUserContext>
        {
            protected override void Seed(IdentityUserContext context)
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
                var rolesManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                var adminRole = new IdentityRole("Admin");
                var moderatorRole = new IdentityRole("Moderator");
                var raterRole = new IdentityRole("Rater");
                var clientRole = new IdentityRole("Client");
                var partnerRole = new IdentityRole("Partner");

                rolesManager.Create(adminRole);
                rolesManager.Create(moderatorRole);
                rolesManager.Create(raterRole);
                rolesManager.Create(clientRole);
                rolesManager.Create(partnerRole);

                var admin = new ApplicationUser() { Email = "adminmail@gmail.com", UserName = "adminmail@gmail.com" };
                var adminPassword = "Gr34dy_4dmin_pass";
                userManager.Create(admin, adminPassword);

                var moderator = new ApplicationUser() { Email = "modertormail@gmail.com", UserName = "modertormail@gmail.com" };
                var moderatorPassword = "Gr34dy_4dmin_pass";
                userManager.Create(moderator, moderatorPassword);

                var rater = new ApplicationUser() { Email = "ratermail@gmail.com", UserName = "ratermail@gmail.com" };
                var raterPassword = "Gr34dy_4dmin_pass";
                userManager.Create(rater, raterPassword);

                var client = new ApplicationUser() { Email = "clientmail@gmail.com", UserName = "clientmail@gmail.com" };
                var clientPassword = "Gr34dy_4dmin_pass";
                userManager.Create(client, clientPassword);

                userManager.AddToRole(admin.Id, adminRole.Name);
                userManager.AddToRole(moderator.Id, moderatorRole.Name);
                userManager.AddToRole(rater.Id, raterRole.Name);
                userManager.AddToRole(client.Id, clientRole.Name);

                base.Seed(context);
            }
        }
    }
}