using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Moq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TrackViewer.Tests
{
	public static class TestHelper
	{
		public static HttpContextBase FakeHttpContext()
		{
			var httpContext = new Mock<HttpContextBase>();
			var request = new Mock<HttpRequestBase>();
			var response = new Mock<HttpResponseBase>();
			var session = new Mock<HttpSessionStateBase>();
			var server = new Mock<HttpServerUtilityBase>();
			var user = new Mock<IPrincipal>();
			var identity = new Mock<IIdentity>();

			request.Setup(req => req.ApplicationPath).Returns("~/");
			request.Setup(req => req.AppRelativeCurrentExecutionFilePath).Returns("~/");
			request.Setup(req => req.PathInfo).Returns(string.Empty);
			response.Setup(res => res.ApplyAppPathModifier(It.IsAny<string>())).Returns((string virtualPath) => virtualPath);
			user.Setup(usr => usr.Identity).Returns(identity.Object);
			identity.SetupGet(id => id.IsAuthenticated).Returns(true);

			httpContext.Setup(ctx => ctx.Request).Returns(request.Object);
			httpContext.SetupGet(ctx => ctx.Request.RequestType).Returns("POST");
			httpContext.Setup(ctx => ctx.Response).Returns(response.Object);
			httpContext.Setup(ctx => ctx.Session).Returns(session.Object);
			httpContext.Setup(ctx => ctx.Server).Returns(server.Object);
			httpContext.Setup(ctx => ctx.User).Returns(user.Object);

			return httpContext.Object;
		}

		public static HttpContextBase FakeHttpContext(string url)
		{
			var context = FakeHttpContext();
			context.Request.SetupRequestUrl(url);
			return context;
		}
		public static void SetFakeControllerContext(this Controller controller)
		{
			var httpContext = FakeHttpContext();
			ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
			controller.ControllerContext = context;
		}

		static string GetUrlFileName(string url)
		{
			if (url.Contains("?"))
				return url.Substring(0, url.IndexOf("?"));
			else
				return url;
		}

		static NameValueCollection GetQueryStringParameters(string url)
		{
			if (url.Contains("?"))
			{
				NameValueCollection parameters = new NameValueCollection();

				string[] parts = url.Split("?".ToCharArray());
				string[] keys = parts[1].Split("&".ToCharArray());

				foreach (string key in keys)
				{
					string[] part = key.Split("=".ToCharArray());
					parameters.Add(part[0], part[1]);
				}

				return parameters;
			}
			else
			{
				return null;
			}
		}

		public static void SetHttpMethodResult(this HttpRequestBase request, string httpMethod)
		{
			Mock.Get(request)
				.Setup(req => req.HttpMethod)
				.Returns(httpMethod);
		}

		public static void SetupRequestUrl(this HttpRequestBase request, string url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			if (!url.StartsWith("~/"))
				throw new ArgumentException("Sorry, we expect a virtual url starting with \"~/\".");

			var mock = Mock.Get(request);

			mock.Setup(req => req.QueryString)
				.Returns(GetQueryStringParameters(url));
			mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
				.Returns(GetUrlFileName(url));
			mock.Setup(req => req.PathInfo)
				.Returns(string.Empty);
		}

		public static void TestRouteData(RouteData data, string controller, string action)
		{
			Assert.IsNotNull(data);
			Assert.AreEqual(controller, data.Values["controller"]);
			Assert.AreEqual(action, data.Values["action"]);
		}
	}
}
