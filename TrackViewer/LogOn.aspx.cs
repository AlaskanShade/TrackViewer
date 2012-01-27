using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace TrackViewer
{
	public partial class LogOn : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void login1_Authenticate(object sender, AuthenticateEventArgs e)
		{
			if (FormsAuthentication.Authenticate(login1.UserName, login1.Password))
			{
				e.Authenticated = true;
				FormsAuthentication.SetAuthCookie(login1.UserName, true);
				FormsAuthentication.RedirectFromLoginPage(login1.UserName, true);
			}
			else
				e.Authenticated = false;
		}
	}
}