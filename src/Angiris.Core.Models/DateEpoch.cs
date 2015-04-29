using System;
using System.Collections.Generic;
using System.Globalization;
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
        private DateTime dateTime = DateTime.MaxValue;
        public DateTime Date { get { return dateTime; } set { dateTime = value; } }
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

        public override string ToString()
        {
            return Date.ToString(CultureInfo.InvariantCulture);
        }
        
    }
}
