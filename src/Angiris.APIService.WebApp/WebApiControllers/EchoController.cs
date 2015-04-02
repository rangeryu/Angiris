using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Angiris.APIService.WebApp.WebApiControllers
{
    public class EchoController : ApiController
    {
        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public object Get()
        {
            string jsonPath = System.Web.HttpContext.Current.Server.MapPath("~/Content/json2.json");
            string jsonContent = File.ReadAllText(jsonPath);
            var obj = JsonConvert.DeserializeObject(jsonContent);
            return obj;
            //return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}