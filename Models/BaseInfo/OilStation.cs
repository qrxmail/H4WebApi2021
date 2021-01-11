using System;
using System.ComponentModel.DataAnnotations;

namespace H4WebApi.Models.BaseInfo
{
    public class OilStation
    {
		/// <summary>
		/// 主键
		/// </summary>
		[Key]
		public Guid PK { get; set; }

        /// <summary>
        /// 所属单位
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 管理区
        /// </summary>
		public string District { get; set; }

        /// <summary>
        /// 站名
        /// </summary>
        public string Name { get; set; }

        public string PLCIP { get; set; }

		public string HMIIP { get; set; }

		public float VolumnPer1cm { get; set; }

		public float LevelCalcFactor { get; set; }

		public float LevelCalcOffset { get; set; }

		public float PumpRatedFlow { get; set; }

		public float PumpCalcFactor { get; set; }

		public float PumpCalcOffset { get; set; }

		public string Longitude { get; set; }

		public string Latitude { get; set; }

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
