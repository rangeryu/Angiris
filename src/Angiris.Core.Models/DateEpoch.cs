using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angiris.Core.Utility;

namespace Angiris.Core.Models
{
    /// <summary>
    /// http://azure.microsoft.com/blog/2014/11/19/working-with-dates-in-azure-documentdb-4/
    /// approach 2 using dataepoch is recommended.
    /// </summary>
    public class DateEpoch
    {
        public DateTime Date { get; set; }
        public int Epoch
        {
            get
            {
                return (this.Date.Equals(null) || this.Date.Equals(DateTime.MinValue))
                    ? int.MinValue
                    : this.Date.ToEpoch();
            }
        }

        public static implicit operator DateTime(DateEpoch value)
        {
            return value.Date;
        }
        public static implicit operator DateEpoch(DateTime value)
        {
            return new DateEpoch() { Date = value };
        }
    }
}
