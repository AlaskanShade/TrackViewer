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

namespace TrackViewer.Domain.Entities
{
	[Table]
	public class Track
	{
		[HiddenInput(DisplayValue=false)]
		[Column(IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.OnInsert)]
		public int TrackID { get; set; }
		[Column]
		public string SourceFile { get; set; }
		[Column]
		public string Name { get; set; }
		[Column]
		public int? TrimStart { get; set; }
		[Column]
		public int? TrimEnd { get; set; }
		[ScaffoldColumn(false)]
		[Column]
		public string Data { get; set; }

		private TrackMetadata _metadata = null;

		public static List<Track> ParseGpx(string filename, Stream data)
		{
			var doc = XDocument.Load(data);
			var xn = doc.Root.Name.Namespace;
			List<Track> tracks = new List<Track>();
			// Read 'trk' elements.  Could also read 'wpt' and 'rte'
			foreach (var trk in doc.Root.Elements(xn + "trk"))
				tracks.Add(new Track { SourceFile = filename, Data = trk.ToString(SaveOptions.DisableFormatting), Name = trk.Element(xn + "name").Value });
			return tracks;
		}

		public TrackPoint[] GetPoints()
		{
			var data = XElement.Parse(Data);
			var xn = data.Name.Namespace;
			return (from pt in data.Descendants(xn + "trkpt") 
					select new TrackPoint 
					{ 
						Latitude = decimal.Parse(pt.Attribute("lat").Value),
						Longitude = decimal.Parse(pt.Attribute("lon").Value),
						Elevation = decimal.Parse(pt.Element(xn + "ele").Value),
						When = DateTime.Parse(pt.Element(xn + "time").Value)
					}).ToArray();
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

		public TrackMetadata GetGmapData()
		{
			if (_metadata != null) return _metadata;
			var points = GetPoints();
			_metadata = new TrackMetadata { 
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

		public class TrackMetadata
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

			public TrackMetadata()
			{
				North = -90;
				South = 90;
				East = -180;
				West = 180;
			}
		}
	}
}
