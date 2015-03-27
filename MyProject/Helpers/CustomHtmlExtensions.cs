using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProject.Helpers
{
	public static class CustomHtmlExtensions
	{
		public static bool IsCurrentController(this HtmlHelper helper,
			string controllerName)
		{
			var currentControllerName =
				(string)helper.ViewContext.RouteData.Values["controller"];

			if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}
			return false;
		}

		public static bool IsCurrentAction(this HtmlHelper helper,
			string actionName,
			string controllerName)
		{
			var currentControllerName =
				(string)helper.ViewContext.RouteData.Values["controller"];

			var currentActionName =
				(string)helper.ViewContext.RouteData.Values["action"];

			if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
				&&
				currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}
			return false;
		}

		public static bool IsCurrentPage(this HtmlHelper helper,
			string actionName,
			string controllerName,
			string IdName)
		{
			var currentControllerName =
				(string)helper.ViewContext.RouteData.Values["controller"];

			var currentActionName =
				(string)helper.ViewContext.RouteData.Values["action"];

			var currentIdName =
				(string)helper.ViewContext.RouteData.Values["id"];

			if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
				&&
				currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase)
				&&
				currentIdName.Equals(IdName, StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}
			return true;
		}

	}
}
 
