using System;
using System.Diagnostics;
using System.IO;
using Ninject;
using NLog;
using SharpNzb.Core.Instrumentation;
using SharpNzb.Core.Providers;
using SharpNzb.Core.Repository;
using SubSonic.DataProviders;
using SubSonic.Repository;
using System.Web;

namespace SharpNzb.Core
{
    //Define Events here for ease of Implementation
    public delegate void ArticleFinishedHandler(Guid ConnectionId);

    public static class CentralDispatch
    {
        private static IKernel _kernel;
        private static readonly Object kernelLock = new object();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void BindKernel()
        {
            lock (kernelLock)
            {
                Logger.Debug("Binding Ninject's Kernel");
                _kernel = new StandardKernel();

                //SQLite
                //string connectionString = String.Format("Data Source={0};Version=3;", Path.Combine(AppPath, "SharpNzb.db"));
                //var dbProvider = ProviderFactory.GetProvider(connectionString, "System.Data.SQLite");

                //SQLExpress
                string connectionString = String.Format(@"server=.\SQLExpress; database=SharpNzb; Trusted_Connection=True;");
                var dbProvider = ProviderFactory.GetProvider(connectionString, "System.Data.SqlClient");

                //SQLite
                //string logConnectionString = String.Format("Data Source={0};Version=3;", Path.Combine(AppPath, "log.db"));
                //var logDbProvider = ProviderFactory.GetProvider(logConnectionString, "System.Data.SQLite");

                //SQLExpress
                string logConnectionString = String.Format(@"server=.\SQLExpress; database=SharpNzbLogs; Trusted_Connection=True;");
                var logDbProvider = ProviderFactory.GetProvider(logConnectionString, "System.Data.SqlClient");

                var logRepository = new SimpleRepository(logDbProvider, SimpleRepositoryOptions.RunMigrations);

                //dbProvider.ExecuteQuery(new QueryCommand("VACUUM", dbProvider)); //What does this do?
                dbProvider.Log = new NlogWriter();
                dbProvider.LogParams = true;

                _kernel.Bind<IDiskProvider>().To<DiskProvider>();
                _kernel.Bind<IConfigProvider>().To<ConfigProvider>().InSingletonScope();
                _kernel.Bind<INzbQueueProvider>().To<NzbQueueProvider>().InSingletonScope();
                _kernel.Bind<INzbImportProvider>().To<NzbImportProvider>().InSingletonScope();
                _kernel.Bind<IHistoryProvider>().To<HistoryProvider>().InSingletonScope();
                _kernel.Bind<ICategoryProvider>().To<CategoryProvider>().InSingletonScope();
                _kernel.Bind<IScriptProvider>().To<ScriptProvider>().InSingletonScope();
                _kernel.Bind<IPreQueueProvider>().To<PreQueueProvider>().InSingletonScope();
                _kernel.Bind<IServerProvider>().To<ServerProvider>().InSingletonScope();
                _kernel.Bind<INntpProvider>().To<NntpProvider>().InSingletonScope();
                _kernel.Bind<IYencProvider>().To<IYencProvider>().InSingletonScope();
                _kernel.Bind<IHttpProvider>().To<HttpProvider>();
                _kernel.Bind<IXmlProvider>().To<XmlProvider>();
                _kernel.Bind<IDecompressProvider>().To<DecompressProvider>();
                _kernel.Bind<INzbParseProvider>().To<NzbParseProvider>();
                _kernel.Bind<INotificationProvider>().To<NotificationProvider>().InSingletonScope();
                _kernel.Bind<ILogProvider>().To<LogProvider>().InSingletonScope();
                _kernel.Bind<IRepository>().ToMethod(c => new SimpleRepository(dbProvider, SimpleRepositoryOptions.RunMigrations)).InSingletonScope();
                _kernel.Bind<IRepository>().ToConstant(logRepository).WhenInjectedInto<SubsonicTarget>().InSingletonScope();
                _kernel.Bind<IRepository>().ToConstant(logRepository).WhenInjectedInto<LogProvider>().InSingletonScope();


                ForceMigration(_kernel.Get<IRepository>());
            }
        }

        public static String AppPath
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return new DirectoryInfo(HttpContext.Current.Server.MapPath("\\")).FullName;
                }
                return Directory.GetCurrentDirectory();
            }
        }

        public static IKernel NinjectKernel
        {
            get
            {
                if (_kernel == null)
                {
                    BindKernel();
                }
                return _kernel;
            }
        }

        private static void ForceMigration(IRepository repository)
        {
            repository.GetPaged<Category>(0, 1);
            repository.GetPaged<History>(0, 1);
            repository.GetPaged<Server>(0, 1);
        }

        /// <summary>
        /// This method forces IISExpress process to exit with the host application
        /// </summary>
        public static void DedicateToHost()
        {
            try
            {
                Logger.Debug("Attaching to parent process for automatic termination.");
                var pc = new PerformanceCounter("Process", "Creating Process ID", Process.GetCurrentProcess().ProcessName);
                var pid = (int)pc.NextValue();
                var hostProcess = Process.GetProcessById(pid);

                hostProcess.EnableRaisingEvents = true;
                hostProcess.Exited += (delegate
                {
                    Logger.Info("Host has been terminated. Shutting down web server.");
                    ShutDown();
                });

                Logger.Debug("Successfully Attached to host. Process ID: {0}", pid);
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
            }
        }

        private static void ShutDown()
        {
            Logger.Info("Shutting down application.");
            Process.GetCurrentProcess().Kill();
        }
    }
}
