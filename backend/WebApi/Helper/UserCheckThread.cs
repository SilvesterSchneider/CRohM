using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using ServiceLayer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Helper
{
    /// <summary>
    /// Der Check-Service der User-Login Zeitpunkte
    /// </summary>
    public class UserCheckThread 
    {
        /// <summary>
        /// Der Job der von Quartz Service ausgeführt wird
        /// </summary>
        private class ExecuteJob : IJob
        {
            private DateTime dateTime = DateTime.MinValue;
            private IUserCheckDateService userCheckSevice;
            private TimeSpan addToCheckDate;

            public async Task Execute(IJobExecutionContext context)
            {
                JobDataMap jobData = context.JobDetail.JobDataMap;
                userCheckSevice = jobData.Get("service") as IUserCheckDateService;
                addToCheckDate = jobData.GetTimeSpanValue("addToCheckDate");
                await CheckAction();
            }

            private async Task CheckAction()
            {
                dateTime = userCheckSevice.GetTheDateTime();
                if (DateTime.Now > dateTime)
                {
                    dateTime = DateTime.Now.AddDays(addToCheckDate.TotalDays);
                    await userCheckSevice.UpdateAsync(dateTime);
                    await userCheckSevice.CheckAllUsersAsync();
                }
            }
        }

        private IUserCheckDateService userCheckSevice;
        private IConfiguration Configuration;


        public UserCheckThread(IUserCheckDateService userCheckSevice, IConfiguration configuration)
        {
            this.userCheckSevice = userCheckSevice;
            this.Configuration = configuration;
        }

        /// <summary>
        /// Starte den Quartz-Service
        /// </summary>
        /// <returns></returns>
        public async Task runScheduledService()
        {
            // construct a scheduler factory
            StdSchedulerFactory factory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();
            // job data map
            JobDataMap jobData = new JobDataMap();
            jobData.Add("service", userCheckSevice);

            TimeSpan addToCheckDate = TimeSpan.FromDays(1);
            TimeSpan.TryParse(Configuration["DeleteInactiveUsers:AddToCheckDate"], out addToCheckDate);
            jobData.Add("addToCheckDate", addToCheckDate);

            // define the job and tie it to our ExecuteJob class
            IJobDetail job = JobBuilder.Create<ExecuteJob>()
                .UsingJobData(jobData)
                .WithIdentity("myJob", "group")
                .Build();

            TimeSpan schedule = TimeSpan.FromHours(4);
            TimeSpan.TryParse(Configuration["DeleteInactiveUsers:ScheduleInterval"], out schedule);

            // Trigger the job to run now, and then every x hours (see configuration)
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(schedule)
                    .RepeatForever())
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
