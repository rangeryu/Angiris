namespace Angiris.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public interface IEntitySnapshot 
	{
		bool IsFromCache { get;set; }

		DateTime TimeStamp { get;set; }

	}
}

