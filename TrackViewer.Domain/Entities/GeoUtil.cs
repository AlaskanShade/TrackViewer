using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TrackViwer.Domain
{
    /// <summary>
    /// Summary description for GUtil
    /// </summary>
    public static class GeoUtil
    {
        #region Parsing
        //private static string GetAttribute(string xml, string name, string attribute)
        //{
        //    int start = xml.IndexOf("<" + name);
        //    int end = xml.IndexOf(">", start);
        //    string node = xml.Substring(start, end - start);
        //    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(node, attribute + "\\s*=\"([^\"]*)\"");
        //    if (m.Success)
        //        return m.Groups[1].Value;
        //    return "";
        //}

        //private static string GetNodeText(string xml, string node)
        //{
        //    int start = xml.IndexOf("<" + node);
        //    int end = xml.IndexOf("</" + node + ">");
        //    return xml.Substring(start, end - start);
        //}

        public static string ParsePath(string xml)
        {
            Match m = Regex.Match(xml, "points:\\s*\"(?<path>[^'\"]*)\"");
            if (m.Success)
                return m.Groups["path"].Value.Replace(@"\\", @"\");
            return "";
        }

        public static string ParseDistance(string xml)
        {
            Match m = Regex.Match(xml, @"([\d\.]*)&#160;mi");
            if (m.Success)
                return m.Groups[1].Value;
            return "0";
        }

        //private static string GetEncodedLine(string xml)
        //{
        //    try
        //    {
        //        string ret = xml.Substring(xml.IndexOf("<points>") + 8);
        //        ret = ret.Substring(0, ret.IndexOf("</points>"));
        //        return ret.Replace(@"\\", @"\");
        //    }
        //    catch { return ""; }
        //}
        #endregion

		//public static PointOfInterestList GetLine(string name, string encodedLine)
		//{
		//    if (encodedLine.Length == 0)
		//        return null;
		//    PointOfInterestList track = new PointOfInterestList { ListName = name };
		//    track.AddRange(DecodeLocations(encodedLine));
		//    return track;
		//}

        //public static double CalculateDistance(GPoints points)
        //{
        //    GPoint first = points[0];
        //    double distance = 0;
        //    for (int i = 1; i < points.Count; i++)
        //    {
        //        distance += ComputeHaversine(first, points[i]);
        //        first = points[i];
        //    }
        //    return distance;
        //}

        //private static double ComputeHaversine(GPoint point1, GPoint point2)
        //{
        //    double conversion = 2 * Math.PI / 360;
        //    double lat = (point2.X - point1.X) * conversion;
        //    double lon = (point2.Y - point1.Y) * conversion;
        //    double a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
        //        Math.Cos(point1.X * conversion) * Math.Cos(point2.X * conversion) *
        //        Math.Sin(lon / 2) * Math.Sin(lon / 2);
        //    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        //    return 6371 * c;
        //}

        #region Encode/Decode

        public static string Encode(int val)
        {
            StringBuilder encodedStr = new StringBuilder();

            // if high bit set, then value was odd before shifting
            // otherwise it was even
            if (val < 0)
                val = 1 + (~val << 1);
            else
                val <<= 1;

            if (val == 0)
                encodedStr.Append((char)63);
            else
            {
                while (val > 0)
                {
                    char c = (char)((val & 31) + ((val < 32) ? 63 : 95));

                    encodedStr.Append(c);
                    val >>= 5;
                }
            }

            return encodedStr.ToString();
        }

		//private static IEnumerable<PointOfInterest> DecodeLocations(string encodedLatLngs)
		//{
		//    List<PointOfInterest> points = new List<PointOfInterest>();
		//    if (encodedLatLngs.Length < 2)
		//        return points;

		//    int b = encodedLatLngs.Length;
		//    int c = 0;
		//    //StringBuilder d = new StringBuilder();
		//    int e = 0;
		//    int f = 0;

		//    while (c < b)
		//    {
		//        PointOfInterest newPoint = new PointOfInterest();
		//        points.Add(newPoint);
		//        int g = 32;
		//        int h = 0;
		//        int i = 0;
		//        int t = 0;

		//        do
		//        {
		//            int ch = encodedLatLngs[c++];
		//            g = ch - 63;
		//            t = (g & 31);
		//            t = t << h;
		//            i |= t;

		//            h += 5;
		//        } while (g >= 32);

		//        int k;

		//        if ((i & 1) == 1)
		//            k = ~(i >> 1);
		//        else
		//            k = i >> 1;

		//        e += k;

		//        newPoint.Latitude = e * .00001;
		//        //d.Append(e * .00001);
		//        //d.Append(",");

		//        h = 0;
		//        i = 0;

		//        do
		//        {
		//            g = encodedLatLngs[c++] - 63;
		//            i |= (g & 31) << h;
		//            h += 5;
		//        } while (g >= 32);

		//        int m = ((i & 1) == 1) ? ~(i >> 1) : i >> 1;

		//        f += m;

		//        newPoint.Longitude = f * .00001;
		//        //d.Append(f * .00001);
		//        //if (c < b)
		//        //    d.Append(",");

		//    }
		//    return points;
		//}
        #endregion

        #region Conversions
        //public static GPoint LatLong2xy(GPoint location, int zoom)
        //{
        //    Int32 tiles = 1 << (17 - zoom);
        //    double radY = location.Y * 2 * Math.PI / 360;
        //    double projY = Math.Log(Math.Tan(radY) + 1 / Math.Cos(radY));
        //    double projY2 = Math.Tan(85.0511 * Math.PI / 360 + Math.PI / 4);
        //    return new GPoint((180 + location.X) * tiles / 360, (85.051 - projY) * tiles / 170.102);

        //    double div = 1 << zoom;

        //    double step = 720 / div;

        //    return new GPoint((180 + location.X) / step + div / 2, (180 + location.Y) / step);
        //}

        //public static GPoint xy2LatLong(GPoint location, int zoom)
        //{
        //    double div = 1 << zoom;

        //    double step = 720 / div;

        //    return new GPoint(location.X * step - 180, (location.Y - div / 2) * step - 180);
        //}
        #endregion
    }
}