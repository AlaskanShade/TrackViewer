using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace TrackViewer.Domain.Entities
{
	public class TrackPoint
	{
		public decimal Longitude { get; set; }
		public decimal Latitude { get; set; }
		public decimal Elevation { get; set; }
		public DateTime When { get; set; }
	}
}
