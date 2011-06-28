using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackViewer.Domain.Abstract;
using TrackViewer.Domain.Mock;
using TrackViewer.Domain.Concrete;
using System.Web.Configuration;
using TrackViewer.Domain.Entities;


namespace TrackViewer.Controllers
{
    public class TrackController : Controller
    {
		private ITracksRepository _tracksRepository;

		public TrackController(ITracksRepository tracksRepository)
		{
			_tracksRepository = tracksRepository;
			//tracksRepository = new TracksRepository(WebConfigurationManager.ConnectionStrings["TrackStoreEntities"].ConnectionString);
		}

        public ActionResult List()
        {
            return View(_tracksRepository.Tracks.OrderBy(t => t.SourceFile).ToList());
        }

		[HttpGet]
		public ActionResult Edit(int id)
		{
			return View(_tracksRepository.Tracks.Where(t => t.TrackID == id).FirstOrDefault());
		}

		[HttpPost]
		public ActionResult Edit(Track t, string trim)
		{
			if (ModelState.IsValid)
			{
				if (trim != null)
					_tracksRepository.SaveTrimTrack(t);
				else
					_tracksRepository.SaveTrack(t);
				TempData["message"] = t.Name + " has been saved.";
				return RedirectToAction("List");
			}
			return View(t);
		}

		public ActionResult ImportAll()
		{
			int total = 0, imported = 0;
			foreach (var file in System.IO.Directory.GetFiles(@"C:\Users\michael\Documents\My Dropbox\GPS\Tracks", "201?????.gpx"))
			{
				var source = System.IO.Path.GetFileName(file);
				total++;
				if (_tracksRepository.Tracks.Count(trk => trk.SourceFile == source) == 0)
				{
					imported++;

					using (System.IO.FileStream fs = System.IO.File.OpenRead(file))
						foreach (var t in Track.ParseGpx(source, fs))
							if (t.GetPoints().Length > 10)
								_tracksRepository.AddTrack(t);
				}
			}
			return new RedirectResult("List");
		}

		[HttpGet]
		public ActionResult Upload()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase gpx)
		{
			string source = System.IO.Path.GetFileName(gpx.FileName);
			if (_tracksRepository.Tracks.Count(trk => trk.SourceFile == source) > 0)
			{
				ViewData["message"] = "File already uploaded";
				return View();
			}
            foreach (var t in TrackViewer.Domain.Entities.Track.ParseGpx(source, gpx.InputStream))
				_tracksRepository.AddTrack(t);
			return RedirectToAction("List");
		}

		public ActionResult Delete(int trackId)
		{
			_tracksRepository.DeleteTrack(trackId);
			return RedirectToAction("List");
		}
    }
}
