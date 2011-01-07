using System.Web.Mvc;
using SharpNzb.Core.Providers;

namespace SharpNzb.Web.Controllers
{
    public class NotificationController : Controller
    {
        //
        // GET: /Notification/

        private readonly INotificationProvider _notifications;
        //
        // GET: /Notification/

        public NotificationController(INotificationProvider notificationProvider)
        {
            _notifications = notificationProvider;
        }

        [HttpGet]
        public JsonResult Index()
        {
            string message = string.Empty;
            if (_notifications.GetProgressNotifications.Count != 0)
            {
                message = _notifications.GetProgressNotifications[0].CurrentStatus;
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

    }
}
