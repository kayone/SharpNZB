using System;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace SharpNzb
{
    static class Program
    {

        private static readonly Logger Logger = LogManager.GetLogger("Application");

        static void Main()
        {
            Logger.Info(Process.GetCurrentProcess().Id);

            try
            {
                Thread.CurrentThread.Name = "Host";

                AppDomain.CurrentDomain.UnhandledException += ((s, e) => AppDomainException(e));
                AppDomain.CurrentDomain.ProcessExit += ProgramExited;
                AppDomain.CurrentDomain.DomainUnload += ProgramExited;
                System.Diagnostics.Process.GetCurrentProcess().Exited += ProgramExited;

                Config.ConfigureNlog();

                Logger.Info("Starting SharpNzb. Start-up Path:'{0}'", Config.ProjectRoot);

                IISController.KillOrphaned();
                IISController.StartIIS();

                System.Diagnostics.Process.Start(IISController.AppUrl);

#if DEBUG
                //Manually Attach debugger to IISExpress
                if (Debugger.IsAttached)
                {
                    try
                    {
                        ProcessAttacher.Attach();
                    }
                    catch (Exception e)
                    {
                        Logger.Warn("Unable to attach to debugger", e);
                    }

                }
#endif


            }
            catch (Exception e)
            {
                AppDomainException(e);
            }

            Console.Write("Press Enter At Any Time To Exit...");
            Console.ReadLine();
            IISController.StopIIS();
        }


        private static void AppDomainException(object excepion)
        {
            Console.WriteLine("EPIC FAIL: {0}", excepion);
            Logger.Fatal("EPIC FAIL: {0}", excepion);

            //For Exceptioneer...
            //new Client
            //{
            //    ApiKey = "43BBF60A-EB2A-4C1C-B09E-422ADF637265",
            //    ApplicationName = "SharpNzb",
            //    CurrentException = excepion as Exception
            //}.Submit();

            IISController.StopIIS();
        }

        static void ProgramExited(object sender, EventArgs e)
        {
            IISController.StopIIS();
        }


    }
}

