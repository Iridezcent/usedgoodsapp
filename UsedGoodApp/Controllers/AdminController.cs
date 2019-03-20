using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsedGoodApp.Models;
using static UsedGoodApp.Models.IdentityUserModel;
using UsedGoodApp.Infrastructure;

namespace UsedGoodApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IdentityUserContext db = new IdentityUserContext();

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;           
            filterContext.Result = RedirectToAction("Index", "Error");   
        }

        // GET: Admin
        public ActionResult Index()
        {
            List<UserView> users = new List<UserView>();
            foreach (var user in db.Users.ToList())
            {
                users.Add(CreateUserView(user));
            }
            return View(users);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(CreateUserView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
                if (string.IsNullOrEmpty(user.UserName))
                    user.UserName = model.Email;
                //user.Roles.Add(new ide)
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    ApplicationUser _user = await UserManager.FindByEmailAsync(model.Email);
                    UserManager.AddToRole(_user.Id, model.SelectedRole);
                    //await UserManager.SendEmailAsync(_user.Id, $"Password for user: {_user.UserName} ", $"Login : {_user.UserName} /n Password : {model.Password}" );
                    GMailer mailer = new GMailer();
                    mailer.ToEmail = model.Email;
                    mailer.Subject = "Спасибо за регистрацию";
                    mailer.Body = $"Логин : {_user.UserName}; <br/> Пароль : {model.Password} ";
                    mailer.IsHtml = true;
                    mailer.Send();

                    return RedirectToAction("Index");
                }                                            
                else
                    ModelState.AddModelError("", "Не удалось создать пользователя");
            }
            return View("Create");
        }

        // GET: Admin/Edit/5
        [ActionName("Edit")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                throw new HttpException(400, String.Empty);
            var _user = await UserManager.FindByIdAsync(id.ToString());
            if (_user == null)
                return HttpNotFound();
            EditableUserView _userView = CreateEditableUserView(_user);
            return View(_userView);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(EditableUserView model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                   
                    if (UserManager.HasPassword(user.Id))
                    {
                        if (!string.IsNullOrEmpty(model.Password))
                        {
                            UserManager.RemovePassword(user.Id);
                            UserManager.AddPassword(user.Id, model.Password);
                        }
                        user.Email = model.Email;
                        //user.UserName = model.UserName;
                        user.PhoneNumber = model.PhoneNumber;
                        UserManager.RemoveFromRole(model.Id, GetUserRoles(model.Email).FirstOrDefault());
                        UserManager.AddToRole(model.Id, model.SelectedRole);
                    }           
                    else
                    {
                        ModelState.AddModelError("", "Произошла ошибка. Попробуйте ещё раз.");
                    }
                    UserManager.Update(user);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", new { id = model.Id});
        }
        
        // GET: Admin/Delete/5
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
                throw new HttpException(400, String.Empty);
            if (id == User.Identity.GetUserId())
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                throw new HttpException(400, String.Empty);
            ApplicationUser _user = await UserManager.FindByIdAsync(id.ToString());
            if (_user == null)
                return HttpNotFound();
            var _userView = CreateUserView(_user);
            return View(_userView);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(string id)
        {
            string _userId = Convert.ToString(id);
            ApplicationUser user = await UserManager.FindByIdAsync(_userId);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private UserView CreateUserView(ApplicationUser user)
        {
            UserView userView = new UserView
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                SelectedRole = GetUserRoles(user.Email).FirstOrDefault()
            };

            return userView;
        }

        private EditableUserView CreateEditableUserView(ApplicationUser user)
        {
            EditableUserView _view = new EditableUserView
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Roles = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem {Text = "Client", Value = "Client", Selected = true },
                    new SelectListItem {Text = "Admin", Value = "Admin", Selected = false},
                    new SelectListItem {Text = "Moderator", Value = "Moderator", Selected = false},
                    new SelectListItem {Text = "Rater", Value = "Rater", Selected = false},
                    new SelectListItem {Text = "Partner", Value = "Partner", Selected = false}
                }, "Value", "Text"),
                SelectedRole = GetUserRoles(user.Email).FirstOrDefault()
            };

            return _view;
        }


        private IEnumerable<string> GetUserRoles(string email)
        {
            IList<string> roles = new List<string>();
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var user = userManager.FindByEmail(email);
            if (user != null)
                roles = userManager.GetRoles(user.Id.ToString());
            return roles;
        }
        #endregion
    }
}
