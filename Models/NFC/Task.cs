using System;
using System.Collections.Generic;

namespace NFCWebApi.Models
{
    public class Task
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 巡检人
        /// </summary>
        public string InspectUser { get; set; }

        /// <summary>
        /// 巡检时间
        /// </summary>
        public string InspectTime { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public string IsComplete { get; set; }

        /// <summary>
        /// 巡检点信息
        /// </summary>
        public List<InspectPoint> InspectPointList { get; set; }


    }


    /// <summary>
    /// 巡检点信息
    /// </summary>
    public class InspectPoint
    {
        /// <summary>
        /// 巡检点编号
        /// </summary>
        public string InspectNo { get; set; }

        /// <summary>
        /// 巡检点名称
        /// </summary>
        public string InspectName { get; set; }

        /// <summary>
        /// 巡检顺序号
        /// </summary>
        public string InspectOrderNo { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public List<DeviceInfo> DeviceList { get; set; }

       
    }


    /// <summary>
    /// 巡检项目信息
    /// </summary>
    public class InspectItemsInfo
    {
        /// <summary>
        /// 巡检项目名称
        /// </summary>
        public string InspectItemName { get; set; }

        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// 数量单位
        /// </summary>
        public string Unit { get; set; }
    }

    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfo
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
      
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// NFC卡编号
        /// </summary>
        public string NFCCardNo { get; set; }

        /// <summary>
        /// 巡检项目信息
        /// </summary>
        public List<InspectItemsInfo> InspectItemList { get; set; }


    }

    
}
