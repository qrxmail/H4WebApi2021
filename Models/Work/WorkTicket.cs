using System;
using System.ComponentModel.DataAnnotations;

namespace H4WebApi.Models.Work
{
    public class WorkTicket
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid PK { get; set; }

        /// <summary>
        /// 装油站
        /// </summary>
        public string LoadStation { get; set; }

        /// <summary>
        /// 油罐
        /// </summary>
        public string OilPot { get; set; }

        /// <summary>
        /// 卸油站
        /// </summary>
        public string UnloadStation { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 可发油量m³
        /// </summary>
        public float OilLoadedMax { get; set; }

        /// <summary>
        /// 铅封号
        /// </summary>
        public string SubSerialNumber { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNumber { get; set; }

        public DateTime? LoadingBeginTime { get; set; }

        public DateTime? LoadingEndTime { get; set; }

        public DateTime? LoadingActualBeginTime { get; set; }

        public DateTime? LoadingActualEndTime { get; set; }

        /// <summary>
        /// 装油量
        /// </summary>
        public float OilLoaded { get; set; }

        /// <summary>
        /// 司机姓名
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// 司机电话
        /// </summary>
        public string DriverPhone { get; set; }

        public string OilLoader { get; set; }

        public DateTime? UnloadingBeginTime { get; set; }

        public DateTime? UnloadingEndTime { get; set; }

        public float OilUnloaded { get; set; }

        public float OilError { get; set; }

        public string OilUnloader { get; set; }

        public string Description { get; set; }

        public string Reviewer { get; set; }

        public string Status { get; set; }

        public float LevelBeginLoad { get; set; }

        public float LevelAfterLoad { get; set; }

        public float TaskDuration { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

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

    public enum WorkTicketStatus { 待接单, 待授权, 待拉油, 待卸油, 待审批, 已完成, 已作废, 已删除 }

    public class WorkTicketView : WorkTicket
    {
        public string LoadStationName { get; set; }
        public string LoadStationBranch { get; set; }
        public string LoadStationDistrict { get; set; }
        public string UnloadStationName { get; set; }
        public string UnloadStationBranch { get; set; }
        public string UnloadStationDistrict { get; set; }

        public string TruckNo { get; set; }
        public string TruckCompany { get; set; }
        public string DrvierName { get; set; }
    }
}
