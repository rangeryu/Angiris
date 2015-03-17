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
    public class FlightRequest
    {
        // 航司名称
        public string Company { get; set; }
        // 起飞城市三字码
        public string DeptureCity { get; set; }
        // 到达城市三字码
        public string ArrivalCity { get; set; }
        // 起飞日期      
        public DateEpoch FlightDate { get; set; }
    }

    // 航班查询响应实体 （任务输出）
    public class FlightResponse
    {
        // 航司名称
        public string Company { get; set; }
        // 航班号
        public string FlightNumber { get; set; }
        // 起飞日期
        public DateEpoch FlightDate { get; set; }
        //出发城市
        public string DeptureCity { get; set; }
        // 到达城市
        public string ArrivalCity { get; set; }
        // 起飞时间
        public DateEpoch DeptureTime { get; set; }
        // 到达时间
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateEpoch ArrivalTime { get; set; }
        // 舱位信息
        public IEnumerable<FlightCabin> FlightCabins { get; set; }
    }


    public class FakeFlightDataSource
    {
        public static List<string> CompanyNameList = new List<string>() { "SPRING", "AIRASIA"};
        public static List<string> CityNameList = new List<string>() { "aaa", "bbb", "ccc", "ddd", "eee", "fff", "ggg" };


    }
}
