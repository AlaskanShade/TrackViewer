using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackViewer.Domain;
using TrackViewer.Domain.Abstract;
using TrackViewer.Domain.Mock;
using TrackViewer.Domain.Concrete;
using System.Web.Configuration;
using TrackViewer.Domain.Entities;


namespace TrackViewer.Controllers
{
	//[Authorize(Users="Shade")]
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
            return View(_tracksRepository.Tracks.OrderByDescending(t => t.TrackDate).ToList());
        }

		// TODO: trim charts
		// TODO: Add markers on map for each hour/15 min/etc.
		[HttpGet]
		public ActionResult View(int id)
		{
			Track trk = _tracksRepository.Tracks.Where(t => t.TrackID == id).FirstOrDefault();
			trk.GenerateMetadata();
			ViewData["TravelMethods"] = trk.TypeOfTravel.ToSelectList();
			return View(trk);
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			Track trk = _tracksRepository.Tracks.Where(t => t.TrackID == id).FirstOrDefault();
			trk.GenerateMetadata();
			ViewData["TravelMethods"] = trk.TypeOfTravel.ToSelectList();
			return View(trk);
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

		public ActionResult UpdateMetadata()
		{
			foreach (var trk in _tracksRepository.Tracks)
			{
				var t = trk.Clone();
				t.GenerateMetadata();
				_tracksRepository.SaveTrack(t);
			}
			return new RedirectResult("List");
		}

		[Authorize(Roles="Admin")]
		[HttpGet]
		public ActionResult Upload()
		{
			return View();
		}

		[Authorize(Roles="Admin")]
		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase gpx)
		{
			string source = System.IO.Path.GetFileName(gpx.FileName);
			//if (_tracksRepository.Tracks.Count(trk => trk.SourceFile == source) > 0)
			//{
			//    ViewData["message"] = "File already uploaded";
			//    return View();
			//}
			foreach (var t in Track.ParseGpx(source, gpx.InputStream))
			{
				t.GenerateMetadata();
				_tracksRepository.AddTrack(t);
			}
			return RedirectToAction("List");
		}

		[Authorize(Roles="Admin")]
		public ActionResult Delete(int trackId)
		{
			_tracksRepository.DeleteTrack(trackId);
			return RedirectToAction("List");
		}

		
		public JsonResult TrackData(int id, string type)
		{
			var trk = _tracksRepository.Tracks.FirstOrDefault(t => t.TrackID == id);
			if (trk == null) return Json(null);
			return Json(trk.GetTrackData(type), JsonRequestBehavior.AllowGet);
		}
    }
}
