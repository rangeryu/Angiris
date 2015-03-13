using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    public class FlightEntityCrawlRequest : IEntityCrawlRequest<FlightRequest,FlightResponse>, IQueuedTask
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

        public DateTime CreateTime
        {
            get;
            set;
        }

        public DateTime FinishTime
        {
            get;
            set;
        }

        public DateTime LastModifiedTime
        {
            get;
            set;
        }
    }
}
