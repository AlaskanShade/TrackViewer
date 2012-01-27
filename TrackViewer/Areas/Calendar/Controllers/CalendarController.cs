using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackViewer.Areas.Calendar.Models;
using TrackViewer.Domain.Abstract;

namespace TrackViewer.Areas.Calendar.Controllers
{
    public class CalendarController : Controller
    {
		private ITracksRepository _tracks;

        public CalendarController(ITracksRepository tracks)
		{
			_tracks = tracks;
		}

		public ActionResult Calendar(int month, int day, int year)
		{
			DateTime startDate = new DateTime(year, month, day);
			ViewData["date"] = startDate;
			var repo = new CalendarRepository(Server.MapPath("~/App_Data"));
			var data = repo.GetData(startDate, 35);
			var tracks = (from t in _tracks.Tracks where t.TrackDate >= startDate && t.TrackDate <= startDate.AddDays(35) select t);
			foreach (var d in Enumerable.Range(0, 35).Select(i => startDate.AddDays(i)))
			{
				var dayTracks = (from t in tracks where t.TrackDate >= d && t.TrackDate <= d.AddDays(1) select t).ToList();
				if (dayTracks != null && dayTracks.Count > 0)
				{
					var dayData = (from dd in data.Data where dd.Day == d select dd).FirstOrDefault();
					if (dayData == null)
					{
						dayData = new DayData { Day = d, Tracks = dayTracks };
						data.Data.Add(dayData);
					}
					dayData.Tracks = dayTracks;
				}
			}
			return View(data);
		}

		[Authorize(Users="Shade")]
		public ActionResult UpdateDay(int month, int day, int year, string data)
		{
			var repo = new CalendarRepository(Server.MapPath("~/App_Data"));
			var dayData = repo.UpdateDay(new DateTime(year, month, day), data);
			//return PartialView("Day", dayData);
			return RedirectToAction("Calendar");
		}
    }
}
