using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using MvcContrib.ActionResults;
using SharpNzb.Core.Model.Nzb;
using SharpNzb.Core.Providers;

namespace SharpNzb.Web.Controllers
{
    public class ApiController : Controller
    {
        //
        // GET: /Api/

        private INzbImportProvider _import;
        private IXmlProvider _xmlProvider;

        public ApiController(INzbImportProvider import, IXmlProvider xmlProvider)
        {
            _import = import;
            _xmlProvider = xmlProvider;
        }

        public ActionResult Index()
        {
            //Get the mode
            string mode = GetMode(Request.QueryString);

            if (mode == "queue")
            {
                string output = GetOutputType(Request.QueryString);
                int start = GetOutputStart(Request.QueryString);
                int limit = GetOutputLimit(Request.QueryString);

                if (!String.IsNullOrEmpty(output)) //Check that this is a request for Queue Output
                {
                    if (output == "xml")
                    {
                        return new XmlResult(_xmlProvider.Queue(start, limit));
                    }

                    else //output type is unknown
                    {
                        //return error
                    }
                }

                else
                {
                    string name = GetName(Request.QueryString);

                    if (name == "change_complete_action")
                    {
                        string value = GetValue(Request.QueryString);

                        //Need list of built-in actions (Shutdown SABnzbd, Shutdown PC, Suspend PC, Hibernate PC)

                        if (value.StartsWith("script_"))
                        {
                            //Run script after downloads finish
                            string scriptName = value.Replace("script_", "");
                        }
                        //Determine if request is a quipt change or if a built in action should be performed
                    }

                    else if (name == "priority")
                    {
                        //Low = -1, Normal = 0, High = 1
                        string value = GetValue(Request.QueryString);
                        int priority = 0;
                        Int32.TryParse(GetValue2(Request.QueryString), out priority);

                        //Change the Priority for the specified NZB
                    }

                    else if (name == "pause")
                    {
                        string value = GetValue(Request.QueryString);

                        //Pause the specified NZB
                    }

                    else if (name == "resume")
                    {
                        string value = GetValue(Request.QueryString);
                        //Resume the specified NZB
                    }

                    else if (name == "rename")
                    {
                        string value = GetValue(Request.QueryString);
                        string newName = GetValue2(Request.QueryString);
                        //Rename the specified NZB
                    }

                    else if (name == "delete")
                    {
                        string value = GetValue(Request.QueryString);
                        //Delete the specified NZB

                        //return  ok\n (Returns ok even if none are deleted)
                    }
                    //Look for name in request to determine which Queue request is wanted
                    //&name=change_complete_action&value= (change action to perform when Queue is emptied, prepend script_ to call a script)
                    //&name=priority&value=GUID&value2=PRIORITY (Change the Priority of a NZB)
                    //&name=pause&value=GUID (Pause a specific NZB)
                    //&name=resume&value=GUID (Resume a specific NZB)
                    //&name=rename&value=GUID&value2=newName (Rename a NZB)
                    //&name=delete&value=GUID (Single GUID to delete from history)
                    //&name=delete&value=GUID,GUID (Comma Separated list of GUIDs)
                    //&name=delete&value=all (Purges the History)
                }

                //Request is Inavlid (Not yet implemented)
                throw new NotImplementedException("Queue Changes");
            }

            else if (mode == "history")
            {
                ProcessHistoryRequest(Request.QueryString);
            }

            else if (mode == "version")
            {
                string output = GetOutputType(Request.QueryString);

                if (!String.IsNullOrEmpty(output)) //Check that this is a request for Queue Output
                {
                    if (output == "xml")
                    {
                        //return as XML
                    }

                    else //output type is unknown
                    {
                        //return as plain text
                    }
                }
                throw new NotImplementedException("Get Version");
            }

            else if (mode == "warnings")
            {
                string output = GetOutputType(Request.QueryString);

                if (!String.IsNullOrEmpty(output)) //Check that this is a request for Queue Output
                {
                    if (output == "xml")
                    {
                        //return as XML
                    }

                    else //output type is unknown
                    {
                        //return as plain text? (Test SAB)
                    }
                }
                throw new NotImplementedException("Get Warnings");
            }

            else if (mode == "get_cats")
            {
                string output = GetOutputType(Request.QueryString);

                if (!String.IsNullOrEmpty(output)) //Check that this is a request for Queue Output
                {
                    if (output == "xml")
                    {
                        //Get the Cats and return XML
                    }

                    if (output == "json")
                    {
                        //Get the cats and return JSon
                    }

                    else
                    {
                        //return as plain text
                    }
                }

                throw new NotImplementedException("Get Cats");
            }

            else if (mode == "get_scripts")
            {
                string output = GetOutputType(Request.QueryString);

                if (!String.IsNullOrEmpty(output)) //Check that this is a request for Queue Output
                {
                    if (output == "xml")
                    {
                        //return as XML
                    }

                    else //output type is unknown
                    {
                        //return as plain text
                    }
                }
                throw new NotImplementedException("Get Scripts");
            }

            else if (mode == "pause")
            {
                //Pause the Entire Queue
                string output = GetOutputType(Request.QueryString);
                //return "ok\n" + Pause Downloading
                throw new NotImplementedException("Pause Queue");
            }

            else if (mode == "resume")
            {
                //Resume the Entire Queue
                string output = GetOutputType(Request.QueryString);
                //return "ok\n" + Resume Downloading
                throw new NotImplementedException("Resume Queue");
            }

            else if (mode == "restart")
            {
                //Find a way to restart the application gracefully
                throw new NotImplementedException("Restart");
            }

            else if (mode == "shutdown")
            {
                throw new NotImplementedException("Shutdown");
            }

            else if (mode == "config")
            {
                string name = GetName(Request.QueryString);

                if (name == "speedlimit")
                {
                    //Set speed limit
                    //return "ok\n"
                    //Enforce Speed Limit
                }

                else if (name == "set_apikey")
                {
                    //Set API Key
                    string newApiKey = Guid.NewGuid().ToString();
                    //return newApiKey + "\n"
                }

                else if (name == "set_pause")
                {
                    int value = 0;
                    Int32.TryParse(GetValue(Request.QueryString), out value);

                    //return "ok\n"
                    //Set pause for specified nzb
                }

                //Config Request...
                //&name=speedlimit&value=# (In KB/sec)
                //&name=set_apikey (returns new apikey as text)
                //&name=set_pause&value=# (Time in Minutes)
                throw new NotImplementedException("Config Request");
            }

            else if (mode == "switch")
            {
                //Switch two items or just move one
                //&value=GUID&value2=GUID2
                //&value=GUID&value2=POSITION (0 Based)

                string value = GetValue(Request.QueryString);
                string value2 = GetValue2(Request.QueryString);
                int newPosition = -1;
                Int32.TryParse(value2, out newPosition);

                if (newPosition > -1) //If position2 is greater than -1 (initial value) then single NZB should be moved to position2
                {
                    //move single NZB
                }

                else //Need to swap two NZBs
                {
                    //Swap position of two NZBs
                }
                throw new NotImplementedException("Switch");
            }

            else if (mode == "change_opts")
            {
                //Change post processing for specified NZB
                //&value=GUID&value2=0-4
                //Skip: 0
                //+Repair: 1
                //+Repair/Unpack: 2
                //+Repair/Unpack/Delete: 3

                string value = GetValue(Request.QueryString);
                int value2 = 10; //Set to Default Post-Processing Value
                Int32.TryParse(GetValue2(Request.QueryString), out value2);

                throw new NotImplementedException("Change Post Processing Opts");
            }

            else if (mode == "change_script")
            {
                //&value=GUID&value2=SCRIPTNAME
                string value = GetValue(Request.QueryString);
                string value2 = GetValue(Request.QueryString);

                //return scripts
                throw new NotImplementedException("Change Script");
            }

            else if (mode == "change_cat")
            {
                //&value=GUID&value2=CATEGORY
                string value = GetValue(Request.QueryString);
                string value2 = GetValue(Request.QueryString);

                //return Change Category
                throw new NotImplementedException("Change Category");
            }

            else if (mode == "get_files")
            {
                //Get Contents of a Queue Item
            }

            else if (mode == "addurl")
            {
                if (ProcessAddUrl(Request.QueryString))
                    return Content("ok\n");
            }

            else if (mode == "addlocalfile")
            {
                if (ProcessAddLocalFile(Request.QueryString))
                    return Content("ok\n");
            }

            //return View(list);
            XElement root = new XElement("root");
            root.Add(new XElement("element1"));
            root.Add(new XElement("element2"));
            root.Add(new XAttribute("attribute1", "a value"));
            
            return new XmlResult(root);
        }

