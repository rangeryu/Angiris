using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Angiris.CentralAdmin.Core;
using Microsoft.AspNet.SignalR;


namespace Angiris.CentralAdmin.WebApp.Controllers
{
    public class ScheduledTaskController : Controller
    {
        // GET: ScheduledTask
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _GenerateTasks(int taskCount)
        {
            ScheduledFlightCrawlRequestFactory flightCrawlRequestFactory = new ScheduledFlightCrawlRequestFactory();
             
            var task = Task.Run(async () =>
            {
                try
                {
                    await flightCrawlRequestFactory.StartPushTaskMessages(taskCount, BroadcastPushTaskUpdate);
                }
                catch (Exception ex)
                {
                    BroadcastPushTaskUpdate(ex.Message + System.Environment.NewLine + ex.InnerException.Message);
                }

            });
            //task.Wait();
            return View();
        }

        private static readonly IHubContext HubContext = GlobalHost.ConnectionManager.GetHubContext<CrawlResultHub>();

        private void BroadcastPushTaskUpdate(string msg)
        {
            Task.Run(() =>
            {
                HubContext.Clients.All.addNewMessage(msg);
            });
        }
    }
}