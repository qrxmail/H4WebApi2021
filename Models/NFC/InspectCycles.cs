using System;
using System.ComponentModel.DataAnnotations;

namespace NFCWebApi.Models
{
    /// <summary>
    /// 巡检周期
    /// </summary>
    public class InspectCycles
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid GId { get; set; }

        /// <summary>
        /// 周期名称
        /// </summary>
        public string CycleName { get; set; }

        /// <summary>
        /// 周期类型
        /// </summary>
        public string CycleType { get; set; }

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
