using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using MyProject.Models;

namespace MyProject.Helpers
{
	public class WebSiteHelper
	{
		public static Guid CurrentUserID
		{
			get
			{
				var httpContext = HttpContext.Current;
				var identity = httpContext.User.Identity as FormsIdentity;

				if (identity == null)
				{
					return Guid.Empty;
				}

				return Guid.Parse(identity.Ticket.UserData); ;
			}
		}

		public static string CurrentUserName
		{
			get
			{
				var httpContext = HttpContext.Current;
				var identity = httpContext.User.Identity as FormsIdentity;

				return identity == null ? "���n�J" : identity.Name;
			}
		}

		public static string SystemUserName(Object id)
		{
			Guid systemUserID;
			if (Guid.TryParse(id.ToString(), out systemUserID))
			{
				if (systemUserID.Equals(Guid.Empty))
				{
					return "�t�ιw�]";
				}
				else
				{
					using (classRoomWebSiteDBContext db = new classRoomWebSiteDBContext())
					{
						var user = db.SystemUser.FirstOrDefault(x => x.ID == systemUserID);
						return (user != null) ? user.Name : "";
					}
				}
			}
			return "";
		}

	}
}
 
