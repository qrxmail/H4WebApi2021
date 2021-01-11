using log4net;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using H4WebApi.Models;
using System;
using System.Threading.Tasks;

namespace H4WebApi.Services
{
    /// <summary>
    /// 自动处理工单
    /// </summary>
    public class AutoOperWorkTicketJobs : IJob
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(AutoOperWorkTicketJobs));

        // 通过静态变量实例化
        public static IHubContext<ChatHub> _hub { get; set; }

        public static JobService jobService { get; set; }

        // 定时任务执行方法
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                log.Info("定时任务执行开始！！！！");

                // 通过Grpc获取站点数据
                string jsonStrData = StationControlService.GetStationJsonData();

                // 执行自动处理工单的方法
                ResultObj resultObj = jobService.AutoOperWorkTicket(jsonStrData);

                // 如果修改了工单状态，则推送消息
                if (resultObj.IsSuccess)
                {
                    // 推送工单处理消息
                    _hub.Clients.All.SendAsync("ReceiveMessage", "自动处理工单消息推送", "工单" + resultObj.KeyInfo + "拉油完成。" + "\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                // 推送工单处理过程日志
                _hub.Clients.All.SendAsync("ReceiveLogOperWorkTicket", "自动处理工单日志推送", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + resultObj.Msg);

                log.Info("定时任务执行结束！！！！");
            });
        }
    }

    /// <summary>
    /// 自动处理二维码信息
    /// </summary>
    public class AutoOperQrCodeJobs : IJob
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(AutoOperQrCodeJobs));

        // 通过静态变量实例化
        public static IHubContext<ChatHub> _hub { get; set; }

        public static JobService jobService { get; set; }

        // 定时任务执行方法
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                log.Info("定时任务执行开始！！！！");

                // 通过Grpc获取站点数据
                string jsonStrData = StationControlService.GetStationJsonData();

                // 接收二维码信息，发送工单信息给grpc服务器
                ResultObj resultObj = jobService.AutoOperQrCode(jsonStrData);

                // 如果修改了工单状态，则推送消息
                if (resultObj.IsSuccess)
                {
                    // 推送处理二维码消息
                    _hub.Clients.All.SendAsync("ReceiveMessage", "二维码信息处理成功!", "");
                }
                // 推送处理二维码信息过程日志
                _hub.Clients.All.SendAsync("ReceiveLogOperQrCode", "二维码信息处理日志推送", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + resultObj.Msg);

                log.Info("定时任务执行结束！！！！");
            });
        }
    }

    /// <summary>
    /// 自动提醒开工单
    /// </summary>
    public class AutoRemindToWorkJobs : IJob
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(AutoRemindToWorkJobs));

        // 通过静态变量实例化
        public static IHubContext<ChatHub> _hub { get; set; }

        public static JobService jobService { get; set; }

        // 定时任务执行方法
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                log.Info("定时任务执行开始！！！！");

                // 通过Grpc获取站点数据
                string jsonStrData = StationControlService.GetStationJsonData();

                // 自动提醒开工单
                ResultObj resultObj = jobService.AutoRemindToWork(jsonStrData);
                // 如果可开工单了，则推送提醒消息
                if (resultObj.IsSuccess)
                {
                    // 开工单消息推送
                    _hub.Clients.All.SendAsync("ReceiveMessage", "开工单消息推送", resultObj.KeyInfo + "\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                // 推送提醒开工单过程日志
                _hub.Clients.All.SendAsync("ReceiveLogRemindToWork", "开工单处理日志推送", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + resultObj.Msg);

                log.Info("定时任务执行结束！！！！");
            });
        }
    }
}
