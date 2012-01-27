using System.Web.Mvc;

namespace TrackViewer.Areas.Calendar
{
	public class CalendarAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Calendar";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Calendar_default",
				"Calendar/{month}/{day}/{year}",
				new { controller = "Calendar", action = "Calendar", month = 7, day = 24, year = 2011 }
			);
		}
	}
}
