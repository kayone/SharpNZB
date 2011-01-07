using System.Web.Mvc;

namespace SharpNzb.Web.Controllers
{
    public class NzbController : Controller
    {
        //
        // GET: /Nzb/

        public ActionResult Index()
        {
            return View();
        }

    }
}
