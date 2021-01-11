using System;
using System.ComponentModel.DataAnnotations;

namespace H4WebApi.Models
{
    public class User
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid UserId { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户类型（登录类型，暂留，没用到）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 当前权限
        /// </summary>
      
        public string CurrentAuthority { get; set; }

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
