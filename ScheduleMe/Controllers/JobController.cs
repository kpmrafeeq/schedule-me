using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using ScheduleMe.Job;
using ScheduleMe.Model;
using System.Runtime;
using System.Text.Json;

namespace ScheduleMe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly ISchedulerFactory _scheduleFactory;
        private readonly ILogger<JobController> _logger;

        public JobController(ISchedulerFactory scheduleFactory, ILogger<JobController> logger)
        {
            _scheduleFactory = scheduleFactory;
            _logger = logger;
        }

        [Route("schedule")]
        [HttpPost]
        public async Task<IActionResult> ScheduleJob([FromBody] JobRequest request)
        {
            var scheduler = await _scheduleFactory.GetScheduler();
            _logger.LogInformation("Scheduling job");
            if (string.IsNullOrEmpty(request.CronExpression))
            {
                return BadRequest("Cron expression is required.");
            }

            var job = JobBuilder.Create<HttpJob>()
                .WithIdentity(request.Name)
                .UsingJobData("Url", request.Url)
                .UsingJobData("Headers", JsonSerializer.Serialize(request.Headers))
                .UsingJobData("Body", request.Body)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(request.CronExpression)
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return Ok("Job scheduled successfully");
        }

        [Route("list")]
        [HttpGet]
        public async Task<IActionResult> ListJobs()
        {
            var scheduler = await _scheduleFactory.GetScheduler();
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            var jobs = new List<object>();

            foreach (var jobKey in jobKeys)
            {
                var detail = await scheduler.GetJobDetail(jobKey);
                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                foreach (var trigger in triggers)
                {
                    var cronTrigger = trigger as ICronTrigger;
                    var cronExpression = cronTrigger?.CronExpressionString;

                    var nextFireTime = trigger.GetNextFireTimeUtc()?.ToLocalTime();
                    var state = await scheduler.GetTriggerState(trigger.Key);

                    var jobInfo = new
                    {
                        JobKey = jobKey.Name,
                        JobGroup = jobKey.Group,
                        CronExpression = cronExpression,
                        NextFireTime = nextFireTime,
                        Status = state.ToString(),
                        Url = detail?.JobDataMap.GetString("Url"),
                        Headers = detail?.JobDataMap.GetString("Headers"),
                        Body = detail?.JobDataMap.GetString("Body")
                    };


                    var formattedJobInfo = $@"
                        JobKey:         {jobInfo.JobKey}
                            JobGroup:       {jobInfo.JobGroup}
                            CronExpression: {jobInfo.CronExpression}
                            NextFireTime:   {jobInfo.NextFireTime}
                            Status:         {jobInfo.Status}
                            Url:            {jobInfo.Url}
                            Headers:        {jobInfo.Headers}
                            Body:           {jobInfo.Body}
                    ";

                    jobs.Add(formattedJobInfo);
                }
            }

            return Ok(string.Join("\n", jobs));
        }


        [Route("pause/{jobName}")]
        [HttpPost]
        public async Task<IActionResult> PauseJob(string jobName)
        {
            var scheduler = await _scheduleFactory.GetScheduler();
            var jobKey = new JobKey(jobName);

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.PauseJob(jobKey);
                _logger.LogInformation("Job {JobName} paused successfully", jobName);
                return Ok($"Job {jobName} paused successfully");
            }
            else
            {
                _logger.LogWarning("Job {JobName} not found", jobName);
                return NotFound($"Job {jobName} not found");
            }
        }

        [Route("stop/{jobName}")]
        [HttpPost]
        public async Task<IActionResult> StopJob(string jobName)
        {
            var scheduler = await _scheduleFactory.GetScheduler();
            var jobKey = new JobKey(jobName);

            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
                _logger.LogInformation("Job {JobName} stopped successfully", jobName);
                return Ok($"Job {jobName} stopped successfully");
            }
            else
            {
                _logger.LogWarning("Job {JobName} not found", jobName);
                return NotFound($"Job {jobName} not found");
            }
        }
    }
}
