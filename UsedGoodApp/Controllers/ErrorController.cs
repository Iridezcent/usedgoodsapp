using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UsedGoodApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult HttpError404()
        {
            return View("Error");
        }

        public ActionResult HttpError500()
        {
            return View("Error");
        }

        public ActionResult HttpError400()
        {
            return View("Error");
        }

        public ActionResult Index()
        {
            return View("Error");
        }
    }
}