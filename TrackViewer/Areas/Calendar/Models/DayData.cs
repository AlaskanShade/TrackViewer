using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrackViewer.Areas.Calendar.Models
{
	public enum LineColor { White, Blue, Red, Yellow, Green }

	public class DayCollection
	{
		[XmlArrayItem]
		public List<DayData> Data { get; set; }

		public DayData GetData(DateTime index)
		{
			var ret = Data.FirstOrDefault(d => d.Day == index);
			if (ret == null)
				return new DayData { Day = index, Lines = new List<DayLine>() };
			return ret;
		}

		public DayCollection()
		{
			Data = new List<DayData>();
		}
	}

	public class DayData
	{
		[XmlAttribute]
		public DateTime Day { get; set; }
		[XmlArrayItem]
		public List<DayLine> Lines { get; set; }
		[XmlIgnore]
		public List<TrackViewer.Domain.Entities.Track> Tracks { get; set; }

		public DayData()
		{
			Lines = new List<DayLine>();
			Tracks = new List<Domain.Entities.Track>();
		}
	}

	public class DayLine
	{
		[XmlAttribute]
		public LineColor Color { get; set; }
		[XmlAttribute]
		public string Text { get; set; }

		public DayLine()
		{
			Color = LineColor.White;
		}

		public DayLine(string line) : this()
		{
			if (!line.StartsWith("#"))
				Text = line;
			else
			{
				switch (line[1])
				{
					case 'B':
						Color = LineColor.Blue;
						break;
					case 'R':
						Color = LineColor.Red;
						break;
					case 'Y':
						Color = LineColor.Yellow;
						break;
					case 'G':
						Color = LineColor.Green;
						break;
				}
			}
			Text = line.Substring(3);
		}
            
		public override string ToString()
		{
			if (Color == LineColor.White)
				return Text;
			return String.Format("#{0}#{1}", Color.ToString()[0], Text);
		}

		public object GetDisplay()
		{
			if (String.IsNullOrWhiteSpace(Text)) return new HtmlString("&nbsp;");
			return Text;
		}
	}
}