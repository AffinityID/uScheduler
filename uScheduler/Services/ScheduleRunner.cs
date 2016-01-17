using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using log4net;
using uScheduler.Models;
using uScheduler.Models.Enums;
using uScheduler.Services.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace uScheduler.Services {
    public class ScheduleRunner : IScheduleRunner {
        private readonly ILog _log;
        protected readonly UmbracoDatabase Database;

        public ScheduleRunner(ApplicationContext applicationContext) {
            _log = LogManager.GetLogger(GetType());
            Database = applicationContext.DatabaseContext.Database;
        }

        public void Run() {
            var query = new Sql()
                .Select("*")
                .From(SchedulerConstants.Database.ScheduleTable)
                .Where("Disabled = 0");

            var schedules = Database.Fetch<Schedule>(query);

            var tasks = schedules.Select(RunScheduledAsync).ToArray();
            Task.WaitAll(tasks);
        }

        public void Run(int userId, int id) {
            var query = new Sql()
                .Select("*")
                .From(SchedulerConstants.Database.ScheduleTable)
                .Where("Id =" + id);

            var schedule = Database.Fetch<Schedule>(query)
                .FirstOrDefault();

            RunAsync(userId, schedule).Wait();
        }

        private async Task RunScheduledAsync(Schedule schedule) {
            var currentTime = DateTime.UtcNow;
            if (schedule.NextRunUtc > currentTime) return;
                
            var frequency = (Frequency) Enum.Parse(typeof(Frequency), schedule.Frequency);

            var nextRun = GetNextRun(frequency, schedule.NextRunUtc);
            if (frequency == Frequency.Single) {
                schedule.Disabled = true;
            }
            else {
                //Handle scenarios where past executions may have been skipped.
                while (nextRun < currentTime) { 
                    nextRun = GetNextRun(frequency, nextRun);
                }
            }
            
            schedule.NextRunUtc = nextRun;

            
            Database.Update(schedule);

            await RunAsync(0, schedule);
        }

        private async Task RunAsync(int userId, Schedule schedule) {
            var log = new Log
            {
                ExecutionDateTimeUtc = DateTime.UtcNow,
                ScheduleId = schedule.Id,
                UserId = userId
            };

            try {
                var url = schedule.Url;
                if (!url.StartsWith("http"))
                    url = $"http://localhost{url}";
                using (var client = new HttpClient())
                {
                    var req = new HttpRequestMessage(new HttpMethod(schedule.HttpVerb), url);
                    if (!string.IsNullOrWhiteSpace(schedule.Data))
                    {
                        req.Content = new StringContent(schedule.Data);
                    }
                    var resp = await client.SendAsync(req).ConfigureAwait(false);
                    log.Result = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    log.Success = resp.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to run schedule {schedule.Name}", ex);
                log.Result = ex.Message;
            }

            Database.Save(log);
        }

        private static DateTime GetNextRun(Frequency frequency, DateTime previousRun) {
            switch (frequency) {
                case Frequency.Single:
                    return previousRun;
                case Frequency.Hourly:
                    return previousRun.AddHours(1);
                case Frequency.Daily:
                    return previousRun.AddDays(1);
                case Frequency.Weekly:
                    return previousRun.AddDays(7);
                case Frequency.Fortnightly:
                    return previousRun.AddDays(14);
                case Frequency.Monthly:
                    return previousRun.AddMonths(1);
                case Frequency.Quarterly:
                    return previousRun.AddMonths(3);
                default:
                    throw new NotImplementedException("The specified frequency has not been implemented.");
            }
        }
    }
}