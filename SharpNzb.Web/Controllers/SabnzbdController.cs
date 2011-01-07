using System.Linq;
using System.Web.Mvc;
using SharpNzb.Core.Providers;
using SharpNzb.Web.Models;
using Telerik.Web.Mvc;

namespace SharpNzb.Web.Controllers
{
    [HandleError]
    public class SabnzbdController : Controller
    {
        private readonly INzbQueueProvider _nzbQueue;
        private readonly IHistoryProvider _history;

        public SabnzbdController(INzbQueueProvider nzbQueue, IHistoryProvider history)
        {
            _nzbQueue = nzbQueue;
            _history = history;

            //nzbQueue.Add(new NzbModel
            //                    {
            //                        Category = "tv",
            //                        DatePosted = DateTime.Now,
            //                        Id = Guid.NewGuid(),
            //                        Name = "Ubuntu Test",
            //                        Status = NzbStatus.Queued,
            //                        Size = 10000000
            //                    });
        }

        public ActionResult Index()
        {
            var queue = _nzbQueue.AllItems();
            var history = _history.AllItems();
            var model = new SabnzbdModels();
            model.Queue = queue;
            model.History = history;

            return View(model);
        }

        public ActionResult Purge()
        {
            _nzbQueue.Purge();
            return RedirectToAction("Index");

        }

        public ActionResult Api()
        {
            return RedirectToAction("Index", "Api", Request.QueryString);
        }

        [GridAction]
        public ActionResult _AjaxBinding()
        {
            return View(new GridModel(_nzbQueue.AllItems()));
        }


        [GridAction]
        public ActionResult _AjaxBinding2()
        {
            var q = _nzbQueue.AllItems().Select(c => new
            {
                c.Id,
                c.Name,
                c.Size,
                c.Remaining,
                c.Category,
                c.PostProcessing,
                c.Script,
            });

            return View(new GridModel(q));
        }
    }
}
