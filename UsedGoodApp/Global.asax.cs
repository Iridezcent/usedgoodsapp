using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using static UsedGoodApp.Models.IdentityUserModel;

namespace UsedGoodApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new IdentityUserDbInitializer());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Application_Error()
        //{
        //    Exception exception = Server.GetLastError();
        //    Response.Clear();

        //    HttpException httpException = exception as HttpException;

        //    if (httpException != null)
        //    {
        //        string action;

        //        switch (httpException.GetHttpCode())
        //        {
        //            case 400:
        //                // bad request
        //                action = "HttpError400";
        //                break;
        //            case 404:
        //                // page not found
        //                action = "HttpError404";
        //                break;
        //            case 500:
        //                // server error
        //                action = "HttpError500";
        //                break;
        //            default:
        //                action = "Index";
        //                break;
        //        }

        //        // clear error on server
        //        Server.ClearError();

        //        Response.Redirect($"/Error/{action}");
        //    }
        //}
    }
}
