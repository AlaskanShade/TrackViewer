using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackViewer.Controllers;
using System.Web.Mvc;
using TrackViewer.Domain.Concrete;

namespace TrackViewer.Tests.Controllers
{
	[TestClass]
	public class TrackControllerTests
	{
		[TestMethod]
		public void TestUploadFile()
		{
			// Arrange
			TrackController controller = new TrackController(new TracksRepository(@"Server=localhost\sql10;Database=TrackStore;Trusted_Connection=yes;"));

			// Act
			//foreach (string file in System.IO.Directory.GetFiles("", "201_____.gpx"))
			//	controller.Upload(new System.Web.HttpPostedFileBase());
			ViewResult result = controller.Upload() as ViewResult;

			// Assert
			//ViewDataDictionary viewData = result.ViewData;
			//Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
		}
	}
}
