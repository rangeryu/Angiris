using Angiris.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public class FlightCrawlEntity : IEntityCrawlRequest<FlightRequest,FlightResponse>, IQueuedTask
    {
        public FlightRequest RequestData
        {
            get;
            set;
        }

        private List<FlightResponse> responseData = new List<FlightResponse>();
        public List<FlightResponse> ResponseData
        {
            get
            {
                return responseData; 
            }
            set
            {
                responseData = value;
            }                
        }


        public TaskStatus Status
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "id", ItemConverterType = typeof(GuidConverter))]
        public Guid TaskID
        {
            get;
            set;
        }

        public string LogData
        {
            get;
            set;
        }

        public DateEpoch CreateTime
        {
            get;
            set;
        }

        public DateEpoch FinishTime
        {
            get;
            set;
        }

        public DateEpoch LastModifiedTime
        {
            get;
            set;
        }
    }
}
