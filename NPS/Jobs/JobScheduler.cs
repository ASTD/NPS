using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.Jobs
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<SlackNotificationJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                  .StartNow()
                  .WithSimpleSchedule(x => x
                      .WithIntervalInMinutes(10)
                      .RepeatForever())
                  .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}