using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrackViewer.Domain.Entities
{
	public class TrackCollection
	{
		[XmlArrayItem(typeof(Track))]
		public List<Track> Tracks { get; set; }
	}
}
