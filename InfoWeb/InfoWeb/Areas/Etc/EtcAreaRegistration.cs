using System.Web.Http;
using System.Web.Mvc;

namespace InfoWeb.Areas.Etc
{
    public class EtcAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Etc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapHttpRoute(
                name: "EtcApiAction",
                routeTemplate: "etc/api/{controller}/{action}",
                // Optional: Added the following line to support default /ip/api access
                defaults: new { action = "Get", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            context.Routes.MapHttpRoute(
                name: "EtcApi",
                routeTemplate: "etc/api/{controller}",
                // Optional: Added the following line to support default /ip/api access
                defaults: new { action = "Get", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Etc_defaultAction_lang",
                "{lang}/Etc/{controller}/{action}/{id}",
                constraints: new { lang = @"(\w{2})|(\w{2}-\w{2})" },
                defaults: new { action = "Index", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Etc_defaultAction",
                "Etc/{controller}/{action}/{id}",
                new { action = "Index", area = AreaName, controller = AreaName, id = UrlParameter.Optional }
            );
            //context.MapRoute(
            //    "Etc_default",
            //    "Etc/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}