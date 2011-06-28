using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackViewer.Domain.Abstract;
using TrackViewer.Domain.Entities;
using System.Data.Linq;

namespace TrackViewer.Domain.Concrete
{
	public class TracksRepository : ITracksRepository
	{
		private Table<Track> tracks;

		public TracksRepository(string connectionString)
		{
			tracks = (new DataContext(connectionString)).GetTable<Track>();
		}

		#region ITracksRepository Members

		public IQueryable<Track> Tracks
		{
			get { return tracks; }
		}

		public void AddTrack(Track t)
		{
			tracks.InsertOnSubmit(t);
			tracks.Context.SubmitChanges();
		}

		public void SaveTrack(Track t)
		{
			t.Data = (from trk in tracks where trk.TrackID == t.TrackID select trk.Data).First();
			tracks.Attach(t);
			tracks.Context.Refresh(RefreshMode.KeepCurrentValues, t);
			tracks.Context.SubmitChanges();
		}

		public void SaveTrimTrack(Track t)
		{
			t.Data = (from trk in tracks where trk.TrackID == t.TrackID select trk.Data).First();
			t.TrimPoints(t.TrimStart, t.TrimEnd);
			tracks.Attach(t);
			tracks.Context.Refresh(RefreshMode.KeepCurrentValues, t);
			tracks.Context.SubmitChanges();
		}

		public void DeleteTrack(int trackId)
		{
			var track = tracks.First(t => t.TrackID == trackId);
			tracks.DeleteOnSubmit(track);
			tracks.Context.SubmitChanges();
		}

		#endregion
	}
}
