using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Xml.Serialization;
using TrackViewer.Domain.Entities;

namespace TrackViewer.Domain.Concrete
{
	public class TracksFileRepository : TrackViewer.Domain.Abstract.ITracksRepository, Track.ILoadData
	{
		private List<Entities.Track> _tracks;
		private string _filePath;
		private int _maxId;

		public TracksFileRepository(string filePath)
		{
			var context = HttpContext.Current;
			_filePath = filePath;
			if (context.Session == null || context.Session["TrackData"] == null)
			{
				if (File.Exists(Path.Combine(filePath, "TrackData.xml")))
				{
					var xSer = new XmlSerializer(typeof(TrackCollection));
					using (var fs = File.OpenRead(Path.Combine(filePath, "TrackData.xml")))
					{
						var data = xSer.Deserialize(fs) as TrackCollection;
						_tracks = data.Tracks;
						fs.Close();
					}
				}
				else
					_tracks = new List<Track>();
				if (context.Session != null)
					context.Session["TrackData"] = _tracks;
			}
			else
				_tracks = context.Session["TrackData"] as List<Track>;
			_maxId = _tracks.Count == 0 ? 0 : _tracks.Max(t => t.TrackID);
			_tracks.ForEach(trk => trk.SetDataLoader(this));
		}

		private string GetTrackFilename(Track t)
		{
			return Path.Combine(_filePath, String.Format("{0:yyyyMMdd}-{1}.gpx", t.TrackDate, String.Join("-", t.Name.Split(Path.GetInvalidFileNameChars()))));
		}

		private void DeleteFile(Track t)
		{
			File.Delete(GetTrackFilename(t));
		}

		private void SaveData(Track t)
		{
			using (var fs = File.Open(Path.Combine(_filePath, "TrackData.xml"), FileMode.Create, FileAccess.Write))
			{
				var xSer = new XmlSerializer(typeof(TrackCollection));
				xSer.Serialize(fs, new TrackCollection { Tracks = _tracks });
				fs.Close();
			}
		}

		#region ITracksRepository Members

		public IQueryable<Entities.Track> Tracks
		{
			get { return _tracks.AsQueryable(); }
		}

		public void AddTrack(Entities.Track t)
		{
			var existing = (from trk in _tracks where trk.SourceFile == t.SourceFile && trk.Name == t.Name && trk.TrackDate == t.TrackDate select t).FirstOrDefault();
			if (existing == null)
			{
				t.TrackID = ++_maxId;
				t.SetDataLoader(this);
				_tracks.Add(t);
				SaveData(t);
			}
			if (!File.Exists(GetTrackFilename(t)))
				File.WriteAllText(GetTrackFilename(t), t.Data);
		}

		public void SaveTrack(Entities.Track t)
		{
			var current = _tracks.FirstOrDefault(trk => trk.TrackID == t.TrackID);
			if (current == null) return;
			current.Name = t.Name;
			current.TrimStart = t.TrimStart;
			current.TrimEnd = t.TrimEnd;
			current.TypeOfTravel = t.TypeOfTravel;
			current.TotalDistance = t.TotalDistance;
			SaveData(t);
		}

		public void SaveTrimTrack(Entities.Track t)
		{
			var current = _tracks.FirstOrDefault(trk => trk.TrackID == t.TrackID);
			if (current == null) return;
			current.Name = t.Name;
			current.TypeOfTravel = t.TypeOfTravel;
			current.TotalDistance = t.TotalDistance;
			current.TrimPoints(t.TrimStart, t.TrimEnd);
			t.GenerateMetadata();
			SaveData(t);
			DeleteFile(current);
			File.WriteAllText(GetTrackFilename(t), t.Data);
		}

		public void DeleteTrack(int trackId)
		{
			var current = _tracks.First(t => t.TrackID == trackId);
			_tracks.Remove(current);
			DeleteFile(current);
		}

		#endregion

		#region ILoadData Members

		public string GetData(Track t)
		{
			if (!File.Exists(GetTrackFilename(t)))
				return null;
			return File.ReadAllText(GetTrackFilename(t));
		}

		#endregion
	}
}
