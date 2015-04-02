using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using RestSharp.Extensions;
using RestSharp.Validation;

namespace Angiris.CentralAdmin.WebApp.Controllers
{
    public class ApiController
    {
        private static readonly string _secretKey = "key-c71cc13573773b936e20883d961dcb84";

        public static IRestResponse SendEmail(string To, string From, string Body, string Subject)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v2");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               _secretKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "alephzero.cn", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "No-Reply <no-reply@alephzero.cn>");
            request.AddParameter("to", To);
            request.AddParameter("subject", Subject);
            request.AddParameter("text", Body);
            request.AddParameter("o:tracking", true);
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }
}