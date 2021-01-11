using System;
using System.ComponentModel.DataAnnotations;

namespace NFCWebApi.Models
{
    /// <summary>
    /// 设备和巡检项目关系表：多对多关系
    /// </summary>
    public class DeviceInspectItem
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid GId { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 巡检项目编号
        /// </summary>
        public string InspectItemNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
    }

    /// <summary>
    /// 设备新增多个巡检项目的模型
    /// </summary>
    public class DeviceInspectItemAddModel
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 巡检项目编号多个
        /// </summary>
        public string[] InspectItemNos { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

    }
}