        private string GetMode(NameValueCollection queryStrings)
        {
            if (queryStrings.AllKeys.Contains("mode", StringComparer.InvariantCultureIgnoreCase)) //Look for "mode" in queryStrings, ignoring case
                return queryStrings.GetValues("mode")[0].ToLower(); //return the value in lower-case

            return null;
        }

        private string GetName(NameValueCollection queryStrrings)
        {
            if (queryStrrings.AllKeys.Contains("name", StringComparer.InvariantCultureIgnoreCase)) //Look for "name" in queryStrrings, ignoring case
                return queryStrrings.GetValues("name")[0].ToLower(); //return the value in lower-case

            return null;
        }

        private string GetValue(NameValueCollection queryStrrings)
        {
            if (queryStrrings.AllKeys.Contains("value", StringComparer.InvariantCultureIgnoreCase)) //Look for "value" in queryStrrings, ignoring case
                return queryStrrings.GetValues("value")[0].ToLower(); //return the value in lower-case

            return null;
        }

        private string GetValue2(NameValueCollection queryStrrings)
        {
            if (queryStrrings.AllKeys.Contains("value2", StringComparer.InvariantCultureIgnoreCase)) //Look for "value2" in queryStrrings, ignoring case
                return queryStrrings.GetValues("value2")[0].ToLower(); //return the value in lower-case

            return null;
        }

