namespace Angiris.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public interface IEntityCrawlRequest<TReq,TResp>
	{
        TReq RequestData { get; set; }
        List<TResp> ResponseData { get; set; }

        TaskStatus Status { get; set; }

        Guid TaskID { get; set; }
         
        string LogData { get; set; }

        DateTime CreateTime { get; set; }

        DateTime FinishTime { get; set; }

        DateTime LastModifiedTime { get; set; }
	}


}

