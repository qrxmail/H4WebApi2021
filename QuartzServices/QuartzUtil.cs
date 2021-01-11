using log4net;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace H4WebApi.Services
{
    public class QuartzUtil
    {
        private static ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(QuartzUtil));

        private static ISchedulerFactory _schedulerFactory;
        private static IScheduler _scheduler;

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="type">类</param>
        /// <param name="jobKey">键</param>
        /// <param name="trigger">触发器</param>
        public static async Task Add(Type type, JobKey jobKey, ITrigger trigger = null)
        {
            // 初始化工场类
            Init();

            //通过工场类获得调度器
            _scheduler = await _schedulerFactory.GetScheduler();
            //开启调度器
            await _scheduler.Start();

            //创建触发器(也叫时间策略)
            if (trigger == null)
            {
                trigger = TriggerBuilder.Create()
                    .WithIdentity("april.trigger")
                    .WithDescription("default")
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow().WithRepeatCount(-1))
                    .Build();
            }
            //创建作业实例
            //type即我们需要执行的作业
            var jobDetail = JobBuilder.Create(type)
                .WithIdentity(jobKey)
                .Build();

            log.Info($"添加任务{jobKey.Group},{jobKey.Name}");
            //将触发器和作业任务绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="jobKey">键</param>
        public static async Task Resume(JobKey jobKey)
        {
            Init();
            _scheduler = await _schedulerFactory.GetScheduler();
            log.Info($"恢复任务{jobKey.Group},{jobKey.Name}");
            await _scheduler.ResumeJob(jobKey);
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        /// <param name="jobKey">键</param>
        public static async Task Stop(JobKey jobKey)
        {
            Init();
            _scheduler = await _schedulerFactory.GetScheduler();
            log.Info($"暂停任务{jobKey.Group},{jobKey.Name}");
            await _scheduler.PauseJob(jobKey);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobKey">键</param>
        public static async Task Delete(JobKey jobKey)
        {
            Init();
            _scheduler = await _schedulerFactory.GetScheduler();
            log.Info($"删除任务{jobKey.Group},{jobKey.Name}");
            await _scheduler.DeleteJob(jobKey);
        }

        /// <summary>
        /// 初始化工场类
        /// </summary>
        private static void Init()
        {
            if (_schedulerFactory == null)
            {
                _schedulerFactory = new StdSchedulerFactory();
            }
        }
    }
}
