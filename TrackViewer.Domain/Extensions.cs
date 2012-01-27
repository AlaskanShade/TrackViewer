using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace TrackViewer.Domain
{
	public static class Extensions
	{
		public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
		{
			var values = from TEnum e in Enum.GetValues(typeof(TEnum))
						 select new { ID = e, Name = e.ToString() };
			return new SelectList(values, "Id", "Name", enumObj);
		}
	}
}
