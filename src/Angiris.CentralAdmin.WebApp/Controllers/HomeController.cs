using Angiris.CentralAdmin.Core;
using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var telemetrydata = new List<DaemonStatus>();
            //await svc.GetAllDaemonStatusListAsync();
            Task.Run(async () =>
            {
                telemetrydata = await svc.GetAllDaemonStatusListAsync();
            }).Wait(); ;

            //TODO: understand why the behaviors diff

            //just always hang at await Database.HashGetAllAsync(this.DaemonStatusHashKeyName);
            //1st try
            //telemetrydata = svc.GetAllDaemonStatusListAsync().Result;

            //2nd try
            //var datatask = svc.GetAllDaemonStatusListAsync();
            //datatask.Wait();
            //telemetrydata = datatask.Result; 


            return View(telemetrydata);
        }
    }
}