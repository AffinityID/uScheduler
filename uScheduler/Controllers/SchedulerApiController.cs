using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using uScheduler.Models;
using uScheduler.Models.Enums;
using uScheduler.Services;
using uScheduler.Services.Interfaces;
using Umbraco.Core.Persistence;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace uScheduler.Controllers {
    [PluginController(SchedulerConstants.Application.Name)]
    public class SchedulerApiController : UmbracoAuthorizedApiController {
        protected IScheduleRunner Runner;

        public SchedulerApiController() {
            Runner = new ScheduleRunner(ApplicationContext);
        }

        public IEnumerable<string> GetScheduleHttpVerbs() {
            return new[] {
                HttpMethod.Get.ToString(),
                HttpMethod.Put.ToString(),
                HttpMethod.Post.ToString(),
                HttpMethod.Delete.ToString()
            };
        }

        public IEnumerable<string> GetScheduleFrequencies() {
            return Enum.GetNames(typeof(Frequency));
        }

        public IEnumerable<Schedule> GetAllSchedules() {
            var query = new Sql()
                .Select("*")
                .From(SchedulerConstants.Database.ScheduleTable);
            return DatabaseContext.Database.Fetch<Schedule>(query);
        }

        public Schedule GetSchedule(int id) {
            var query = new Sql()
                .Select("*")
                .From(SchedulerConstants.Database.ScheduleTable)
                .Where("Id =" + id);

            return DatabaseContext.Database.Fetch<Schedule>(query)
                .FirstOrDefault();
        }

        [HttpGet]
        [AllowAnonymous]
        public void KeepAlive() {
            //This method is merely a stub so that external Keep Alive calls have something lightweight to call.
        }

        [HttpPost]
        public Schedule SaveSchedule([FromBody] Schedule schedule) {
            DatabaseContext.Database.Save(schedule);
            return schedule;
        }

        [HttpPost]
        public void RunSchedule([FromUri] int id) {
            Runner.Run(Security.CurrentUser.Id, id);
        }

        public int DeleteSchedule(int id) {
            return DatabaseContext.Database.Delete<Schedule>(id);
        }
    }
}