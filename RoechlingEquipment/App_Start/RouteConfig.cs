using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RoechlingEquipment
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
               name: "LoginPage",
               url: "{culture}/login.html",
               defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Login", action = "LoginPage" });

            routes.MapRoute(
               name: "Index",
               url: "{culture}/index.html",
               defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index" });

            routes.MapRoute(
                name: "Create",
                url: "{culture}/create.html",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Create" });

            routes.MapRoute(
                name: "AdSetIndex",
                url: "{culture}/adsetindex.html",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "AdSetIndex" });

            routes.MapRoute(
                name: "AdSetUsers",
                url: "{culture}/setusers.html",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "AdSetUsers" });

            routes.MapRoute(
              name: "AdSetDepartments",
              url: "{culture}/setdepartments.html",
              defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "AdSetDepartments" });

            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { culture = CultureHelper.GetDefaultCulture(), controller = "Home", action = "Index", id = UrlParameter.Optional });

        }
    }
}