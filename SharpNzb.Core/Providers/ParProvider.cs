using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SharpNzb.Core.Providers
{
    public class ParProvider : IParProvider
    {
        //
        #region IParProvider Members

        public bool Verify(string fileName)
        {
            //string arguments = String.Format("v \"{0}\"", nzb.DownloadDirectory + Path.DirectorySeparatorChar + nzb.Par2File);
            string arguments = "";

            throw new NotImplementedException("Par2 - Verify");

            Process process = new Process();
            process.StartInfo.UseShellExecute = false; //Must be false to redirect the output
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Environment.CurrentDirectory + @"\utils\phpar2.exe";
            process.StartInfo.Arguments = arguments;
            process.OutputDataReceived += new DataReceivedEventHandler(VerifyDataReceived); //Event for when new data is written
            process.Start(); //Start the Process
            process.BeginOutputReadLine(); //Read the Output Async
            process.WaitForExit(); //Wait for the Process to Exit

            throw new NotImplementedException("Par2 Verify");
        }

        public bool Repair(string fileName)
        {
            
            throw new NotImplementedException("Par2 Repair");
        }

        #endregion

        private int _processedCount;
        private bool _repairNeeded;
        private bool _repairIsPossible;
        private int _blocksNeeded;

        private void VerifyDataReceived(object sender, DataReceivedEventArgs args)
        {
            //Process the Output - Look for Repair Needed etc
            Console.WriteLine(args.Data);
            //_output.WriteLine(args.Data); //Append this line to _output - Might not need this...

            string line = args.Data;

            if (String.IsNullOrEmpty(line)) //If line is Null or Empty Return so an Exception isn't thrown + not of any use to us
                return;

            if (line.StartsWith("Target: "))
            {
                _processedCount++; //Add to the Number of Files Processed (whether missing or available)
                return; //No need to process anymore for this line
            }

            if (line.StartsWith("All files are correct"))
            {
                //Winnnnn
                return;
            }

            if (line.StartsWith("Repair is required"))
            {
                _repairNeeded = true; //Need to wait for the proper line to be processed
                return; //No need to process anymore for this line
            }

            if (_repairNeeded)
            {
                if (line.StartsWith("Repair is not possible."))
                {
                    _repairIsPossible = false;
                    return; //No need to process anymore for this line
                }

                if (!_repairIsPossible)
                {
                    //How many blocks need to be downloaded?
                    string patten = @"You need (?<Blocks>\d+) more recovery blocks to be able to repair.";
                    Match match = Regex.Match(line, patten);

                    if (match.Success)
                    {
                        _blocksNeeded = Int32.Parse(match.Groups["Blocks"].Value);
                        return;
                    }
                }
            }
        }
    }
}
