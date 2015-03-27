using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace MyProject.Helpers
{
    public static class UrlHelper2
    {
        public static string GetCurrentUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        public static string CurrentRouteID
        {
            get
            {
                RouteData currentRoute = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
                if (currentRoute == null) return string.Empty;
                FieldInfo field = typeof(RouteCollection).GetField("_namedMap", BindingFlags.Instance | BindingFlags.NonPublic);
                Dictionary<string, RouteBase> namedMap = field.GetValue(RouteTable.Routes) as Dictionary<string, RouteBase>;
                return namedMap.First(route => { return route.Value == currentRoute.Route; }).Key;
            }
        }
        public static string GetCurrentWebsiteRoot()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        }
        public static string GetCurrentQueryString()
        {
            return HttpContext.Current.Request.QueryString.ToString();
        }
        public static string GetPartialQuertString(string key)
        {
            return HttpContext.Current.Request.QueryString[key];
        }

    }
}
 
