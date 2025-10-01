using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ICMS.App_Start
{
    public class IsAuthorized : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var MySession = HttpContext.Current.Session;
            string contoller = filterContext.Controller.ToString();
            if (MySession["Role"].ToString() == "INT")
            {
                if (contoller != "ICMS.Controllers.InitiatorController")
                {
                    //filterContext.Result = new RedirectResult(string.Format("Shared/Error"));
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }
            }
            if (MySession["Role"].ToString() == "QCP")
            {
                if (contoller == "ICMS.Controllers.RequestPlannerController" || contoller == "ICMS.Controllers.PlannerExternalRequestController" || contoller == "ICMS.Controllers.InitiatorController" || contoller == "ICMS.Controllers.InspectorController")
                {
                    //if (contoller != "ICMS.Controllers.PlannerExternalRequestController")
                    //{
                    //    //filterContext.Result = new RedirectResult(string.Format("Shared/Error"));
                    //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                    //}
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }

            }
            if (MySession["Role"].ToString() == "QCI")
            {
                if (contoller != "ICMS.Controllers.InspectorController")
                {
                    //filterContext.Result = new RedirectResult(string.Format("Shared/Error"));
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }
            }
            if (MySession["Role"].ToString() == "HOD")
            {
                if (contoller != "ICMS.Controllers.HODController")
                {
                    //filterContext.Result = new RedirectResult(string.Format("Shared/Error"));
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }
            }
        }
    }
}