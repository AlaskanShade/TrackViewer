using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackViewer.Domain.Abstract;
using TrackViewer.Domain.Entities;
using System.IO;

namespace TrackViewer.Domain.Mock
{
	public class FakeTracksRepository : ITracksRepository
	{
		private static IQueryable<Track> fakeTracks;

		static FakeTracksRepository()
		{
			var data = File.OpenRead(@"C:\Users\michael\Documents\My Dropbox\GPS\Tracks\20110530.gpx");
			fakeTracks = Track.ParseGpx("20110530.gpx", data).AsQueryable();
		}

		#region ITracksRepository Members

		public IQueryable<Track> Tracks
		{
			get { return fakeTracks; }
		}

		public void AddTrack(Track t)
		{
			throw new NotImplementedException();
		}

		public void SaveTrimTrack(Track t)
		{
			throw new NotImplementedException();
		}

		public void SaveTrack(Track t)
		{
			throw new NotImplementedException();
		}

		public void DeleteTrack(int trackId)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
