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

            public async Task Execute(IJobExecutionContext context)
            {
                JobDataMap jobData = context.JobDetail.JobDataMap;
                userCheckSevice = jobData.Get("service") as IUserCheckDateService;
                await CheckAction();
            }

            private async Task CheckAction()
            {
                dateTime = userCheckSevice.GetTheDateTime();
                if (DateTime.Now > dateTime)
                {
                    dateTime = DateTime.Now.AddDays(1);
                    await userCheckSevice.UpdateAsync(dateTime);
                    await userCheckSevice.CheckAllUsersAsync();
                }
            }
        }

        private IUserCheckDateService userCheckSevice;

        public UserCheckThread(IUserCheckDateService userCheckSevice)
        {
            this.userCheckSevice = userCheckSevice;
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
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ExecuteJob>()
                .UsingJobData(jobData)
                .WithIdentity("myJob", "group")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(2)
                    .RepeatForever())
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
