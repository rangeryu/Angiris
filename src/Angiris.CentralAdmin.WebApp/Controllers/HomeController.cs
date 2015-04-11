using Angiris.CentralAdmin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Angiris.CentralAdmin.WebApp.Controllers
{
    public class HomeController : Controller
    {
        TelemetryService svc = new TelemetryService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DaemonStatus()
        {
            var telemetrydata = svc.GetAllDaemonStatusList().Result;



            return View(telemetrydata);
        }
    }
}