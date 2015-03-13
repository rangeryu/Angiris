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

	}




}

