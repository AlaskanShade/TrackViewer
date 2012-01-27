using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;

namespace TrackViewer.Areas.Calendar.Models
{
	public class CalendarRepository
	{
		private string _path;
		private DayCollection _collection;

		public CalendarRepository(string path)
		{
			_path = Path.Combine(path, "calendar.xml");
			if (File.Exists(_path))
			{
				var xSer = new XmlSerializer(typeof(DayCollection));
				using (var fs = File.OpenRead(_path))
				{
					_collection = xSer.Deserialize(fs) as DayCollection;
				}
			}
			else
				_collection = new DayCollection();
		}

		public DayCollection GetData(DateTime startDay, int days)
		{
			if (startDay.DayOfWeek != DayOfWeek.Sunday)
				startDay = startDay.AddDays((int)startDay.DayOfWeek * -1);
			return new DayCollection { Data = _collection.Data.Where(d => d.Day >= startDay && d.Day <= startDay.AddDays(days)).ToList() };
		}

		public DayData UpdateDay(DateTime day, string data)
		{
			var lineStrings = data.Split('\n');
			List<DayLine> lines = new List<DayLine>();
			foreach (var lineString in lineStrings)
				lines.Add(new DayLine(lineString.Trim()));
			var existing = _collection.Data.FirstOrDefault(d => d.Day == day);
			if (existing == null)
			{
				existing = new DayData { Day = day, Lines = lines };
				_collection.Data.Add(existing);
			}
			else
				existing.Lines = lines;
			var xSer = new XmlSerializer(typeof(DayCollection));
			using (var fs = File.Open(_path, FileMode.Create))
			{
				xSer.Serialize(fs, _collection);
			}
			return existing;
		}
	}
}