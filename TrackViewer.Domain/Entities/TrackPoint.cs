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

		public double DistanceBetween(TrackPoint point)
		{
			double dLat = DegreesToRadians(Latitude - point.Latitude);
			double dLon = DegreesToRadians(Longitude - point.Longitude);
			double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					  Math.Cos(DegreesToRadians(point.Latitude)) * Math.Cos(DegreesToRadians(Latitude)) *
					  Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return 6371000 * c;
		}

		private static double DegreesToRadians(decimal degrees)
		{
			return (Math.PI * (double)degrees) / 180;
		} 
	}
}
