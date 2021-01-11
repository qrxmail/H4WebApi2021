using System;
using System.ComponentModel.DataAnnotations;

namespace NFCWebApi.Models
{
    /// <summary>
    /// 巡检路线
    /// </summary>
    public class InspectLine
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid GId { get; set; }

        /// <summary>
        /// 路线名称
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 设备巡检项目（存多个，以逗号分隔，DeivceInspectItem表的主键）
        /// </summary>
        public string DeviceInspectItems { get; set; }

        /// <summary>
        /// 设备巡检项目名称
        /// </summary>
        public string DeviceInspectItemsName { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string LastUpdateUser { get; set; }


    }
}
