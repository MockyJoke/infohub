using System.Web.Http;
using System.Web.Mvc;

namespace InfoWeb.Areas.IP
{
    public class IPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.Routes.MapHttpRoute(
                name: "IPApiAction",
                routeTemplate: "ip/api/{controller}/{action}",
                // Optional: Added the following line to support default /ip/api access
                defaults: new { action = "Get", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            context.Routes.MapHttpRoute(
                name: "IPApi",
                routeTemplate: "ip/api/{controller}",
                // Optional: Added the following line to support default /ip/api access
                defaults: new { action = "Get", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            context.MapRoute(
                "IP_defaultAction_lang",
                "{lang}/ip/{controller}/{action}/{id}",
                constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" },
                defaults: new { action = "Index", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            context.MapRoute(
                "IP_defaultAction",
                "ip/{controller}/{action}/{id}",
                new { action = "Index", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            //context.MapRoute(
            //    "IP_default",
            //    "ip/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}