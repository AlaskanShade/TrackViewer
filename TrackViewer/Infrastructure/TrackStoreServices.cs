using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackViewer.Domain.Abstract;
using Ninject.Modules;
using TrackViewer.Domain.Concrete;
using System.Web.Configuration;

namespace TrackViewer.Infrastructure
{
	public class TrackStoreServices : NinjectModule
	{
		public override void Load()
		{
			// SQL
			Bind<ITracksRepository>().To<TracksRepository>().WithConstructorArgument("connectionString", WebConfigurationManager.ConnectionStrings["TrackStoreEntities"].ConnectionString);
			// Files
			//Bind<ITracksRepository>().To<TracksFileRepository>().WithConstructorArgument("filePath", HttpContext.Current.Server.MapPath("~/App_Data"));
		}
	}
}