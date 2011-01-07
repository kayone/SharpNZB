using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NLog;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class ScriptProvider : IScriptProvider
    {
        private readonly IConfigProvider _config;
        private readonly IDiskProvider _disk;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ScriptProvider(IConfigProvider config, IDiskProvider disk)
        {
            _config = config;
            _disk = disk;
        }

        #region IScriptProvider Members

        public List<string> AllScripts()
        {
            //Return All Files located in Script Dir (Found in Config)
            return _disk.GetFiles(_config.GetValue("ScriptDir", "scripts", true), "", SearchOption.TopDirectoryOnly).ToList();
        }

        public bool Run(string name, NzbModel nzb)
        {
            //result should be output from the script...
            var scriptDir = Path.GetFullPath(_config.GetValue("ScriptDir", AllScripts(), true));

            var scriptPath = Path.GetFullPath(scriptDir + Path.DirectorySeparatorChar + name);

            if (!_disk.FileExists(scriptPath))
            {
                Logger.Error("Script does not exist.");
                return false;
            }

            //1 	The final directory of the job (full path)
            //2 	The original name of the NZB file
            //3 	Clean version of the job name (no path info and ".nzb" removed)
            //4 	Indexer's report number (if supported) - Don't have this information right now....
            //5 	User-defined category
            //6 	Group that the NZB was posted in e.g. alt.binaries.x
            //7 	Status of post processing. 0 = OK, 1=failed verification, 2=failed unpack, 3=1+21

            var arguments = String.Format("{0} {1} {2} {3} {4} {5} {6}", GetStringForArgument(nzb.FinalDirectory), GetStringForArgument(nzb.Name), GetStringForArgument(nzb.Name), String.Empty, nzb.Category, nzb.Files[0].Groups[0], (int)nzb.PostProcessingStatus);

            var process = new Process();
            process.StartInfo.UseShellExecute = false; //Must be false to redirect the output
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = scriptPath;
            process.StartInfo.Arguments = arguments;
            process.Start(); //Start the Process
            process.WaitForExit(); //Wait for the Process to Exit

            nzb.ScriptOutput = process.StandardOutput.ReadToEnd(); //Save the output of the script to ScriptOutput (For later use in History)

            if (process.ExitCode != 0)
                return false;

            return true;
        }

        #endregion

        private string GetStringForArgument(string field)
        {
            if (field.Contains(" "))
                return String.Format("\"{0}\"", field);

            return field;
        }
    }
}
