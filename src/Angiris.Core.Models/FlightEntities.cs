using Angiris.Core.Models;
using Angiris.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Core.Models
{
    // 航班舱位信息实体
    public class FlightCabin
    {
        // 舱位名称
        public string CabinName { get; set; }
        // 机票价格
        public decimal CabinPrice { get; set; }
        // 机场建设费
        public decimal AirPortFee { get; set; }
        // 燃油附加费
        public decimal FuelFee { get; set; }
        // 其他费用
        public decimal OtherFee { get; set; }
        // 保险产品名
        public string InsuranceName { get; set; }
        // 保险产品价格
        public decimal InsurancePrice { get; set; }
        // 货币单位
        public string CurrencyType { get; set; } 

    }

    // 航班查询请求实体（任务输入）
    public class FlightRequest : IEquatable<FlightRequest>
    {
        // 航司名称
        public string Company { get; set; }
        // 起飞城市三字码
        public string DepartureCity { get; set; }
        // 到达城市三字码
        public string ArrivalCity { get; set; }
        // 起飞日期      
        public DateEpoch FlightDate { get; set; }

        public string DistinctHash
        {
            get
            {
                return ToDistinctHash(this);
            }
        }


        public bool Equals(FlightRequest other)
        {
            return DistinctHash.Equals(other.DistinctHash);
        }

        public override int GetHashCode()
        {
            return DistinctHash.GetHashCode();
        }
        public override string ToString()
        {
            return DistinctHash;
        }

        public static string ToDistinctHash(FlightRequest entity)
        {
            return string.Format("{0}-{1}-{2:yyyyMMdd}-{3}", entity.DepartureCity, entity.ArrivalCity, entity.FlightDate.Date, entity.Company);
        }
    }

    // 航班查询响应实体 （任务输出）
    public class FlightResponse
    {
        // 航司名称
        public string Company { get; set; }
        // 航班号
        public string FlightNumber { get; set; }

        private DateEpoch flightDate;
        // 起飞日期
        public DateEpoch FlightDate
        {
            get { return flightDate; }
            set
            {
                flightDate = value.Date.Date;//force to 12:00AM.
            }
        }
        //出发城市
        public string DepartureCity { get; set; }
        // 到达城市
        public string ArrivalCity { get; set; }
        // 起飞时间
        public DateEpoch DepartureTime { get; set; }
        // 到达时间
        public DateEpoch ArrivalTime { get; set; }
        // 舱位信息
        public IEnumerable<FlightCabin> FlightCabins { get; set; }

        public DateEpoch TimeStamp { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get
            {
                return ToDistinctHash(this.FlightNumber, this.FlightDate);
            }
        }

        public static string ToDistinctHash(string flightNumber, DateTime flightDate)
        {
            return string.Format("{0}-{1:yyyyMMdd}", flightNumber, flightDate.Date);
        }
    }


    public class FakeFlightDataSource
    {
        public static List<string> CompanyNameList = new List<string>() { "SPRING", "AIRASIA","SOUTHERN","EASTERN","TIGER","SCOTT","JETBLUE"};
        public static List<string> CityNameList = new List<string>() { "aaa", "bbb", "ccc", "ddd", "eee", "fff", "ggg","hhh","iii","jjj","kkk" };


    }
}
