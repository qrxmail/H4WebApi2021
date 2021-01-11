using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Quartz;
using H4WebApi.Services;
using System.Threading.Tasks;

namespace H4WebApi.Controllers.Quartz
{
    [Route("api")]
    [ApiController]
    public class QuartzController : ControllerBase
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(QuartzController));

        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        private readonly IHubContext<ChatHub> _hub;

        private readonly IConfiguration _configuration;
       
        public QuartzController(ISchedulerFactory schedulerFactory, IHubContext<ChatHub> hub, IConfiguration configuration)
        {
            _schedulerFactory = schedulerFactory;
            _hub = hub;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("QuartzTask")]
        public void QuartzTask(string type, string jobName, string jobGroup)
        {
            JobKey jobKey = new JobKey(jobName, jobGroup);
            switch (type)
            {
                //添加任务
                case "add":
                    //创建触发器(也叫时间策略)
                    var trigger = TriggerBuilder.Create()
                            .WithDescription("workTicket")
                            .WithIdentity("admin")
                            //.WithSchedule(CronScheduleBuilder.CronSchedule("0 0/30 * * * ? *").WithMisfireHandlingInstructionDoNothing())
                            // 间隔固定时间执行
                            //.WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
                            // 只执行一次
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).WithRepeatCount(0).WithMisfireHandlingInstructionIgnoreMisfires())
                            .Build();
                    switch (jobName)
                    {
                        case "AutoOperWorkTicket":
                            //实例化Jobs用到的对象（singalr）
                            AutoOperWorkTicketJobs._hub = _hub;
                            //实例化Jobs用到的对象JobService
                            AutoOperWorkTicketJobs.jobService = new JobService(_configuration);
                            _ = QuartzUtil.Add(typeof(AutoOperWorkTicketJobs), jobKey, trigger);
                            break;
                        case "AutoOperQrCode":
                            //实例化Jobs用到的对象（singalr）
                            AutoOperQrCodeJobs._hub = _hub;
                            //实例化Jobs用到的对象JobService
                            AutoOperQrCodeJobs.jobService = new JobService(_configuration);
                            _ = QuartzUtil.Add(typeof(AutoOperQrCodeJobs), jobKey, trigger);
                            break;
                        case "AutoRemindToWork":
                            //实例化Jobs用到的对象（singalr）
                            AutoRemindToWorkJobs._hub = _hub;
                            //实例化Jobs用到的对象JobService
                            AutoRemindToWorkJobs.jobService = new JobService(_configuration);
                            _ = QuartzUtil.Add(typeof(AutoRemindToWorkJobs), jobKey, trigger);
                            break;
                    }
                    
                    break;
                //暂停任务
                case "stop":
                    _ = QuartzUtil.Stop(jobKey);
                    break;
                //恢复任务
                case "resume":
                    _ = QuartzUtil.Resume(jobKey);
                    break;
                //删除任务
                case "delete":
                    _ = QuartzUtil.Delete(jobKey);
                    break;
            }
        }

        /// <summary>
        /// 定时任务测试
        /// </summary>
        /// <returns></returns>
        [Route("taskTest")]
        [HttpGet]
        public async Task TaskTest()
        {
            log.Info("定时任务测试开始");

            //通过工场类获得调度器
            _scheduler = await _schedulerFactory.GetScheduler();
            //开启调度器
            await _scheduler.Start();
            //创建触发器(也叫时间策略)
            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever())//每10秒执行一次
                            .Build();
            //实例化Jobs用到的对象（singalr）
            AutoOperWorkTicketJobs._hub = _hub;
            //实例化Jobs用到的对象JobService
            AutoOperWorkTicketJobs.jobService = new JobService(_configuration);
            //创建作业实例
            //Jobs即我们需要执行的作业
            var jobDetail = JobBuilder.Create<AutoOperWorkTicketJobs>()
                            .WithIdentity("Myjob", "group")//我们给这个作业取了个“Myjob”的名字，并取了个组名为“group”
                            .Build();
            //将触发器和作业任务绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);

            log.Info("定时任务测试结束");
        }
    }

}
