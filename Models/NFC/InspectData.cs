using System;
using System.ComponentModel.DataAnnotations;

namespace NFCWebApi.Models
{

    /// <summary>
    /// 巡检数据
    /// </summary>
    public class InspectData
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid GId { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public string TaskNo { get; set; }

        /// <summary>
        /// 巡检点编号
        /// </summary>
        public string InspectNo { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 巡检项目编号
        /// </summary>
        public string InspectItemNo { get; set; }

        /// <summary>
        /// 巡检结果值
        /// </summary>
        public double ResultValue { get; set; }

        /// <summary>
        /// 是否跳检
        /// </summary>
        public string IsJumpInspect { get; set; }

        /// <summary>
        /// 跳检原因
        /// </summary>
        public string JumpInspectReason { get; set; }

        /// <summary>
        /// 巡检时间
        /// </summary>
        public DateTime InspectTime { get; set; }

        /// <summary>
        /// 巡检人
        /// </summary>
        public string InspectUser { get; set; }

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
