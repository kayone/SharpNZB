using System;
using System.Xml.Linq;

namespace SharpNzb.Core.Providers
{
    public class XmlProvider : IXmlProvider
    {
        private readonly INzbQueueProvider _nzbQueue;
        private readonly IHistoryProvider _history;
        private readonly ICategoryProvider _category;
        private readonly IScriptProvider _script;

        public XmlProvider(INzbQueueProvider nzbQueue, IHistoryProvider history, ICategoryProvider category, IScriptProvider script)
        {
            _nzbQueue = nzbQueue;
            _history = history;
            _category = category;
            _script = script;
        }

        #region IXmlProvider Members

        public XElement Categories()
        {
            //XElement root = new XElement("root");
            //root.Add(new XElement("element1"));
            //root.Add(new XElement("element2"));
            //root.Add(new XAttribute("attribute1", "a value"));

            XElement categories = new XElement("categories");

            foreach (var cat in _category.AllItems())
            {
                var category = new XElement("category");
                category.Value = cat.Name;
                categories.Add(category);
            }

            return categories;
        }

        public XElement History(int start, int count)
        {
            throw new NotImplementedException("XmlProvider - History");
        }

        public XElement Queue(int start, int count)
        {
            XElement queue = new XElement("queue");
            queue.Add(new XElement("active_lang"));
            queue.Element("active_lang").Value = "us-en";
            queue.Add(new XElement("session"));
            queue.Element("session").Value = "MYAPIKEYGOESHERE";
            queue.Add(new XElement("slots"));

            foreach (var item in _nzbQueue.Range(start, count))
            {
                var slot = new XElement("slot");
                slot.Add(new XElement("status"));
                slot.Element("status").Value = item.Status.ToString();
                slot.Add(new XElement("index"));
                slot.Element("index").Value = _nzbQueue.Index(item.Id).ToString();
                slot.Add(new XElement("eta"));
                slot.Add(new XElement("timeleft"));
                slot.Add(new XElement("avg_age"));
                slot.Add(new XElement("script"));
                slot.Add(new XElement("msgid"));
                slot.Add(new XElement("verbosity"));
                slot.Add(new XElement("mb"));
                slot.Add(new XElement("sizeleft"));
                slot.Add(new XElement("filename"));
                slot.Element("filename").Value = item.Name;
                slot.Add(new XElement("priority"));
                slot.Element("priority").Value = item.Priority.ToString();
                slot.Add(new XElement("cat"));
                slot.Element("cat").Value = item.Category;
                slot.Add(new XElement("mbleft"));
                slot.Add(new XElement("percentage"));
                slot.Add(new XElement("nzo_id"));
                slot.Add(new XElement("unpackopts"));
                slot.Add(new XElement("size"));

                queue.Element("slots").Add(slot);
            }

            queue.Add(new XElement("speed"));
            queue.Add(new XElement("size"));
            queue.Add(new XElement("limit"));
            queue.Add(new XElement("start"));
            queue.Add(new XElement("diskspacetotal2"));
            queue.Add(new XElement("darwin"));
            queue.Add(new XElement("last_warning"));
            queue.Add(new XElement("have_warning"));
            queue.Add(new XElement("noofslots"));
            queue.Add(new XElement("pause_int"));

            queue.Add(new XElement("categories"));

            foreach (var item in _category.AllItems())
            {
                var cat = new XElement("category");
                cat.Value = item.Name;
                queue.Element("categories").Add(cat);
            }

            queue.Add(new XElement("diskspacetotal1"));
            queue.Add(new XElement("mb"));
            queue.Add(new XElement("loadavg"));
            queue.Add(new XElement("cache_max"));
            queue.Add(new XElement("speedlimit"));
            queue.Add(new XElement("webdir"));
            queue.Add(new XElement("paused"));
            queue.Add(new XElement("isverbose"));
            queue.Add(new XElement("restart_req"));
            queue.Add(new XElement("power_options"));
            queue.Add(new XElement("helpuri"));
            queue.Add(new XElement("uptime"));
            queue.Add(new XElement("refreshrate"));
            queue.Add(new XElement("version"));
            queue.Add(new XElement("color_scheme"));
            queue.Add(new XElement("new_release"));
            queue.Add(new XElement("nt"));
            queue.Add(new XElement("status"));
            queue.Add(new XElement("finish"));
            queue.Add(new XElement("cache_art"));
            queue.Add(new XElement("pause_all"));
            queue.Add(new XElement("finishaction"));
            queue.Add(new XElement("sizeleft"));
            queue.Add(new XElement("cache_size"));
            queue.Add(new XElement("mbleft"));
            queue.Add(new XElement("diskspace2"));
            queue.Add(new XElement("diskspace1"));

            //scripts
            queue.Add(new XElement("scripts"));

            foreach (var item in _script.AllScripts())
            {
                var script = new XElement("script");
                script.Value = item;
                queue.Element("scripts").Add(script);
            }

            queue.Add(new XElement("timeleft"));
            queue.Add(new XElement("nzb_quota"));
            queue.Add(new XElement("eta"));
            queue.Add(new XElement("kbpersec"));
            queue.Add(new XElement("new_rel_url"));
            queue.Add(new XElement("queue_details"));

            return queue;
        }

        public XElement Scripts()
        {
            XElement scripts = new XElement("scripts");

            foreach (var s in _script.AllScripts())
            {
                var script = new XElement("category");
                script.Value = s;
                scripts.Add(script);
            }

            return scripts;
        }

        public XElement Version()
        {
            XElement version = new XElement("version");
            version.Value = "1.2.3.4";
            return version;
        }

        #endregion
    }
}
