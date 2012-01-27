using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Xml.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TrackViewer.Domain.Entities
{
	public enum TravelType { NotSet, Hike, Bike, Car, Boat }

	[Table]
	public class Track
	{
		private TimeSpan? _time;

		[HiddenInput(DisplayValue=false)]
		[Column(IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.OnInsert)]
		[XmlAttribute]
		public int TrackID { get; set; }
		[Column]
		[XmlAttribute]
		public string SourceFile { get; set; }
		[Column]
		[XmlAttribute]
		public string Name { get; set; }
		[Column]
		public int? TrimStart { get; set; }
		[Column]
		public int? TrimEnd { get; set; }
		[ScaffoldColumn(false)]
		[Column]
		[XmlIgnore]
		public string Data { get; set; }
		[XmlAttribute]
		[Column]
		public DateTime TrackDate { get; set; }
		[XmlAttribute]
		[Column]
		public int PointCount { get; set; }
		[Display(Name="Total Distance")]
		[XmlAttribute]
		[Column]
		public double TotalDistance { get; set; }
		[Display(Name="Total Time")]
		[XmlIgnore]
		[Column]
		public TimeSpan TotalTime { get { if (_time == null) return TimeSpan.Zero; return _time.Value; } set { _time = value; } }
		[XmlAttribute("Time")]
		public string TotalTimeXml { get { return TotalTime.ToString(); } set { TotalTime = TimeSpan.Parse(value); } }
		[Display(Name="Travel Method")]
		[XmlAttribute]
		[Column]
		public TravelType TypeOfTravel { get; set; }

		private GmapMetadata _metadata = null;
		private TrackPoint[] _points;
		private ILoadData _dataLoader;

		public void SetDataLoader(ILoadData data)
		{
			_dataLoader = data;
		}

		public static List<Track> ParseGpx(string filename, Stream data)
		{
			var doc = XDocument.Load(data);
			var xn = doc.Root.Name.Namespace;
			List<Track> tracks = new List<Track>();
			// Read 'trk' elements.  Could also read 'wpt' and 'rte'
			foreach (var trk in doc.Root.Elements(xn + "trk"))
			{
				var newDoc = new XDocument(doc);
				newDoc.Declaration = new XDeclaration(doc.Declaration);
				newDoc.Root.ReplaceNodes(new XElement(trk));
				var newTrack = new Track { SourceFile = filename, Data = newDoc.ToString(), Name = trk.Element(xn + "name").Value };
				var points = newTrack.GetPoints();
				if (points.Length > 10)
					tracks.Add(newTrack);
			}
			return tracks;
		}

		public TrackPoint[] GetPoints()
		{
			if (_points != null) return _points;
			if (Data == null && _dataLoader != null) Data = _dataLoader.GetData(this);
			if (Data == null) return new TrackPoint[] { };
			var data = XElement.Parse(Data);
			var xn = data.Name.Namespace;
			_points = (from pt in data.Descendants(xn + "trkpt") 
					select new TrackPoint 
					{ 
						Latitude = decimal.Parse(pt.Attribute("lat").Value),
						Longitude = decimal.Parse(pt.Attribute("lon").Value),
						Elevation = decimal.Parse(pt.Element(xn + "ele").Value),
						When = DateTime.Parse(pt.Element(xn + "time").Value)
					}).ToArray();
			return _points;
		}

		public void GenerateMetadata()
		{
			var points = GetPoints();
			if (points.Length == 0) return;
			TrackDate = points[0].When;
			PointCount = points.Length;
			TotalTime = points.Last().When - points.First().When;
			GetDistance();
		}

		public double GetDistance()
		{
			var points = GetPoints();
			if (points.Length == 0) return 0;
			var prev = points[0];
			TotalDistance = 0;
			Array.ForEach(points.Skip(1).ToArray(), pnt => { TotalDistance += pnt.DistanceBetween(prev); prev = pnt; });
			return TotalDistance;
		}

		public void TrimPoints(int? start, int? end)
		{
			var xn = XElement.Parse(Data).Name.Namespace;
			var points = GetPoints() as IEnumerable<TrackPoint>;
			if (start.HasValue)
				points = points.Skip(start.Value - 1);
			if (end.HasValue)
				points = points.Take((end ?? points.Count()) - (start ?? 0) + 1);
			Data = new XElement(xn + "trk", 
				new XElement(xn + "Name", Name),
				new XElement(xn + "trkseg",
					(from p in points 
					 select new XElement(xn + "trkpt", 
						 new XAttribute("lat", p.Latitude),
						 new XAttribute("lon", p.Longitude),
						 new XElement(xn + "ele", p.Elevation),
						 new XElement(xn + "time", p.When.ToString("u")))))).ToString();
			TrimStart = null;
			TrimEnd = null;
		}

		public double[] GetTimes()
		{
			var points = GetPoints();
			var first = points.First();
			return points.Select(pnt => (pnt.When - first.When).TotalSeconds).ToArray();
		}

		public string GetDistances()
		{
			var points = GetPoints();
			var prev = points.First();
			var total = 0d;
			var distances = points.Select(pnt => { total += pnt.DistanceBetween(prev); prev = pnt; return total / 1609.344; }).ToArray();
			return String.Format("[{0}]", String.Join(",", distances.Select(i => i.ToString())));
		}

		public GmapMetadata GetGmapData()
		{
			if (_metadata != null) return _metadata;
			var points = GetPoints();
			_metadata = new GmapMetadata { 
				StartLat = points[0].Latitude, 
				StartLon = points[0].Longitude, 
				EndLat = points[points.Length - 1].Latitude,
				EndLon = points[points.Length - 1].Longitude
			};
			int prev_x = 0, prev_y = 0, level_pos = 0;
			//string levelValues = "C@?@A@?@B@?@A@?@B@?@A@?@";
			StringBuilder sbEncode = new StringBuilder();
			//StringBuilder sbLevels = new StringBuilder();
			//for (int i = TrimStart ?? 0; i < (TrimEnd ?? points.Length); i++)
			for (int i = 0; i < points.Length; i++)
			{
				var point = points[i];
				if (i >= (TrimStart ?? 0) && i <= (TrimEnd ?? points.Length - 1))
				{
					_metadata.North = Math.Max(_metadata.North, point.Latitude);
					_metadata.South = Math.Min(_metadata.South, point.Latitude);
					_metadata.East = Math.Max(_metadata.East, point.Longitude);
					_metadata.West = Math.Min(_metadata.West, point.Longitude);
				}

				int x = Convert.ToInt32(point.Latitude * 100000);
				int y = Convert.ToInt32(point.Longitude * 100000);

				int delta_x = x - prev_x;
				int delta_y = y - prev_y;

				prev_x = x; prev_y = y;

				sbEncode.Append(TrackViwer.Domain.GeoUtil.Encode(delta_x).Replace("\\", "\\\\"));
				sbEncode.Append(TrackViwer.Domain.GeoUtil.Encode(delta_y).Replace("\\", "\\\\"));
				//sbLevels.Append(levelValues[level_pos]);
				//level_pos++;
				//if (level_pos == levelValues.Length) level_pos = 0;
			}
			_metadata.EncodedPolyline = sbEncode.ToString();
			return _metadata;
		}

		public dynamic GetTrackData(string type)
		{
			var points = GetPoints();
			var data = new List<object[]>();
			string title = null;
			if (type == "Elevation")
			{
				title = "Elevation";
				// elevation is in meters
				foreach (var point in points)
					data.Add(new object[] { point.When.ToString("yyyy-MM-dd HH:mm:ss"), 3.2808M * point.Elevation });
			}
			else if (type == "Speed")
			{
				title = "Speed";
				var previous1 = points[0];
				var previous2 = points[1];
				var prevDistance = previous2.DistanceBetween(previous1);
				var prevTime = (previous2.When - previous1.When).TotalHours;
				for (int i = 2; i < points.Length; i++)
				{
					var point = points[i];
					// distance in meters
					var distance = point.DistanceBetween(previous2);
					// time in hours
					var time = (point.When - previous2.When).TotalHours;
					data.Add(new object[] { point.When.ToString("yyyy-MM-dd HH:mm:ss"), ((distance + prevDistance) / 1609.344) / (time + prevTime) });
					previous1 = previous2;
					previous2 = point;
					prevDistance = distance;
					prevTime = time;
				}
			}
			var span = points.Last().When - points.First().When;
			var ticks = (span.TotalHours < 1 ? "5 minutes" : (span.TotalHours < 2 ? "10 minutes" : (span.TotalHours < 3 ? "15 minutes" : (span.TotalHours < 6 ? "30 minutes" : "1 hour"))));
			return new { Title = title, MinX = data.First()[0], MaxX = data.Last()[0], Data = data, Tick = ticks };
		}

		public class GmapMetadata
		{
			public decimal North { get; set; }
			public decimal South { get; set; }
			public decimal East { get; set; }
			public decimal West { get; set; }
			public decimal StartLat { get; set; }
			public decimal StartLon { get; set; }
			public decimal EndLat { get; set; }
			public decimal EndLon { get; set; }
			public string EncodedPolyline { get; set; }

			public GmapMetadata()
			{
				North = -90;
				South = 90;
				East = -180;
				West = 180;
			}
		}

		public interface ILoadData
		{
			string GetData(Track t);
		}

		#region ICloneable Members

		public Track Clone()
		{
			return new Track { Data = this.Data, Name = this.Name, PointCount = this.PointCount, SourceFile = this.SourceFile, TotalDistance = this.TotalDistance, TotalTime = this.TotalTime, TrackDate = this.TrackDate, TrackID = this.TrackID, TrimStart = this.TrimStart, TrimEnd = this.TrimEnd, TypeOfTravel = this.TypeOfTravel };
		}

		#endregion
	}
}
