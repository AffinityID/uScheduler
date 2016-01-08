using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using uScheduler.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace uScheduler.Controllers {
    public class SchedulerApiController : UmbracoAuthorizedApiController {
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

        [HttpPost]
        public Schedule SaveSchedule(Schedule schedule) {
            if (schedule.Id > 0)
                DatabaseContext.Database.Update(schedule);
            else
                DatabaseContext.Database.Save(schedule);

            return schedule;
        }

        public int DeleteSchedule(int id) {
            return DatabaseContext.Database.Delete<Schedule>(id);
        }
    }
}