        private string GetOutputType(NameValueCollection queryStrrings)
        {
            if (queryStrrings.AllKeys.Contains("output", StringComparer.InvariantCultureIgnoreCase)) //Look for "output" in queryStrrings, ignoring case
                return queryStrrings.GetValues("output")[0].ToLower(); //return the value in lower-case

            return null;
        }

        private int GetOutputStart(NameValueCollection queryStrrings)
        {
            int start = 0;
            if (queryStrrings.AllKeys.Contains("start", StringComparer.InvariantCultureIgnoreCase)) //Look for "start" in queryStrrings, ignoring case
                Int32.TryParse(queryStrrings.GetValues("start")[0].ToLower(), out start); //return the value in lower-case

            return start;
        }

        private int GetOutputLimit(NameValueCollection queryStrrings)
        {
            int limit = 0;
            if (queryStrrings.AllKeys.Contains("limit", StringComparer.InvariantCultureIgnoreCase)) //Look for "limit" in queryStrrings, ignoring case
                Int32.TryParse(queryStrrings.GetValues("limit")[0].ToLower(), out limit); //return the value in lower-case

            return limit;
        }

        private void GetAddNzbDetails(NameValueCollection queryStrrings, NzbImportModel nzb)
        {
            if (queryStrrings.AllKeys.Contains("name"))
                nzb.Location = queryStrrings.GetValues("name")[0];

            //If Post Processing is defined, use it, otherwise set to -100 (Default)
            if (queryStrrings.AllKeys.Contains("pp"))
                nzb.PostProcessing = Convert.ToInt32(queryStrrings.GetValues("pp")[0]);
            else
                nzb.PostProcessing = -100;

            if (queryStrrings.AllKeys.Contains("script"))
                nzb.Script = queryStrrings.GetValues("script")[0];

            if (queryStrrings.AllKeys.Contains("cat"))
                nzb.Category = queryStrrings.GetValues("cat")[0];

            //If Priority is defined, use it, otherwise set to -100 (Default)
            if (queryStrrings.AllKeys.Contains("priority"))
                nzb.Priority = Convert.ToInt32(queryStrrings.GetValues("priority")[0]);
            else
                nzb.Priority = -100;

            if (queryStrrings.AllKeys.Contains("nzbname"))
                nzb.NewName = queryStrrings.GetValues("nzbname")[0];

            return;
        }

        private void ProcessHistoryRequest(NameValueCollection queryStrrings)
        {
            string output = GetOutputType(Request.QueryString);
            int start = GetOutputStart(Request.QueryString); ;
            int limit = GetOutputLimit(Request.QueryString); ;

            if (output != "") //Check that this is a request for Queue Output
            {
                if (output == "xml")
                {
                    //return XML
                }

                else //output type is unknown
                {
                    //return error
                }
            }

            else
            {
                string name = GetName(queryStrrings);

                if (name == "delete")
                {
                    //Look for name in request to determine which History request is wanted
                    //&name=delete&value=GUID (Single GUID to delete from history)
                    //&name=delete&value=GUID,GUID (Comma Separated list of GUIDs)
                    //&name=delete&value=all (Purges the History)

                    string value = GetValue(Request.QueryString);

                    //return "ok\n"
                }
            }

            //Request is Inavlid (Not yet implemented)
            //return error
            throw new NotImplementedException("History Changes");
        }

        private bool ProcessAddUrl(NameValueCollection queryStrrings)
        {
            NzbImportModel nzb = new NzbImportModel();
            nzb.ImportType = ImportType.Url;

            GetAddNzbDetails(queryStrrings, nzb); //Get the details for the NZB (only using URL right now)

            //Todo: Return "ok\n" to user
            _import.BeginImport(nzb);
            return true;
        }

        private bool ProcessAddLocalFile(NameValueCollection queryStrrings)
        {
            NzbImportModel nzb = new NzbImportModel();
            nzb.ImportType = ImportType.Disk;

            GetAddNzbDetails(queryStrrings, nzb); //Get the details for the NZB (only using URL right now)

            //Import the NZB and Add to Queue

            _import.BeginImport(nzb);
            return true;
        }
    }
}
