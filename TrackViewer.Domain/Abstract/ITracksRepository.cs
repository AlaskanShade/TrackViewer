using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackViewer.Domain.Entities;

namespace TrackViewer.Domain.Abstract
{
	public interface ITracksRepository
	{
		IQueryable<Track> Tracks { get; }

		void AddTrack(Track t);
		void SaveTrack(Track t);
		void SaveTrimTrack(Track t);
		void DeleteTrack(int trackId);
	}
}
