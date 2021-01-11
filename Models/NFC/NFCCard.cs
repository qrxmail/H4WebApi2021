using System;
using System.ComponentModel.DataAnnotations;

namespace NFCWebApi.Models
{
    /// <summary>
    /// 巡检卡
    /// </summary>
    public class NFCCard
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid GId { get; set; }

        /// <summary>
        /// NFC卡编号
        /// </summary>
        public string NFCCardNo { get; set; }

        /// <summary>
        /// 印刷编号
        /// </summary>
        public string PrintNo { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

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

        /// <summary>
        /// 最后巡检时间
        /// </summary>
        public DateTime LastInspectTime { get; set; }

        /// <summary>
        /// 最后巡检人
        /// </summary>
        public string LastInspectUser { get; set; }


    }
}
