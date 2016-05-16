using System.Web.Http;
using System.Web.Mvc;

namespace InfoWeb.Areas.GMD
{
    // Tutorial for WEBAPI2 Areas:http://shashangka.com/2016/01/30/asp-net-mvc5-routing-with-web-api2-area/
    public class GMDAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GMD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //****************=======Default Api Route=========*******************

            context.Routes.MapHttpRoute(
                name: "GMDApiAction",
                routeTemplate: "GMD/api/{controller}/{action}"
            );

            context.Routes.MapHttpRoute(
                name: "GMDApi",
                routeTemplate: "GMD/api/{controller}"
            );

            //****************=======Default Route=========***********************


            context.MapRoute(
                "GMD_default",
                "GMD/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}