using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Angiris.CentralAdmin.Core;
using Angiris.Core.Models;

namespace Angiris.CentralAdmin.WebApp.Controllers
{
    public class DashboardController : Controller
    {
        readonly TelemetryService _svc = new TelemetryService();

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DaemonStatus()
        {

            var telemetrydata = new List<DaemonStatus>();
            //await svc.GetAllDaemonStatusListAsync();
            Task.Run(async () =>
            {
                telemetrydata = await _svc.GetAllDaemonStatusListAsync();
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