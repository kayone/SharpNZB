using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using NLog;
using SharpNzb.Core;
using SharpNzb.Core.Instrumentation;
using SharpNzb.Web.Ninject.Web.Mvc;

namespace SharpNzb.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Sabnzbd", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        protected override void OnApplicationStarted()
        {
            LogConfiguration.Setup();
            Logger.Info("SAB# Starting up.");
            CentralDispatch.DedicateToHost();
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            base.OnApplicationStarted();
        }

        protected override IKernel CreateKernel()
        {
            var kernel = CentralDispatch.NinjectKernel;
            // kernel.Bind<IRepository>().ToConstant(kernel.Get<IRepository>("LogDb"));

            return kernel;
        }

        // ReSharper disable InconsistentNaming
        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            if (lastError is HttpException)
            {
                Logger.WarnException(lastError.Message, lastError);
            }
            else
            {
                Logger.FatalException(lastError.Message, lastError);
            }
        }

        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.Name = "UI";
        }
    }
}