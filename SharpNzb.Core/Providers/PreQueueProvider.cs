using System;
using System.Diagnostics;
using System.IO;
using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public class PreQueueProvider : IPreQueueProvider
    {
        private IConfigProvider _config;
        private IDiskProvider _disk;

        public PreQueueProvider(IConfigProvider config, IDiskProvider disk)
        {
            _config = config;
            _disk = disk;
        }

        #region IPreQueueProvider Members

        public bool Run (NzbModel nzb)
        {
            //Run The Pre-Queue Script

            var script = _config.GetValue("PreQueueScript", String.Empty, false);
            var scriptDir = _config.GetValue("ScriptDir", "scripts", true);

            //CHeck if the script dir contains / or \ if not it is a relevant path

            var scriptPath = scriptDir + Path.DirectorySeparatorChar + script;

            //If the script isn't on disk, then return true (Better to Accept than reject)
            if (!_disk.FileExists(scriptPath))
                return true;

            //1 : Name of the NZB (no path, no ".nzb")
            //2 : PP (0, 1, 2 or 3)
            //3 : Category
            //4 : Script (no path)
            //5 : Priority (-100, -1, 0 or 1 meaning Default, Low, Normal, High)
            //6 : Size of the download (in bytes)
            //7 : Group list (separated by spaces)
            //8 : Show name
            //9 : Season (1..99)
            //10 : Episode (1..99)
            //11: Episode name

            string groups = String.Join(" ", nzb.Files[0].Groups.ToArray());

            var arguments = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}", GetStringForArgument(nzb.Name), nzb.PostProcessing, nzb.Category, nzb.Script,nzb.Priority, nzb.Size,
                groups, nzb.ShowName, nzb.SeasonNumber, nzb.EpisodeNumber, nzb.EpisodeName);

            var process = new Process();
            process.StartInfo.UseShellExecute = false; //Must be false to redirect the output
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = scriptPath;
            process.StartInfo.Arguments = arguments;
            process.Start(); //Start the Process
            process.WaitForExit(); //Wait for the Process to Exit

            int lineNumber = 0;
            string line;

            while ((line = process.StandardOutput.ReadLine()) != null)
            {
                if (lineNumber == 0)
                {
                    if (!Convert.ToBoolean(line)) return false; //If set to Refuse then return false
                }

                if (lineNumber == 1)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        nzb.Name = line;
                    }
                }

                if (lineNumber == 2)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        nzb.PostProcessing = (PostProcessing)Convert.ToInt32(line);
                    }
                }

                if (lineNumber == 3)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        nzb.Category = line;
                    }
                }

                if (lineNumber == 4)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        nzb.Script = line;
                    }
                }

                if (lineNumber == 5)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        nzb.Priority = (Priority)Convert.ToInt32(line);
                    }
                }

                if (lineNumber == 6)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        //Set each File for this NZB to have the group set to line
                        foreach (var file in nzb.Files)
                        {
                            file.Groups.Clear();
                            file.Groups.Add(line);
                        }
                    }
                }

                if (lineNumber > 6)
                    break;

                lineNumber++;
            }
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
