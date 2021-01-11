using System;
using System.ComponentModel.DataAnnotations;

namespace H4WebApi.Models.BaseInfo
{
    public class Truck
    {
        /// <summary>
		/// 主键
		/// </summary>
		[Key]
        public Guid PK { get; set; }

        public string Company { get; set; }

        public string Number { get; set; }

        public float Volumn { get; set; }

        public string LeadSealNumber { get; set; }

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
