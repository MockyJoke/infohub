﻿using InfoWeb.App;
using System.Web;
using System.Web.Mvc;

namespace InfoWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LocalizationAttribute("en"), 0);
        }
    }
}
