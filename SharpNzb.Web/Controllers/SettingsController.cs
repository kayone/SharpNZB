using System.Web.Mvc;
using SharpNzb.Core.Providers;
using SharpNzb.Web.Models;

namespace SharpNzb.Web.Controllers
{
    public class SettingsController : Controller
    {
        //
        // GET: /Settings/

        private IConfigProvider _configProvider;

        public SettingsController(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public ActionResult Index()
        {
            var settings = new SettingsModel();
            settings.TempDir = _configProvider.GetValue("TempDir", "download", true);
            settings.CompleteDir = _configProvider.GetValue("CompleteDir", "complete", true);

            return View(settings);
        }

        [HttpPost]
        public ActionResult Index(SettingsModel model)
        {
            if (ModelState.IsValid)
            {
                _configProvider.SetValue("TempDir", model.TempDir);
                _configProvider.SetValue("CompleteDir", model.CompleteDir);
                //return RedirectToAction("index");
            }
            return RedirectToAction("index", "Sabnzbd");
        }

    }
}
