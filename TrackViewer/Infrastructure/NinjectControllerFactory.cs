using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;

namespace TrackViewer.Infrastructure
{
	public class NinjectControllerFactory : DefaultControllerFactory
	{
		private IKernel kernel = new StandardKernel(new TrackStoreServices());

		protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null) return null;
			return (IController) kernel.Get(controllerType);
		}
	}
}