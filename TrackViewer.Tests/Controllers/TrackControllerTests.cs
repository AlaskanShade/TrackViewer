using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrackViewer.Controllers;
using System.Web.Mvc;
using TrackViewer.Domain.Concrete;
using System.Web.Routing;
using Moq;
using TrackViewer.Domain.Abstract;
using TrackViewer.Domain.Entities;
using System.Collections.Specialized;

namespace TrackViewer.Tests.Controllers
{
	[TestClass]
	public class TrackControllerTests
	{
		private TrackController GetController()
		{
			var mockRepository = new Mock<ITracksRepository>();
			List<Track> tracks = new List<Track>();
			tracks.Add(new Track { TrackID = 1, Name = "Test", TypeOfTravel = TravelType.Car });
			mockRepository.Setup(r => r.Tracks).Returns(tracks.AsQueryable());
			mockRepository.Setup(r => r.AddTrack(It.IsAny<Track>())).Callback((Track t) => tracks.Add(t));
			mockRepository.Setup(r => r.DeleteTrack(It.IsAny<int>())).Callback((int i) => tracks.RemoveAll(t => t.TrackID == i));
			return new TrackController(mockRepository.Object);
			//return new TrackController(new TracksRepository(@"Server=localhost;Database=TrackStore;Trusted_Connection=yes;"));
		}

		[TestMethod]
		public void TestUploadFile()
		{
			// Arrange
			var controller = GetController();
			//controller.SetFakeControllerContext();

			// Act
			//foreach (string file in System.IO.Directory.GetFiles("", "201_____.gpx"))
			//	controller.Upload(new System.Web.HttpPostedFileBase());
			var result = controller.List() as ViewResult;

			// Assert
			var model = result.ViewData.Model as IList<Track>;
			Assert.IsNotNull(model, "Model should have been type of IList<Track>");
		}

		[TestMethod]
		public void TestHome()
		{
			//Arrange
			var controller = GetController();
			controller.SetFakeControllerContext();

			//Setup postback values
			var request = Mock.Get(controller.Request);
			request.Setup(r => r.Form).Returns(() => 
			{
				var nv = new NameValueCollection();
				nv.Add("Name", "value");
				return nv;
			});

			//Setup session value
			var sessionStub = Mock.Get(controller.HttpContext.Session);
			sessionStub.Setup(s => s["value"]).Returns("some value");
		}

		[TestMethod]
		public void TestDefaultAction()
		{
			var context = TestHelper.FakeHttpContext("~/");
			var routes = new RouteCollection();
			MvcApplication.RegisterRoutes(routes);

			var routeData = routes.GetRouteData(context);

			TestHelper.TestRouteData(routeData, "Track", "List");
		}
	}
}
