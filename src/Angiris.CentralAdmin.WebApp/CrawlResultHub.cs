using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Angiris.CentralAdmin.WebApp
{
    public class CrawlResultHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string msg)
        {
            Clients.All.addNewMessage(msg);
        }
    }
}