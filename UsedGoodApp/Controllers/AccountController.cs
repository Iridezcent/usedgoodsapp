using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UsedGoodApp.Infrastructure;
using UsedGoodApp.Models;
using static UsedGoodApp.Models.IdentityUserModel;

namespace UsedGoodApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                //return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public ActionResult Register()
        {
            return View();
        }
        
        //[HttpPost]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        IdentityResult result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            GMailer mailer = new GMailer();
        //            mailer.ToEmail = model.Email;
        //            mailer.Subject = "Спасибо за регистрацию";
        //            mailer.Body = $"Логин : {model.Email}; \nПароль : {model.Password}";
        //            mailer.IsHtml = true;
        //            mailer.Send();
        //            return RedirectToAction("Login", "Account");
        //        }
        //        else
        //        {
        //            foreach (string error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error);
        //            }
        //        }
        //    }
        //    return View(model);
        //}

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Сбои при входе не приводят к блокированию учетной записи
            // Чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.IsPersistent, shouldLockout: false);            
            switch (result)
            {
                case SignInStatus.Success:
                    if (string.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Index", "Product");
                    return Redirect(returnUrl);
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неудачная попытка входа.");
                    ViewBag.returnUrl = returnUrl;
                    return View(model);
            }
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index","Product");
        }

        //[HttpGet]
        //public ActionResult Delete()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ActionName("Delete")]
        //public async Task<ActionResult> DeleteConfirmed()
        //{
        //    ApplicationUser user = await UserManager.FindByEmailAsync(User.Identity.Name);
        //    if (user != null)
        //    {
        //        IdentityResult result = await UserManager.DeleteAsync(user);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Logout", "Account");
        //        }
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return View("ForgotPasswordConfirmation");
                }
                //Token Provider
                //var provider = new DpapiDataProtectionProvider("UsedGoodApp");
                //UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("UsedGoodToken"));

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Сброс пароля",
                //    "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>");
                GMailer mailer = new GMailer();
                mailer.ToEmail = model.Email;
                mailer.Subject = "Сброс пароля";
                mailer.Body = "Ссылка для сброса пароля : <a href =\""+ callbackUrl +"\">"+ callbackUrl+ "</a> ";
                mailer.IsHtml = true;
                mailer.Send();
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Не показывать, что пользователь не существует
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            //AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //public async Task<ActionResult> Edit()
        //{
        //    ApplicationUser user = await UserManager.FindByEmailAsync(User.Identity.Name);
        //    if (user != null)
        //    {
        //        //EditModel model = new EditModel();
        //        return View(model);
        //    }
        //    return RedirectToAction("Login", "Account");
        //}

        //[HttpPost]
        //public async Task<ActionResult> Edit(EditModel model)
        //{
        //    ApplicationUser user = await UserManager.FindByEmailAsync(User.Identity.Name);
        //    if (user != null)
        //    {
        //        IdentityResult result = await UserManager.UpdateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Что-то пошло не так");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Пользователь не найден");
        //    }

        //    return View(model);
        //}
    }
}